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
using System.Collections.Concurrent;

namespace SmartWard.Infrastructure
{
    public class ActivitySystem
    {
        #region Properties
        public string Name { get; set; }
        public string IP { get; private set; }
        public int Port { get; private set; }
        public Dictionary<string,Activity> Activities
        {
            get { return new Dictionary<string, Activity>(activities); }
        }
        public Dictionary<string,IUser> Users
        {
            get { return users; }
        }
        public LocationTracker Tracker { get; set; }
        #endregion

        #region Members
        private DocumentStore documentStore = new DocumentStore();
        private BroadcastService broadcast = new BroadcastService();
        private Dictionary<string,IUser> users = new Dictionary<string,IUser>();
        private ConcurrentDictionary<string, Activity> activities = new ConcurrentDictionary<string,Activity>();
        #endregion

        #region Events
        public event UserAddedHandler UserAdded     = delegate { };
        public event UserRemovedHandler UserRemoved = delegate { };
        public event UserChangedHandler UserUpdated = delegate { };

        public event ActivityAddedHandler ActivityAdded = delegate { };
        public event ActivityChangedHandler ActivityChanged = delegate { };
        public event ActivityRemovedHandler ActivityRemoved = delegate { };
        #endregion

        #region Constructor
        public ActivitySystem(string storeAddress,string systemName="activitysystem")
        {
            Tracker = new LocationTracker();
            Name = systemName;
            IP = Net.GetIp(IPType.All);
            Port = 1000;

            InitializeDocumentStore(storeAddress);
        }
        ~ActivitySystem()
        {
            StopBroadcast();
            StopLocationTracker();
        }
        #endregion

        #region Eventhandlers
        private void Tracker_TagButtonDataReceived(Tag tag, TagEventArgs e)
        {
            var col = new Collection<IUser>(users.Values.ToList());
            if (col.Contains(u => u.Tag == e.Tag.Id))
            {
                int index = col.FindIndex(u => u.Tag == e.Tag.Id);
                
                if (e.Tag.ButtonA == ButtonState.Pressed)
                {
                    users[col[index].Id].State = 2;
                    users[col[index].Id].Selected = true;
                }
                else if (e.Tag.ButtonB == ButtonState.Pressed)
                {
                    users[col[index].Id].State = 1;
                    users[col[index].Id].Selected = true;
                }
                else
                {
                    users[col[index].Id].State = 0;
                    users[col[index].Id].Selected = true;
                }
                UserUpdated(this, new UserEventArgs(users[col[index].Id]));
            }
        }
        private void tracker_Detection(Detector detector, DetectionEventArgs e)
        {
           
        }
        #endregion

        #region Privat Methods
        private void InitializeDocumentStore(string address)
        {
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
                                users.Remove(change.Id);
                                UserRemoved(this, new UserRemovedEventArgs(change.Id));
                            }
                            break;
                        case Raven.Abstractions.Data.DocumentChangeTypes.Put:
                            {
                                using (var session = documentStore.OpenSession("activitysystem"))
                                {
                                    var user = session.Load<IUser>(change.Id);
                                    if (users.ContainsKey(change.Id))
                                    {
                                        users[change.Id].UpdateAllProperties<IUser>(user);
                                        UserUpdated(this, new UserEventArgs(user));
                                    }
                                    else
                                    {
                                        users.Add(user.Id,user);
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
                    users.Add(entry.Id,entry);
                }
            }
        }
        #endregion

        #region Public Methods
        public void StartBroadcast(DiscoveryType type, string hostName, string location = "undefined", string code = "-1")
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
        public void StopLocationTracker()
        {
            Tracker.Stop();
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
        public void UpdateUser<T>(string id, IUser user)
        {
            using (var session = documentStore.OpenSession(Name))
            {
                var usr = session.Load<T>(id);
                ((IUser)usr).UpdateAllProperties<T>(user);
                session.SaveChanges();
            }
        }
        public void AddActivity(Activity act)
        {
            activities.AddOrUpdate(act.Id, act, (key, oldValue) => act);
        }
        public void UpdateActivity(Activity act)
        {
            activities.TryUpdate(act.Id, act, act);
        }
        public void RemoveActivity(string id)
        {
            var backup = new Activity();
            activities.TryRemove(id, out backup);
        }
        public Activity GetActivity(string id)
        {
            return activities[id];
        }
        #endregion
    }

    #region Extension Methods
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
    #endregion
}
