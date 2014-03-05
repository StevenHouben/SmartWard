using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

using NooSphere.Infrastructure;
using NooSphere.Infrastructure.ActivityBase;
using NooSphere.Infrastructure.Discovery;
using NooSphere.Infrastructure.Helpers;

using NooSphere.Model;
using NooSphere.Model.Device;
using NooSphere.Model.Users;
using SmartWard.Models;
using NooSphere.Model.Resources;
using SmartWard.Models.Notifications;
using NooSphere.Infrastructure.Web;
using NooSphere.Infrastructure.Web.Controllers;
using NooSphere.Infrastructure.Context.Location;
using System.Threading.Tasks;


namespace SmartWard.Infrastructure
{
    public class WardNode : INotifyPropertyChanged
    {
        private ActivitySystem _activitySystem;
        private ActivityService _activityService;
        private ActivityClient _client;

        public EventHandler<Activity> ActivityAdded;

        protected void OnActivityAdded(Activity activity)
        {
            if (ActivityAdded != null)
                ActivityAdded(this, activity);
        }

        public EventHandler<Activity> ActivityChanged;

        protected void OnActivityChanged(Activity activity)
        {
            if (ActivityChanged != null)
                ActivityChanged(this, activity);
        }

        public EventHandler<Activity> ActivityRemoved;

        protected void OnActivityRemoved(Activity activity)
        {
            if (ActivityRemoved != null)
                ActivityRemoved(this, activity);
        }

        public event EventHandler<User> UserAdded;

        protected void OnUserAdded(User user)
        {
            if (UserAdded != null)
                UserAdded(this, user);
        }
        public event EventHandler<User> UserChanged;
        protected void OnUserChanged(User user)
        {
            if (UserChanged != null)
                UserChanged(this, user);
        }
            
        public event EventHandler<User> UserRemoved;
        protected void OnUserRemoved(User user)
        {
            if (UserRemoved != null)
                UserRemoved(this, user);
        }

        public event EventHandler<Resource> ResourceAdded;

        protected void OnResourceAdded(Resource resource)
        {
            if (ResourceAdded != null)
                ResourceAdded(this, resource);
        }
        public event EventHandler<Resource> ResourceChanged;
        protected void OnResourceChanged(Resource resource)
        {
            if (ResourceChanged != null)
                ResourceChanged(this, resource);
        }
        public event EventHandler<Resource> ResourceRemoved;
        protected void OnResourceRemoved(Resource resource)
        {
            if (ResourceRemoved != null)
                ResourceRemoved(this, resource);
        }

        public event EventHandler<NooSphere.Model.Notifications.Notification> NotificationAdded;

        protected void OnNotificationAdded(NooSphere.Model.Notifications.Notification notification)
        {
            if (NotificationAdded != null)
                NotificationAdded(this, notification);
        }
        public event EventHandler<NooSphere.Model.Notifications.Notification> NotificationChanged;
        protected void OnNotificationChanged(NooSphere.Model.Notifications.Notification notification)
        {
            if (NotificationChanged != null)
                NotificationChanged(this, notification);
        }
        public event EventHandler<NooSphere.Model.Notifications.Notification> NotificationRemoved;
        protected void OnNotificationRemoved(NooSphere.Model.Notifications.Notification notification)
        {
            if (NotificationRemoved != null)
                NotificationRemoved(this, notification);
        }

        public event EventHandler<Device> DeviceAdded;
        protected void OnDeviceAdded(Device device)
        {
            if (DeviceAdded != null)
                DeviceAdded(this, device);
        }
        public event EventHandler<Device> DeviceChanged;
        protected void OnDeviceChanged(Device device)
        {
            if (DeviceChanged != null)
                DeviceChanged(this, device);
        }
        public event EventHandler<Device> DeviceRemoved;
        protected void OnDeviceRemoved(Device device)
        {
            if (DeviceRemoved != null)
                DeviceRemoved(this, device);
        }

        public Dictionary<string, IUser> Users
        {
            get
            {
                if (_client != null)
                    return new Dictionary<string, IUser>(_client.Users);
                if(_activitySystem != null)
                    return new Dictionary<string, IUser>(_activitySystem.Users);
                return new Dictionary<string, IUser>();
            }
        }
        public Dictionary<string, IResource> Resources
        {
            get
            {
                if (_client != null)
                    return new Dictionary<string, IResource>(_client.Resources);
                if (_activitySystem != null)
                    return new Dictionary<string, IResource>(_activitySystem.Resources);
                return new Dictionary<string, IResource>();
            }
        }

