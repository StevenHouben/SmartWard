using NooSphere.Infrastructure.Helpers;
using SmartWard.Commands;
using SmartWard.Infrastructure;
using SmartWard.Models;
using SmartWard.PDA.Helpers;
using SmartWard.PDA.Views;
using SmartWard.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Windows.Networking.Proximity;
using Windows.Storage.Streams;

namespace SmartWard.PDA.ViewModels
{
    class AuthenticatedViewModel : ViewModelBase
    {
        private string _data; 
        private bool _NfcDetected;
        private string _nfcId;
        private ProximityDevice _proximityDevice; 
        private long _MessageType;
        private List<Clinician> _users;

        #region Properties
        public WardNode WardNode { get; set; }
        public string NfcId { get; set; }

        #endregion

        private ICommand _loginCommand;

        public ICommand LoginCommand
        {
            get
            {
                return _loginCommand ?? (_loginCommand = new RelayCommand(
                    param => LoginClinician(param),
                    param => true
                    ));
            }
        }

        public AuthenticatedViewModel(WardNode wardNode) 
        {
            _users = new List<Clinician>();
            WardNode = wardNode;
            WardNode.UserCollection.Where(c => c.Type.Equals("Clinician")).ToList().ForEach(c => _users.Add((Clinician)c));

            _proximityDevice = ProximityDevice.GetDefault();
            if (_proximityDevice != null)
            {
                _proximityDevice.DeviceArrived += _proximityDevice_DeviceArrived;
                _proximityDevice.DeviceDeparted += _proximityDevice_DeviceDeparted;
            }

        } 
        
        void _proximityDevice_DeviceDeparted(ProximityDevice sender) 
        { 
            NfcDetected = false; 
        } 
 
        void _proximityDevice_DeviceArrived(ProximityDevice sender) 
        { 
            NfcDetected = true;
            NfcId = Regex.Match(sender.DeviceId, @"{.*}").Value;
            Application.Current.Dispatcher.Invoke(() => LoginClinician(NfcId));
        } 
 
        public bool NfcDetected 
        { 
            get { return _NfcDetected; } 
            set 
            { 
                _NfcDetected = value; 
                OnPropertyChanged("NfcDetected"); 
                OnPropertyChanged("NfcSearching"); 
            } 
        } 
 
        public bool NfcSearching 
        { 
            get { return !_NfcDetected; } 
        } 
 
        public event PropertyChangedEventHandler PropertyChanged; 
 
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) 
        { 
            PropertyChangedEventHandler handler = PropertyChanged; 
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName)); 
        } 

        public void LoginClinician(object nfcId)
        {
            String id = nfcId.ToString();
            Clinician clinician = _users.Where(c => c.NfcId.Equals(nfcId)).FirstOrDefault();
            if (clinician != null)
            {
                AuthenticationHelper.User = clinician;
                Activities activities = new Activities();
                activities.DataContext = new ActivitiesViewModel(WardNode);

                WardNode.SetClientDeviceUser(clinician);
                WardNode.SetClientDeviceTag(clinician.Tag);

                PDAWindow pdaWindow = (PDAWindow)Application.Current.MainWindow;
                // Initialize notification list
                (pdaWindow.DataContext as WindowViewModel).InitializeNotificationList();
                
                pdaWindow.NotificationBar.Visibility = Visibility.Visible;
                pdaWindow.ContentFrame.Navigate(activities);
            }
        }
    }
}
