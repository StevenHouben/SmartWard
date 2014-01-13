using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

using ABC.Infrastructure;
using ABC.Infrastructure.ActivityBase;
using ABC.Infrastructure.Discovery;
using ABC.Infrastructure.Helpers;

using ABC.Model;
using ABC.Model.Device;
using ABC.Model.Users;
using SmartWard.Models;
using ABC.Model.Resources;
using SmartWard.Models.Notifications;


namespace SmartWard.Infrastructure
{
    public class WardNode : INotifyPropertyChanged
    {
        private ActivitySystem _activitySystem;
        private ActivityService _activityService;
        private ActivityClient _client;

        public event EventHandler<User> UserAdded;

        protected void OnPatientAdded(User user)
        {
            if (UserAdded != null)
                UserAdded(this, user);
        }
        public event EventHandler<User> UserChanged;
        protected void OnPatientChanged(User user)
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

        public event EventHandler<Notification> NotificationAdded;

        protected void OnNotificationAdded(Notification notification)
        {
            if (NotificationAdded != null)
                NotificationAdded(this, notification);
        }
        public event EventHandler<Notification> NotificationChanged;
        protected void OnNotificationChanged(Notification notification)
        {
            if (NotificationChanged != null)
                NotificationChanged(this, notification);
        }
        public event EventHandler<Notification> NotificationRemoved;
        protected void OnNotificationRemoved(Notification notification)
        {
            if (NotificationRemoved != null)
                NotificationRemoved(this, notification);
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

        public Dictionary<string, ABC.Model.Notifications.INotification> Notifications
        {
            get
            {
                if (_client != null)
                    return new Dictionary<string, ABC.Model.Notifications.INotification>(_client.Notifications);
                if (_activitySystem != null)
                    return new Dictionary<string, ABC.Model.Notifications.INotification>(_activitySystem.Notifications);
                return new Dictionary<string, ABC.Model.Notifications.INotification>();
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
        public Collection<IActivity> Activities { get; set; }
        public Collection<User> UserCollection { get; set; }

        public Collection<Resource> ResourceCollection { get; set; }

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
                    if (!IsLocationEnabled)
                        _activitySystem.StopLocationTracker();
                    else
                        _activitySystem.StartLocationTracker();
                }
                OnPropertyChanged("isLocationEnabled");
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
                        _activitySystem.StopBroadcast();
                    else

                        _activitySystem.StartBroadcast(DiscoveryType.Zeroconf, "HyPRBoard", "PIT-Lab");
                }
                OnPropertyChanged("isBroadcastEnabled");
            }
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
        public event EventHandler<Patient> EWSAdded;

        private WardNode(WardNodeConfiguration configuration, WebConfiguration webConfiguration)
        {
            _configuration = configuration;
            _webConfiguration = webConfiguration;

            UserCollection =  new ObservableCollection<User>();
            ResourceCollection = new ObservableCollection<Resource>();

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

        private void InitializeData(ActivityController controller)
        {
            foreach (var pat in controller.Users.Values.OfType<Patient>())
            {
                UserCollection.Add(pat);
            }
            foreach (var resource in controller.Resources.Values.OfType<Resource>())
            {
                ResourceCollection.Add(resource);
            }
        }

        private void StartClientAndSystem()
        {
            _activitySystem = new ActivitySystem("WardNode-" + Guid.NewGuid());

            _activitySystem.ConnectionEstablished += _activitySystem_ConnectionEstablished;

            InitializeEvents(_activitySystem);

            _activitySystem.Run(Net.GetUrl(_webConfiguration.Address, _webConfiguration.Port, "").ToString());

            _activitySystem.StartBroadcast(DiscoveryType.Zeroconf, 
                "Department-x", "Anonymous Hospital, 4th floor");

            _activitySystem.StartLocationTracker();
        }

        private void InitializeEvents(ActivityController controller)
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
                    OnPatientChanged(t);
                    break;
                }

            });
        }

        void node_UserAdded(object sender, UserEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var patient = e.User as Patient;

                if (patient != null)
                {
                    UserCollection.Add(patient);

                    OnPatientAdded(patient);
                }
            });
        }

        void node_ResourceRemoved(object sender, ResourceRemovedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                for (var i = 0; i < Resources.Count; i++)
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

        void node_DeviceRemoved(object sender, DeviceRemovedEventArgs e)
        {
            //OnDeviceRemoved(e);
        }

        void node_DeviceChanged(object sender, DeviceEventArgs e)
        {
            //OnDeviceChanged(e);
        }

        void node_DeviceAdded(object sender, DeviceEventArgs e)
        {
           // OnDeviceChanged(e);
        }

        void node_ActivityRemoved(object sender, ActivityRemovedEventArgs e)
        {
            //OnActivityRemoved(e);
        }

        void node_ActivityChanged(object sender, ActivityEventArgs e)
        {
            //OnActivityChanged(e);
        }

        void node_ActivityAdded(object sender, ActivityEventArgs e)
        {
           // OnActivityAdded(e);
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