        public Dictionary<string, NooSphere.Model.Notifications.INotification> Notifications
        {
            get
            {
                if (_client != null)
                    return new Dictionary<string, NooSphere.Model.Notifications.INotification>(_client.Notifications);
                if (_activitySystem != null)
                    return new Dictionary<string, NooSphere.Model.Notifications.INotification>(_activitySystem.Notifications);
                return new Dictionary<string, NooSphere.Model.Notifications.INotification>();
            }
        }

        public Dictionary<string, IDevice> Devices
        {
            get
            {
                if (_client != null)
                    return new Dictionary<string, IDevice>(_client.Devices);
                if (_activitySystem != null)
                    return new Dictionary<string, IDevice>(_activitySystem.Devices);
                return new Dictionary<string, IDevice>();
            }
        }
        #region Events
        //public event UserAddedHandler UserAdded = delegate { };

        //protected virtual void OnUserAdded(UserEventArgs e)
        //{
        //    var handler = UserAdded;
        //    if (handler != null) handler(this, e);
        //}

        //public event UserRemovedHandler UserRemoved = delegate { };

        //protected virtual void OnUserRemoved(UserRemovedEventArgs e)
        //{
        //    var handler = UserRemoved;
        //    if (handler != null) handler(this, e);
        //}

        //public event UserChangedHandler UserChanged = delegate { };

        //protected virtual void OnUserChanged(UserEventArgs e)
        //{
        //    var handler = UserChanged;
        //    if (handler != null) handler(this, e);
        //}

        //public event ActivityAddedHandler ActivityAdded = delegate { };

        //protected virtual void OnActivityAdded(ActivityEventArgs e)
        //{
        //    var handler = ActivityAdded;
        //    if (handler != null) handler(this, e);
        //}

        //public event ActivityChangedHandler ActivityChanged = delegate { };

        //protected virtual void OnActivityChanged(ActivityEventArgs e)
        //{
        //    var handler = ActivityChanged;
        //    if (handler != null) handler(this, e);
        //}

        //public event ActivityRemovedHandler ActivityRemoved = delegate { };

        //protected virtual void OnActivityRemoved(ActivityRemovedEventArgs e)
        //{
        //    var handler = ActivityRemoved;
        //    if (handler != null) handler(this, e);
        //}

        //public event DeviceAddedHandler DeviceAdded = delegate { };

        //protected virtual void OnDeviceAdded(DeviceEventArgs e)
        //{
        //    var handler = DeviceAdded;
        //    if (handler != null) handler(this, e);
        //}

        //public event DeviceChangedHandler DeviceChanged = delegate { };

        //protected virtual void OnDeviceChanged(DeviceEventArgs e)
        //{
        //    var handler = DeviceChanged;
        //    if (handler != null) handler(this, e);
        //}

        //public event DeviceRemovedHandler DeviceRemoved = delegate { };

        //protected virtual void OnDeviceRemoved(DeviceRemovedEventArgs e)
        //{
        //    var handler = DeviceRemoved;
        //    if (handler != null) handler(this, e);
        //}

        //public event ConnectionEstablishedHandler ConnectionEstablished = delegate { };

        //protected virtual void OnConnectionEstablished()
        //{
        //    var handler = ConnectionEstablished;
        //    if (handler != null) handler(this, EventArgs.Empty);
        //}

        #endregion

        #region Properties
        public Collection<Activity> ActivityCollection { get; set; }
        public Collection<User> UserCollection { get; set; }
        public Collection<Resource> ResourceCollection { get; set; }
        public Collection<NooSphere.Model.Notifications.Notification> NotificationCollection { get; set; }
        public Collection<Device> DeviceCollection { get; set; }

        private readonly WardNodeConfiguration _configuration;
        private readonly WebConfiguration _webConfiguration;

        private bool _isLocationEnabled = true;
        public bool IsLocationEnabled
        {
            get { return _isLocationEnabled; }
            set
            {
                _isLocationEnabled = value;
                if (_activitySystem != null)
                {
                    if (!IsLocationEnabled) {
                        _activitySystem.UnsubscribeToTagMoved(TagMovedHandler);
                        _activitySystem.StopLocationTracker();
                    }
                    else
                    {
                        _activitySystem.StartLocationTracker();
                        _activitySystem.SubscribeToTagMoved(TagMovedHandler);
                    }
                }
                OnPropertyChanged("isLocationEnabled");
            }
        }

