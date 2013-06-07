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
using System.Diagnostics;
using SmartWard.Devices;

namespace SmartWard.Infrastructure
{
    public class ActivitySystem
    {
        #region Properties
        public string Name { get; set; }
        public string IP { get; private set; }
        public int Port { get; private set; }
        public Dictionary<string,IActivity> Activities
        {
            get { return new Dictionary<string, IActivity>(activities); }
        }
        public Dictionary<string,IUser> Users
        {
            get { return new Dictionary<string,IUser>(users); }
        }
        public Dictionary<string, IDevice> Devices
        {
            get { return new Dictionary<string, IDevice>(devices); }
        }
        public LocationTracker Tracker { get; set; }
        #endregion

        #region Members
        private DocumentStore documentStore;
        private BroadcastService broadcast = new BroadcastService();
        private ConcurrentDictionary<string, IUser> users = new ConcurrentDictionary<string, IUser>();
        private ConcurrentDictionary<string, IActivity> activities = new ConcurrentDictionary<string,IActivity>();
        private ConcurrentDictionary<string, IDevice> devices = new ConcurrentDictionary<string, IDevice>();
        #endregion

        #region Events
        public event UserAddedHandler UserAdded     = delegate { };
        public event UserRemovedHandler UserRemoved = delegate { };
        public event UserChangedHandler UserUpdated = delegate { };

        public event ActivityAddedHandler ActivityAdded = delegate { };
        public event ActivityChangedHandler ActivityChanged = delegate { };
        public event ActivityRemovedHandler ActivityRemoved = delegate { };

        public event ConnectionEstablishedHandler ConnectionEstablished = delegate { };
        #endregion

        #region Constructor
        public ActivitySystem(string systemName="activitysystem")
        {
            Tracker = new LocationTracker();
            Name = systemName;
            IP = Net.GetIp(IPType.All);
            Port = 1000;

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
            documentStore = new DocumentStore()
            {
                Conventions =
                {
                    FindTypeTagName = type =>
                    {
                        if (typeof(IUser).IsAssignableFrom(type))
                            return "IUser";
                        if (typeof(IActivity).IsAssignableFrom(type))
                            return "IActivity";
                        if (typeof(IDevice).IsAssignableFrom(type))
                            return "IDevice";
                        return DocumentConvention.DefaultTypeTagName(type);
                    }
                }
            };
            documentStore.ParseConnectionString("Url = "+address);
            documentStore.Initialize();

            SubscribeToChanges();
            LoadStore();
            ConnectionEstablished(this, new EventArgs());
        }
        public T Cast<T>(object input)
        {
            return (T)input;
        }
        private void SubscribeToChanges()
        {
            documentStore.Changes("activitysystem").ForAllDocuments()
                .Subscribe(change =>
                {
                     using (var session = documentStore.OpenSession("activitysystem"))
                     {
                         var obj = session.Load<object>(change.Id);
                         if (obj is IUser)
                             HandleIUserMessages(change);
                         else if (obj is IActivity)
                             HandleActivityMessages(change);
                         else if (obj is IDevice)
                             HandleDeviceMessages(change);

                     }
                });
        }

