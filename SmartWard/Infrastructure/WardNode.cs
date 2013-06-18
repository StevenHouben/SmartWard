using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using NooSphere.Infrastructure.ActivityBase;
using NooSphere.Infrastructure.Helpers;
using NooSphere.Model;
using NooSphere.Primitives;
using SmartWard.Model;
using Action = System.Action;


namespace SmartWard.Infrastructure
{
    public class WardNode : INotifyPropertyChanged
    {
        private ActivitySystem _activitySystem;
        private ActivityService _activityService;
        private ActivityClient _client;

        #region Properties
        public ObservableCollection<IActivity> Activities { get; set; }
        public ObservableCollection<Patient> Patients { get; set; }

        private readonly WardNodeConfiguration _configuration;
        private WebConfiguration _webConfiguration;

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

                        _activitySystem.StartBroadcast(NooSphere.Infrastructure.Discovery.DiscoveryType.Zeroconf, "HyPRBoard", "PIT-Lab");
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
            return new WardNode(WardNodeConfiguration.ClientAndSystem, webConfiguration);
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
                case WardNodeConfiguration.ClientAndSystem:
                    StartClientAndSystem();
                    break;
            }
        }
        private void StartClient()
        {
            //throw new NotImplementedException();
        }
        private void StartClientAndSystem()
        {
            _activitySystem = new ActivitySystem(systemName: "WardNode-"+ Guid.NewGuid());

            _activitySystem.ConnectionEstablished += _activitySystem_ConnectionEstablished;

            _activitySystem.UserAdded += _activitySystem_UserAdded;
            _activitySystem.UserRemoved += _activitySystem_UserRemoved;
            _activitySystem.UserChanged += _activitySystem_UserUpdated;

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
            //_activitySystem.StartBroadcast(NooSphere.Infrastructure.Discovery.DiscoveryType.Zeroconf, "HyPRBoard", "PIT-Lab");

            //_activitySystem.StartLocationTracker();
        }

        public void AddPatient(Patient p)
        {
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
        private void _activitySystem_UserUpdated(object sender, UserEventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action) (() =>
            {
                foreach (var t in Patients.Where(t => t.Id == e.User.Id))
                {
                    t.UpdateAllProperties(e.User);
                    break;
                }
            }));
        }

        private void _activitySystem_UserRemoved(object sender, UserRemovedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action) (() =>
            {
                for (var i = 0; i < Patients.Count; i++)
                {
                    if (Patients[i].Id != e.Id) continue;
                    Patients.RemoveAt(i);
                    break;
                }
            }));
        }
        private int _roomCounter;
        private void _activitySystem_UserAdded(object sender, UserEventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action) (() =>
            {
                var patient = e.User as Patient;

                if (patient != null)
                {
                    patient.PropertyChanged += p_PropertyChanged;
                    Patients.Add((Patient) e.User);
                    
                }
            }));
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