        private void TagMovedHandler(Detector detector, TagEventArgs e)
        {
            lock (this) //Location tracking threads are NOT allowed to modify the database concurrently.
            {
                Devices.Values.ToList().ForEach(d =>
                {
                    if (d.TagValue == e.Tag.Id && d.Location != e.Tag.Detector.Name)
                    {
                        d.Location = e.Tag.Detector.Name;
                        UpdateDevice(d as Device);
                    }
                });
                Users.Values.ToList().ForEach(u =>
                {
                    if (u.Tag == e.Tag.Id && u.Location != e.Tag.Detector.Name)
                    {
                        u.Location = e.Tag.Detector.Name;
                        UpdateUser(u as User);
                    }
                });
            }
        }

        private bool _isWebApiEnabled = true;
        public bool IsWebApiEnabled
        {
            get { return _isWebApiEnabled; }
            set
            {
                _isWebApiEnabled = value;
                if (_activityService != null)
                {
                    if (!_isWebApiEnabled)
                        _activityService.Stop();
                    else
                        _activityService.Start(Net.GetIp(IpType.All), 8000);
                }
                OnPropertyChanged("isWebApiEnabled");
            }
        }

        private bool _isBroadcastEnabled = true;
        public bool IsBroadcastEnabled
        {
            get { return _isBroadcastEnabled; }
            set
            {
                _isBroadcastEnabled = value;
                if (_activitySystem != null)
                {
                    if (!IsBroadcastEnabled)
                        _activityService.StopBroadcast();
                    else

                        _activityService.StartBroadcast(DiscoveryType.Zeroconf, "HyPRBoard", "PIT-Lab", "1337");
                }
                OnPropertyChanged("isBroadcastEnabled");
            }
        }
            
        public void SetClientDeviceUser(User user)
        {
            if (_client != null)
            {
                _client.Device.Owner = user;
                UpdateDevice((Device)_client.Device);
            }

        }
        public void SetClientDeviceTag(String s)
        {
            if (_client != null)
            {
                _client.Device.TagValue = s;
                _client.Device.Location = GetTag(s);
                UpdateDevice((Device)_client.Device);
            }
        }

        public void RemoveClientDevice()
        {
            if (_client != null) RemoveDevice((Device)_client.Device);
        }

        public IDevice GetClientDevice()
        {
            if (_client == null) return null;
            return _client.Device;
        }
        #endregion


        public static event EventHandler<WebConfiguration> WardNodeFound;

        public static void FindWardNodes()
        {
            var disco = new DiscoveryManager();
            disco.DiscoveryAddressAdded += (sender, e) =>
            {
                if (WardNodeFound != null)
                    WardNodeFound(disco,new WebConfiguration(e.ServiceInfo.Address));
            };
            disco.Find(DiscoveryType.Zeroconf);
        }

        public static WardNode StartWardNodeAsClient(WebConfiguration webConfiguration)
        {
            return new WardNode(WardNodeConfiguration.Client, webConfiguration);
        }
        
        public static WardNode StartWardNodeAsSystem(WebConfiguration webConfiguration)
        {
            return new WardNode(WardNodeConfiguration.System, webConfiguration);
        }

        private WardNode(WardNodeConfiguration configuration, WebConfiguration webConfiguration)
        {
            _configuration = configuration;
            _webConfiguration = webConfiguration;

            ActivityCollection = new ObservableCollection<Activity>();
            UserCollection =  new ObservableCollection<User>();
            ResourceCollection = new ObservableCollection<Resource>();
            NotificationCollection = new ObservableCollection<NooSphere.Model.Notifications.Notification>();
            DeviceCollection = new ObservableCollection<Device>();

            StartNode();
        }
        private void StartNode()
        {
            switch (_configuration)
            {
                case WardNodeConfiguration.System:
                    StartClientAndSystem();
                    break;
                case WardNodeConfiguration.Client:
                    StartClient();
                    break;
            }
        }
        private void StartClient()
        {
            _client = new ActivityClient(_webConfiguration.Address, _webConfiguration.Port, new Device());

            InitializeEvents(_client);

            InitializeData(_client);

        }
        
        private void InitializeData(ActivityNode node)
        {
            foreach (var activity in node.Activities.Values.OfType<Activity>())
            {
                ActivityCollection.Add(activity);
            }
            foreach (var user in node.Users.Values.OfType<User>())
            {
                UserCollection.Add(user);
            }
            foreach (var resource in node.Resources.Values.OfType<Resource>())
            {
                ResourceCollection.Add(resource);
            }
            foreach (var notification in node.Notifications.Values.OfType<NooSphere.Model.Notifications.Notification>())
            {
                NotificationCollection.Add(notification);
            }
            foreach (var device in node.Devices.Values.OfType<Device>())
            {
                DeviceCollection.Add(device);
            }
        }

