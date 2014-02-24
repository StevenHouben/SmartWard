using NooSphere.Model.Device;
using SmartWard.Commands;
using SmartWard.Infrastructure;
using SmartWard.Models;
using SmartWard.Models.Notifications;
using SmartWard.PDA.Helpers;
using SmartWard.PDA.Views;
using SmartWard.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SmartWard.PDA.ViewModels
{
    public class WindowViewModel : NotificationsContainerViewModelBase
    {
        #region Properties
        public ObservableCollection<NotificationViewModelBase> FilteredNotifications { get; set; }
        public ObservableCollection<NotificationViewModelBase> FilteredPushNotifications { get; set; }
        public bool NewNotifications
        {
            get { return FilteredNotifications.Count > 0; }
        }
        public bool NoNewNotifications
        {
            get { return FilteredNotifications.Count == 0; }
        }
        public string NotificationCount
        {
            get
            {
                if (FilteredNotifications.Count > 0)
                    return FilteredNotifications.Count.ToString();
                else
                    return "";
            }
        }
        private NavigateToEnum NavigateTo { get; set; }

        private bool _navigateToVisible;
        public bool NavigateToVisible { get { return _navigateToVisible; } set { _navigateToVisible = value; OnPropertyChanged("NavigateToVisible"); } }

        private string _navigateToString;
        public string NavigateToString { get { return "Navigate to " + _navigateToString + "?"; } set { _navigateToString = value; OnPropertyChanged("NavigateToString"); } }

        private bool _navigateToPatientsVisible;
        public bool NavigateToPatientsVisible { get { return _navigateToPatientsVisible; } set { _navigateToPatientsVisible = value; OnPropertyChanged("NavigateToPatientsVisible"); } }
        public ObservableCollection<PatientViewModelBase> NavigateToPatients { get; set; }
        #endregion
        private ICommand _navigateToCommand;
        public ICommand NavigateToCommand
        {
            get
            {
                return _navigateToCommand ?? (_navigateToCommand = new RelayCommand(
                    param => NavigateToView(),
                    param => true
                    ));
            }
        }
        private void NavigateToView()
        {
            switch (NavigateTo)
            {
                case NavigateToEnum.Activities:
                    ((PDAWindow)Application.Current.MainWindow).ContentFrame.Navigate(new Activities() { DataContext = new ActivitiesViewModel(WardNode) });
                    break;
                case NavigateToEnum.Patients:
                    ((PDAWindow)Application.Current.MainWindow).ContentFrame.Navigate(new Patients() { DataContext = new PatientsViewModel(new List<String>(), WardNode) });
                    break;
                case NavigateToEnum.Round:
                    var round = WardNode.ActivityCollection.First(a => a.Type == typeof(RoundActivity).Name && ((RoundActivity)a).ClinicianId == AuthenticationHelper.User.Id && !((RoundActivity)a).IsFinished) as RoundActivity;
                    ((PDAWindow)Application.Current.MainWindow).ContentFrame.Navigate(new Patients() { DataContext = new PatientsViewModel(round.GetPatientIds(), WardNode) });
                    break;
            }
        }

        public WindowViewModel(WardNode wardNode) : base(wardNode, new List<NotificationType>() {NotificationType.Notification, NotificationType.PushNotification} ) 
        {
            NavigateToPatients = new ObservableCollection<PatientViewModelBase>();
            base.Notifications.CollectionChanged += Notifications_CollectionChanged;
            base.PushNotifications.CollectionChanged += PushNotifications_CollectionChanged;
            //Hook up to device changes
            WardNode.DeviceAdded += HandleCurrentDeviceLocationChange;
            WardNode.DeviceChanged += HandleCurrentDeviceLocationChange;
        }

        #region Current device location tracking
        private void HandleCurrentDeviceLocationChange(object sender, Device d)
        {
            //Only do something if a user is logged in
            if (AuthenticationHelper.User == null) return;
            if (d.Id == WardNode.GetClientDevice().Id)
            {
                if (WardNode.GetClientDevice().Location != d.Location)
                {
                    WardNode.GetClientDevice().Location = d.Location;
                    switch (d.Location)
                    {
                        case "Halls":
                            var round = WardNode.ActivityCollection.FirstOrDefault(a => a.Type == typeof(RoundActivity).Name && ((RoundActivity)a).ClinicianId == AuthenticationHelper.User.Id && !((RoundActivity)a).IsFinished) as RoundActivity;
                            if (round != null)
                            {
                                NavigateTo = NavigateToEnum.Round;
                                NavigateToString = "rounds view";
                            }
                            else
                            {
                                NavigateTo = NavigateToEnum.Round;
                                NavigateToString = "patients overview";
                            }
                            NavigateToVisible = true;
                            break;
                        case "Whiteboard":
                            NavigateTo = NavigateToEnum.Activities;
                            NavigateToString = "activities overview";
                            NavigateToVisible = true;
                            break;
                        default: //In a patient room
                            NavigateToPatients.Clear();
                            WardNode.UserCollection.Where(u => u.Type == typeof(Patient).Name && u.Location == d.Location).ToList().ForEach(p => NavigateToPatients.Add(new PatientViewModelBase(p as Patient)));
                            if (NavigateToPatients.Count > 0)
                            {
                                NavigateTo = NavigateToEnum.Patient;
                                NavigateToPatientsVisible = true;
                            }                            
                            break;
                    }
                }
            }
        }
        private enum NavigateToEnum
        {
            Activities,
            Patients,
            Round,
            Patient
        }
        #endregion

        #region Notifications
        public void InitializeNotificationList()
        {
            FilteredNotifications = new ObservableCollection<NotificationViewModelBase>();
            FilteredNotifications.CollectionChanged += (s, e) => OnPropertyChanged("FilteredNotifications");
            Notifications.Where(n => n.Notification.To.Contains(AuthenticationHelper.User.Id) && !n.Notification.SeenBy.Contains(AuthenticationHelper.User.Id)).ToList().ForEach(n => FilteredNotifications.Add(n));

            FilteredPushNotifications = new ObservableCollection<NotificationViewModelBase>();
            FilteredPushNotifications.CollectionChanged += Push;
            PushNotifications.Where(n => n.Notification.To.Contains(AuthenticationHelper.User.Id) && !n.Notification.SeenBy.Contains(AuthenticationHelper.User.Id)).ToList().ForEach(n => FilteredPushNotifications.Add(n));
        }
        
        void Notifications_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var list = e.NewItems;
                foreach (var item in list)
                {
                    addNotification(item as NotificationViewModelBase);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var list = e.OldItems;
                foreach (var item in list)
                {
                    FilteredNotifications.Remove(item as NotificationViewModelBase);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (var item in e.OldItems)
                {
                    FilteredNotifications.Remove(item as NotificationViewModelBase);
                }
                foreach (var item in e.NewItems)
                {
                    addNotification(item as NotificationViewModelBase);
                }
            }
            OnPropertyChanged("NewNotifications");
            OnPropertyChanged("NoNewNotifications");
            OnPropertyChanged("NotificationCount");
        }

        void PushNotifications_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var list = e.NewItems;
                foreach (var item in list)
                {
                    addNotification(item as NotificationViewModelBase);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var list = e.OldItems;
                foreach (var item in list)
                {
                    FilteredPushNotifications.Remove(item as NotificationViewModelBase);
                }

            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (var item in e.OldItems)
                {
                    FilteredPushNotifications.Remove(item as NotificationViewModelBase);
                }
                foreach (var item in e.NewItems)
                {
                    addNotification(item as NotificationViewModelBase);
                }
            }
        }

        public void Push(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var list = e.NewItems;
                foreach (NotificationViewModelBase item in list)
                {
                    switch (item.Notification.ReferenceType)
                    {
                        case "Patient":
                            Patient p = (Patient) WardNode.UserCollection.Where(u => u.Id == item.ReferenceId).ToList().FirstOrDefault();
                            WardNode.RemoveNotification(item.Id);
                            ((PDAWindow)Application.Current.MainWindow).ContentFrame.Navigate(new PatientView() { DataContext = new PatientsLayoutViewModel(p, WardNode) });
                            break;
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var list = e.OldItems;
                foreach (var item in list)
                {
                    FilteredPushNotifications.Remove(item as NotificationViewModelBase);
                }
                
            }
        }

        /// <summary>
        /// Adds a notication to the list, if user logged in is in To list and not in SeenBy list.
        /// </summary>
        /// <param name="notificationViewModel"></param>
        private void addNotification(NotificationViewModelBase notificationViewModel)
        {
            if (notificationViewModel == null) return;
            if (notificationViewModel.Notification.To.Contains(AuthenticationHelper.User.Id) && !notificationViewModel.Notification.SeenBy.Contains(AuthenticationHelper.User.Id))
            {
                notificationViewModel.NotificationUpdated += NotificationUpdated;
                switch (notificationViewModel.Notification.Type)
                {
                    case "PushNotification":
                        FilteredPushNotifications.Add(notificationViewModel);
                        break;
                    case "Notification":
                        FilteredNotifications.Add(notificationViewModel);
                        break;
                }
                
            }
        }
        #endregion
    }
}
