using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using ABC.Infrastructure;
using ABC.Infrastructure.ActivityBase;
using ABC.Infrastructure.Helpers;
using ABC.Model;
using ABC.Model.Device;
using ABC.Model.Users;
using SmartWard.Model;
using Action = System.Action;


namespace SmartWard.Infrastructure
{
    public class WardNode : INotifyPropertyChanged
    {
        private ActivitySystem _activitySystem;
        private ActivityService _activityService;
        private ActivityClient _client;

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

        #region Events
        public event UserAddedHandler UserAdded = delegate { };

        protected virtual void OnUserAdded(UserEventArgs e)
        {
            var handler = UserAdded;
            if (handler != null) handler(this, e);
        }

        public event UserRemovedHandler UserRemoved = delegate { };

        protected virtual void OnUserRemoved(UserRemovedEventArgs e)
        {
            var handler = UserRemoved;
            if (handler != null) handler(this, e);
        }

        public event UserChangedHandler UserChanged = delegate { };

        protected virtual void OnUserChanged(UserEventArgs e)
        {
            var handler = UserChanged;
            if (handler != null) handler(this, e);
        }

        public event ActivityAddedHandler ActivityAdded = delegate { };

        protected virtual void OnActivityAdded(ActivityEventArgs e)
        {
            var handler = ActivityAdded;
            if (handler != null) handler(this, e);
        }

        public event ActivityChangedHandler ActivityChanged = delegate { };

        protected virtual void OnActivityChanged(ActivityEventArgs e)
        {
            var handler = ActivityChanged;
            if (handler != null) handler(this, e);
        }

        public event ActivityRemovedHandler ActivityRemoved = delegate { };

        protected virtual void OnActivityRemoved(ActivityRemovedEventArgs e)
        {
            var handler = ActivityRemoved;
            if (handler != null) handler(this, e);
        }

        public event DeviceAddedHandler DeviceAdded = delegate { };

        protected virtual void OnDeviceAdded(DeviceEventArgs e)
        {
            var handler = DeviceAdded;
            if (handler != null) handler(this, e);
        }

        public event DeviceChangedHandler DeviceChanged = delegate { };

        protected virtual void OnDeviceChanged(DeviceEventArgs e)
        {
            var handler = DeviceChanged;
            if (handler != null) handler(this, e);
        }

        public event DeviceRemovedHandler DeviceRemoved = delegate { };

        protected virtual void OnDeviceRemoved(DeviceRemovedEventArgs e)
        {
            var handler = DeviceRemoved;
            if (handler != null) handler(this, e);
        }

        public event ConnectionEstablishedHandler ConnectionEstablished = delegate { };

        protected virtual void OnConnectionEstablished()
        {
            var handler = ConnectionEstablished;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        #endregion

        #region Properties
        public ObservableCollection<IActivity> Activities { get; set; }
        public ObservableCollection<Patient> Patients { get; set; }

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

                        _activitySystem.StartBroadcast(ABC.Infrastructure.Discovery.DiscoveryType.Zeroconf, "HyPRBoard", "PIT-Lab");
                }
                OnPropertyChanged("isBroadcastEnabled");
            }
        }
  
        #endregion

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

            Patients =  new ObservableCollection<Patient>();

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
            controller.UserRemoved += _client_UserRemoved;
        }

        private void StartClientAndSystem()
        {
            _activitySystem = new ActivitySystem(systemName: "WardNode-" + Guid.NewGuid());

            _activitySystem.ConnectionEstablished += _activitySystem_ConnectionEstablished;

            InitializeEvents(_activitySystem);

            _activitySystem.Run(Net.GetUrl(_webConfiguration.Address, _webConfiguration.Port, "").ToString());

            foreach (var usr in _activitySystem.Users.Values)
            {
                var pat = usr as Patient;
                if (pat != null)
                {
                    pat.PropertyChanged += p_PropertyChanged;
                    Patients.Add(pat);
                }
            }
            //_activitySystem.StartBroadcast(ABC.Infrastructure.Discovery.DiscoveryType.Zeroconf, "HyPRBoard", "PIT-Lab");

            //_activitySystem.StartLocationTracker();
        }

        void _client_UserRemoved(object sender, UserRemovedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                for (var i = 0; i < Patients.Count; i++)
                {
                    if (Patients[i].Id != e.Id) continue;
                    Patients.RemoveAt(i);
                    break;
                }
            }));
            OnUserRemoved(e);
        }

        void node_UserChanged(object sender, UserEventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                foreach (var t in Patients.Where(t => t.Id == e.User.Id))
                {
                    t.UpdateAllProperties(e.User);
                    break;
                }
            }));
            OnUserChanged(e);

        }

        void node_UserAdded(object sender, UserEventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                var patient = e.User as Patient;

                if (patient != null)
                {
                    patient.PropertyChanged += p_PropertyChanged;
                    Patients.Add((Patient)e.User);

                }
            }));
            OnUserAdded(e);
        }

        void node_DeviceRemoved(object sender, DeviceRemovedEventArgs e)
        {
            OnDeviceRemoved(e);
        }

        void node_DeviceChanged(object sender, DeviceEventArgs e)
        {
            OnDeviceChanged(e);
        }

        void node_DeviceAdded(object sender, DeviceEventArgs e)
        {
            OnDeviceChanged(e);
        }

        void node_ActivityRemoved(object sender, ActivityRemovedEventArgs e)
        {
            OnActivityRemoved(e);
        }

        void node_ActivityChanged(object sender, ActivityEventArgs e)
        {
            OnActivityChanged(e);
        }

        void node_ActivityAdded(object sender, ActivityEventArgs e)
        {
            OnActivityAdded(e);
        }

        public void AddPatient(Patient p)
        {
            if(_client != null)
                _client.AddUser(p);
            else if(_activitySystem != null)
                _activitySystem.AddUser(p);
        }
        void p_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {     
            var p = (Patient)sender;
            System.Threading.Tasks.
                Task.Factory.StartNew(() => _activitySystem.UpdateUser(p));
        }

        void _activitySystem_ConnectionEstablished(object sender, EventArgs e)
        {
            try
            {
                _activityService = new ActivityService(_activitySystem, Net.GetIp(IpType.All), 8000);
                _activityService.ConnectionEstablished += _activityService_ConnectionEstablished;
                _activityService.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        void _activityService_ConnectionEstablished(object sender, EventArgs e)
        {
            StartClient();
        }

        private int _roomCounter;


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
