﻿using System;
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


namespace SmartWard.Infrastructure
{
    public class WardNode : INotifyPropertyChanged
    {
        private ActivitySystem _activitySystem;
        private ActivityService _activityService;
        private ActivityClient _client;

        public event EventHandler<Patient> PatientAdded;

        protected void OnPatientAdded(Patient p)
        {
            if (PatientAdded != null)
                PatientAdded(this,p);
        }
        public event EventHandler<Patient> PatientChanged;
        protected void OnPatientChanged(Patient p)
        {
            if (PatientChanged != null)
                PatientChanged(this, p);
        }
            
        public event EventHandler<Patient> PatientRemoved;
        protected void OnPatientRemoved(Patient p)
        {
            if (PatientRemoved != null)
                PatientRemoved(this, p);
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
        public Collection<Patient> Patients { get; set; }

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
                        _activityService.Start(Net.GetIp(IpType.All), 8070);
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

                        _activityService.StartBroadcast(DiscoveryType.Zeroconf, "HyPRBoard", "PIT-Lab");
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

            InitializeData(_client);

        }

        private void InitializeData(ActivityNode controller)
        {
            foreach (var pat in controller.Users.Values.OfType<Patient>())
            {
                Patients.Add(pat);
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

            InitializeData(_activitySystem);

            _activityService = new ActivityService(_activitySystem, Net.GetIp(IpType.All), 8070);
            _activityService.Start();


            _activityService.StartBroadcast(DiscoveryType.Zeroconf,
                "Department-x", "Anonymous Hospital, 4th floor");
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
        }
        

        void node_UserRemoved(object sender, UserRemovedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                for (var i = 0; i < Patients.Count; i++)
                {
                    if (Patients[i].Id == e.Id)
                    {
                        OnPatientRemoved(Patients[i]);
                        Patients.RemoveAt(i);
                        break;
                    }
                }
            });
        }

        void node_UserChanged(object sender, UserEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (var t in Patients.Where(t => t.Id == e.User.Id))
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
                    Patients.Add(patient);

                    OnPatientAdded(patient);
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

        public void AddPatient(Patient p)
        {
            if(_client != null)
                _client.AddUser(p);
            else if(_activitySystem != null)
                _activitySystem.AddUser(p);
        }
        public void UpdatePatient(Patient p)
        {
            if (_client != null)
                _client.UpdateUser(p);
            else if (_activitySystem != null)
                _activitySystem.UpdateUser(p);
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