        private void StartClientAndSystem()
        {
            var webconfiguration = WebConfiguration.DefaultWebConfiguration;

            const string ravenDatabaseName = "activitysystem";

            var databaseConfiguration = new DatabaseConfiguration(webconfiguration.Address, webconfiguration.Port, ravenDatabaseName);
            _activitySystem = new ActivitySystem(databaseConfiguration);

            InitializeEvents(_activitySystem);

            _activitySystem.StartLocationTracker();
            _activitySystem.SubscribeToTagMoved(TagMovedHandler);

            InitializeData(_activitySystem);

            _activityService = new ActivityService(_activitySystem, Net.GetIp(IpType.All), 8070);
            _activityService.Start();


            _activityService.StartBroadcast(DiscoveryType.Zeroconf,
                "Department-x", "Anonymous Hospital, 4th floor", "1337");
        }

        private void InitializeEvents(ActivityNode controller)
        {
            controller.ActivityAdded += node_ActivityAdded;
            controller.ActivityChanged += node_ActivityChanged;
            controller.ActivityRemoved += node_ActivityRemoved;
            controller.DeviceAdded += node_DeviceAdded;
            controller.DeviceChanged += node_DeviceChanged;
            controller.DeviceRemoved += node_DeviceRemoved;
            controller.UserAdded += node_UserAdded;
            controller.UserChanged += node_UserChanged;
            controller.UserRemoved += node_UserRemoved;
            controller.ResourceAdded += node_ResourceAdded;
            controller.ResourceChanged += node_ResourceChanged;
            controller.ResourceRemoved += node_ResourceRemoved;
            controller.NotificationAdded += node_NotificationAdded;
            controller.NotificationChanged += node_NotificationChanged;
            controller.NotificationRemoved += node_NotificationRemoved;
        }