        private void HandleDeviceMessages(Raven.Abstractions.Data.DocumentChangeNotification change)
        {
            throw new NotImplementedException();
        }
        private void HandleActivityMessages(Raven.Abstractions.Data.DocumentChangeNotification change)
        {
            switch (change.Type)
            {
                case Raven.Abstractions.Data.DocumentChangeTypes.Delete:
                    {
                        IActivity backupActivity;
                        activities.TryRemove(change.Id, out backupActivity);
                        ActivityRemoved(this, new ActivityRemovedEventArgs(change.Id));
                    }
                    break;
                case Raven.Abstractions.Data.DocumentChangeTypes.Put:
                    {
                        using (var session = documentStore.OpenSession("activitysystem"))
                        {
                            var activity = session.Load<IActivity>(change.Id);
                            if (activities.ContainsKey(change.Id))
                            {
                                activities[change.Id].UpdateAllProperties<IActivity>(activity);
                                ActivityChanged(this, new ActivityEventArgs(activities[change.Id]));
                            }
                            else
                            {

                                activities.AddOrUpdate(activity.Id, activity, (key, oldValue) => activity);
                            }
                        }
                    }
                    break;
                default:
                    Console.WriteLine(change.Type.ToString() + " received.");
                    break;
            }
        }
        private void HandleIUserMessages(Raven.Abstractions.Data.DocumentChangeNotification change)
        {
            switch (change.Type)
            {
                case Raven.Abstractions.Data.DocumentChangeTypes.Delete:
                    {
                        IUser backupUser;
                        users.TryRemove(change.Id,out backupUser);
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
                                users.AddOrUpdate(user.Id, user,(key, oldValue) => user);
                                UserAdded(this, new UserEventArgs(user));
                            }
                        }
                    }
                    break;
                default:
                    Console.WriteLine(change.Type.ToString() + " received.");
                    break;
            }
        }
        private void LoadStore()
        {
            using (var session = documentStore.OpenSession("activitysystem"))
            {
                var userResult = from user in session.Query<IUser>()
                                 where user.BaseType == typeof(IUser).Name
                                 select user;

                foreach (var entry in userResult)
                {
                    users.AddOrUpdate(entry.Id, entry, (key, oldValue) => entry);
                }

                var activityResult = from activity in session.Query<IActivity>() 
                                     where activity.BaseType == typeof(IActivity).Name
                                     select activity;

                foreach (var entry in activityResult)
                {
                    activities.AddOrUpdate(entry.Id, entry, (key, oldValue) => entry);
                }

                var deviceResult = from device in session.Query<IDevice>()
                                   where device.BaseType == typeof( IDevice).Name
                                   select device;

                foreach (var entry in deviceResult)
                {
                    devices.AddOrUpdate(entry.Id, entry, (key, oldValue) => entry);
                }
            }
        }
        private void AddToStore(INoo noo)
        {
            using (var session = documentStore.OpenSession(Name))
            {
                session.Store(noo);
                session.SaveChanges();
            }
        }
        private void UpdateStore(string id, INoo noo)
        {
            using (var session = documentStore.OpenSession(Name))
            {
                var obj = session.Load<INoo>(id);
                obj.UpdateAllProperties<INoo>(noo);
                session.SaveChanges();
            }
        }
        private void RemoveFromStore(string id)
        {
            using (var session = documentStore.OpenSession(Name))
            {
                var obj = session.Load<INoo>(id);
                session.Delete<INoo>(obj);
                session.SaveChanges();
            }
        }
        #endregion

        #region Public Methods
        public void Run(string storeAddress)
        {
            InitializeDocumentStore(storeAddress);
        }
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
            if(Tracker.IsRunning)
            {
                Tracker.Detection += tracker_Detection;
                Tracker.TagButtonDataReceived += Tracker_TagButtonDataReceived;
                Tracker.Start();
            }
        }
        public void StopLocationTracker()
        {
            if(Tracker.IsRunning)
                Tracker.Stop();
        }
        public IUser FindUserByCid(string cid)
        {
            using (var session = documentStore.OpenSession(Name))
            {
                var results = from user in session.Query<IUser>()
                              where user.Cid == cid
                              select user;
                var resultList = results.ToList<IUser>();
                if (resultList != null && resultList.Count > 0)
                {
                    return resultList[0];
                }
                else
                    return null;
            }

        }
        public void AddUser(IUser user)
        {
            AddToStore(user);
        }
        public void RemoveUser(string id)
        {
            RemoveFromStore(id);
        }
        public void UpdateUser(string id, IUser user)
        {
            UpdateStore(id, user);
        }
        public IUser GetUser(string id)
        {
            return users[id];
        }
        public void AddActivity(IActivity act)
        {
            AddToStore(act);
        }
        public void UpdateActivity(IActivity act)
        {
            UpdateStore(act.Id, act);
        }
        public void RemoveActivity(string id)
        {
            RemoveFromStore(id);
        }
        public IActivity GetActivity(string id)
        {
            return activities[id];
        }

        public void AddDevice(IDevice dev)
        {
            AddToStore(dev);
        }
        public void UpdateDevice(IDevice dev)
        {
            UpdateStore(dev.Id, dev);
        }
        public void RemoveDevice(string id)
        {
            RemoveFromStore(id);
        }
        public IDevice GetDevice(string id)
        {
            return devices[id];
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
