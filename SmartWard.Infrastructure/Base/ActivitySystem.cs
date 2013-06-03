using SmartWard.Infrastructure.Discovery;
using SmartWard.Infrastructure.Helpers;
using SmartWard.Infrastructure.Location;
using SmartWard.Infrastructure.Location.Sonitor;
using SmartWard.Model;
using SmartWard.Primitives;
using SmartWard.Users;
using Raven.Client.Document;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartWard.Infrastructure
{
    public class ActivitySystem
    {
        public string Name { get; set; }
        private DocumentStore documentStore;
        public Collection<User> Users { get; private set; }
        public Collection<Activity> Activities { get; private set; }

        public LocationTracker Tracker { get; set; }

        public event UserAddedHandler UserAdded = delegate { };
        public event UserRemovedHandler UserRemoved = delegate { };
        public event UserChangedHandler UserUpdated = delegate { };

        public string IP { get; private set; }
        public int Port { get; private set; }

        private readonly BroadcastService broadcast = new BroadcastService();

        public ActivitySystem(string address,string systemName="activitysystem")
        {
            Tracker = new LocationTracker();
            Name = systemName;
            IP = Net.GetIp(IPType.All);
            Port = 1000;
            Users = new Collection<User>();
            Activities = new Collection<Activity>();
            InitializeDocumentStore(address);
        }

        public void StartBroadcast(DiscoveryType type, string hostName,string location = "undefined",string code="-1" )
        {
            var t = new Thread(() =>
            {
                StopBroadcast();
                broadcast.Start(type, hostName, location, code,
                                 Net.GetUrl(IP, Port, ""));
            }) { IsBackground = true };
            t.Start();
        }
        public void StopBroadcast()
        {
            if (broadcast != null)
                if (broadcast.IsRunning)
                    broadcast.Stop();
        }

        public void StartLocationTracker()
        {
            Tracker.Detection += tracker_Detection;
            Tracker.TagButtonDataReceived += Tracker_TagButtonDataReceived;
            Tracker.Start();
        }

        void Tracker_TagButtonDataReceived(Tag tag, TagEventArgs e)
        {
            if (Users.Contains(u => u.Tag == e.Tag.Id))
            {
                int index = Users.FindIndex(u => u.Tag == e.Tag.Id);

                if (e.Tag.ButtonA == ButtonState.Pressed)
                {
                    Users[index].State = 2;
                    Users[index].Selected = true;
                }
                else if (e.Tag.ButtonB == ButtonState.Pressed)
                {
                    Users[index].State = 1;
                    Users[index].Selected = true;
                }
                else
                {
                    Users[index].State = 0;
                    Users[index].Selected = true;
                }
                UserUpdated(this, new UserEventArgs(Users[index]));
            }
        }
        public void StopLocationTracker()
        {
            Tracker.Stop();
        }
        private void tracker_Detection(Detector detector, DetectionEventArgs e)
        {
           
        }

        private void InitializeDocumentStore(string address)
        {
            documentStore = new DocumentStore();
            documentStore.ParseConnectionString("Url = "+address);
            documentStore.Initialize();

            SubscribeToChanges();
            LoadDocuments();
        }
        private void SubscribeToChanges()
        {
            documentStore.Changes("activitysystem").ForAllDocuments()
                .Subscribe(change =>
                {
                    switch (change.Type)
                    {
                        case Raven.Abstractions.Data.DocumentChangeTypes.Delete:
                            {
                                Users.Remove(u => u.Id == change.Id);
                                UserRemoved(this, new UserRemovedEventArgs(change.Id));
                            }
                            break;
                        case Raven.Abstractions.Data.DocumentChangeTypes.Put:
                            {
                                using (var session = documentStore.OpenSession("activitysystem"))
                                {
                                    var user = session.Load<User>(change.Id);
                                    if (Users.Contains(u => u.Id == change.Id))
                                    {
                                        int index = Users.FindIndex(u => u.Id == change.Id);
                                        Users[index] = user;
                                        UserUpdated(this, new UserEventArgs(user));
                                    }
                                    else
                                    {
                                        Users.Add(user);
                                        UserAdded(this, new UserEventArgs(user));
                                    }
                                }
                            }
                            break;
                        default:
                            Console.WriteLine(change.Type.ToString() + " received.");
                            break;
                    }

                });
        }
        private void LoadDocuments()
        {
            using (var session = documentStore.OpenSession("activitysystem"))
            {
                var results = from user in session.Query<Patient>() select user;

                foreach (var entry in results)
                {
                    Users.Add(entry);
                }
            }
        }


        public User FindUserByCid(string cid)
        {
            using (var session = documentStore.OpenSession(Name))
            {
                var results = from user in session.Query<User>()
                              where user.Cid == cid
                              select user;
                var resultList = results.ToList<User>();
                if (resultList != null && resultList.Count > 0)
                {
                    return resultList[0];
                }
                else
                    return null;
            }

        }
        public void AddUser(User user)
        {
            using (var session = documentStore.OpenSession(Name))
            {
                session.Store(user);
                session.SaveChanges();
            }

        }
        public void UpdateUser<T>(string id, User user)
        {
            using (var session = documentStore.OpenSession(Name))
            {
                var usr = session.Load<T>(id);
                ((IUser)usr).UpdateAllProperties<T>(user);
                session.SaveChanges();
            }
        }

    }
    public static class ExtensionMethods
    {
        public static Collection<T> Remove<T>(
            this Collection<T> coll, Func<T, bool> condition)
        {
            var itemsToRemove = coll.Where(condition).ToList();

            foreach (var itemToRemove in itemsToRemove)
            {
                coll.Remove(itemToRemove);
            }

            return coll;
        }
        public static bool Contains<T>(
           this Collection<T> coll, Func<T, bool> condition)
        {
            if (coll == null) throw new ArgumentNullException("collection");
            if (condition == null) throw new ArgumentNullException("condition");
            var contains = coll.Where(condition).ToList();
            if (contains.Count > 0)
                return true;
            else
                return false;
        }
        public static int FindIndex<T>(this IEnumerable<T> items, Func<T, bool> predicate)
        {
            if (items == null) throw new ArgumentNullException("items");
            if (predicate == null) throw new ArgumentNullException("predicate");

            int retVal = 0;
            foreach (var item in items)
            {
                if (predicate(item)) return retVal;
                retVal++;
            }
            return -1;
        }
    }
}