        void node_ActivityRemoved(object sender, ActivityRemovedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                for (var i = 0; i < ActivityCollection.Count; i++)
                {
                    if (ActivityCollection[i].Id == e.Id)
                    {
                        OnActivityRemoved(ActivityCollection[i]);
                        ActivityCollection.RemoveAt(i);
                        break;
                    }
                }
            });
        }

        void node_ActivityChanged(object sender, ActivityEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (var t in ActivityCollection.Where(t => t.Id == e.Activity.Id))
                {
                    t.UpdateAllProperties(e.Activity);
                    OnActivityChanged(t);
                    break;
                }

            });
        }

        void node_ActivityAdded(object sender, ActivityEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var activity = e.Activity as Activity;

                if (activity != null)
                {
                    ActivityCollection.Add(activity);
                    OnActivityAdded(activity);
                }
            });
        }
        void node_UserRemoved(object sender, UserRemovedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                for (var i = 0; i < UserCollection.Count; i++)
                {
                    if (UserCollection[i].Id == e.Id)
                    {
                        OnUserRemoved(UserCollection[i]);
                        UserCollection.RemoveAt(i);
                        break;
                    }
                }
            });
        }

        void node_UserChanged(object sender, UserEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (var t in UserCollection.Where(t => t.Id == e.User.Id))
                {
                    t.UpdateAllProperties(e.User);
                    OnUserChanged(t);
                    break;
                }

            });
        }

        void node_UserAdded(object sender, UserEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                UserCollection.Add((User)e.User);
                OnUserAdded((User)e.User);
            });
        }

        void node_ResourceRemoved(object sender, ResourceRemovedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                for (var i = 0; i < ResourceCollection.Count; i++)
                {
                    if (ResourceCollection[i].Id == e.Id)
                    {
                        OnResourceRemoved(ResourceCollection[i]);
                        ResourceCollection.RemoveAt(i);
                        break;
                    }
                }
            });
        }

        void node_ResourceChanged(object sender, ResourceEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (var t in ResourceCollection.Where(t => t.Id == e.Resource.Id))
                {
                    t.UpdateAllProperties(e.Resource);
                    OnResourceChanged(t);
                    break;
                }

            });
        }

        void node_ResourceAdded(object sender, ResourceEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var res = e.Resource as Resource;

                if (res != null)
                {
                    ResourceCollection.Add(res);

                    OnResourceAdded(res);
                }
            });
        }

        void node_NotificationRemoved(object sender, NotificationRemovedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                for (var i = 0; i < NotificationCollection.Count; i++)
                {
                    if (NotificationCollection[i].Id == e.Id)
                    {
                        OnNotificationRemoved(NotificationCollection[i]);
                        NotificationCollection.RemoveAt(i);
                        break;
                    }
                }
            });
        }

        void node_NotificationChanged(object sender, NotificationEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (var t in NotificationCollection.Where(t => t.Id == e.Notification.Id))
                {
                    t.UpdateAllProperties(e.Notification);
                    OnNotificationChanged((Notification)t);
                    break;
                }
            });
        }

        void node_NotificationAdded(object sender, NotificationEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var res = e.Notification as Notification;
                if (res != null)
                {
                    NotificationCollection.Add(res);
                    OnNotificationAdded(res);
                }
            });
        }

        void node_DeviceRemoved(object sender, DeviceRemovedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    for (var i = 0; i < DeviceCollection.Count; i++)
                    {
                        if (DeviceCollection[i].Id == e.Id)
                        {
                            OnDeviceRemoved(DeviceCollection[i]);
                            DeviceCollection.RemoveAt(i);
                            break;
                        }
                    }
                });
            } catch (TaskCanceledException exception)
            {
                // Exception thrown when closing the PDA application.
                // The application removes the device its run on from the infrastructure on close.
                // This eventhandler is run when the infrastructure fires an event that the
                // device has in fact been removed. This eventhandler tries to remove
                // the device from the application's device collection, however as the application
                // is closed or closing it is not possible, and the exception is thrown. The exception
                // is just swallowed as the application is closed and thus it is obviously not needed 
                // to update its device collection.
                // 
            }
        }

        void node_DeviceChanged(object sender, DeviceEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (var t in DeviceCollection.Where(t => t.Id == e.Device.Id))
                {
                    t.UpdateAllProperties(e.Device);
                    OnDeviceChanged((Device)t);
                    break;
                }
            });
        }

        void node_DeviceAdded(object sender, DeviceEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var res = e.Device as Device;
                if (res != null)
                {
                    DeviceCollection.Add(res);
                    OnDeviceAdded(res);
                }
            });
        }

        public void AddActivity(Activity activity)
        {
            if (_client != null)
                _client.AddActivity(activity);
            else if (_activitySystem != null)
                _activitySystem.AddActivity(activity);
        }
        public void UpdateActivity(Activity activity)
        {
            if (_client != null)
                _client.UpdateActivity(activity);
            else if (_activitySystem != null)
                _activitySystem.UpdateActivity(activity);
        }
        public void RemoveActivity(string Id)
        {
            if (_client != null)
                _client.RemoveActivity(Id);
            else if (_activitySystem != null)
                _activitySystem.RemoveActivity(Id);
        }
        public void AddUser(User user)
        {
            if(_client != null)
                _client.AddUser(user);
            else if(_activitySystem != null)
                _activitySystem.AddUser(user);
        }
        public void UpdateUser(User user)
        {
            if (_client != null)
                _client.UpdateUser(user);
            else if (_activitySystem != null)
                _activitySystem.UpdateUser(user);
        }

        public void AddResource(Resource resource)
        {

            if (_client != null)
                _client.AddResource(resource);
            else if (_activitySystem != null)
                _activitySystem.AddResource(resource);
        }
        public void UpdateResource(Resource resource)
        {
            if (_client != null)
                _client.UpdateResource(resource);
            else if (_activitySystem != null)
                _activitySystem.UpdateResource(resource);
        }

        public void AddNotification(Notification notification)
        {
            if (_client != null)
                _client.AddNotification(notification);
            else if (_activitySystem != null)
                _activitySystem.AddNotification(notification);
        }
        public void UpdateNotification(Notification notification)
        {
            if (_client != null)
                _client.UpdateNotification(notification);
            else if (_activitySystem != null)
                _activitySystem.UpdateNotification(notification);
        }
        public void RemoveNotification(string Id)
        {
            if (_client != null)
                _client.RemoveNotification(Id);
            else if (_activitySystem != null)
                _activitySystem.RemoveNotification(Id);
        }

        public void AddDevice(Device device)
        {
            if (_client != null)
                _client.AddDevice(device);
            else if (_activitySystem != null)
                _activitySystem.AddDevice(device);
        }
        public void RemoveDevice(Device device)
        {
            if (_client != null)
                _client.RemoveDevice(device.Id);
            else if (_activitySystem != null)
                _activitySystem.RemoveDevice(device.Id);
        }
        public void UpdateDevice(Device device)
        {
            if (_client != null)
                _client.UpdateDevice(device);
            else if (_activitySystem != null)
                _activitySystem.UpdateDevice(device);
        }

        public string GetTag(string id)
        {
            if (_client != null)
               return _client.GetTagLocation(id);
            return null;
        }

        void _activitySystem_ConnectionEstablished(object sender, EventArgs e)
        {
            InitializeData(_activitySystem);

            _activityService = new ActivityService(_activitySystem, Net.GetIp(IpType.All), 8000);
            _activityService.Start();
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
               PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        #endregion

    }
}
