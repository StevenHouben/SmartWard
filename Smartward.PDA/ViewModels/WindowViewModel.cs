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
using System.Windows.Navigation;
using System.Windows.Threading;

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
        private DispatcherTimer _dispatcherTimer;
        private NavigateToEnum NavigateTo { get; set; }

        private bool _navigateToVisible;
        public bool NavigateToVisible { 
            get { return _navigateToVisible; } 
            set 
            { 
                _navigateToVisible = value; 
                OnPropertyChanged("NavigateToVisible");
                if (value)
                {
                    NavigateToPatientsVisible = false;
                   _dispatcherTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(10) };
                   _dispatcherTimer.Tick += (sender, args) =>
                   {
                       var timer = sender as DispatcherTimer;
                       if (timer != null)
                       {
                           timer.Stop();
                       }
                       Application.Current.Dispatcher.Invoke(() => NavigateToVisible = false);
                   };
                   _dispatcherTimer.Start();
                }
                else
                {
                    if (_dispatcherTimer != null) _dispatcherTimer.Stop();
                }
            } 
        }

        private string _navigateToString;
        public string NavigateToString { get { return "Navigate to " + _navigateToString + "?"; } set { _navigateToString = value; OnPropertyChanged("NavigateToString"); } }

        private bool _navigateToPatientsVisible;
        public bool NavigateToPatientsVisible { 
            get { return _navigateToPatientsVisible; } 
            set { 
                _navigateToPatientsVisible = value; 
                OnPropertyChanged("NavigateToPatientsVisible");
                if (value) {
                    NavigateToVisible = false;
                    _dispatcherTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(10) };
                    _dispatcherTimer.Tick += (sender, args) =>
                    {
                        var timer = sender as DispatcherTimer;
                        if (timer != null)
                        {
                            timer.Stop();
                        }
                        Application.Current.Dispatcher.Invoke(() => NavigateToPatientsVisible = false);
                    };
                    _dispatcherTimer.Start();
                }
                else
                {
                    if (_dispatcherTimer != null) _dispatcherTimer.Stop();
                }
            } 
        }
        public ObservableCollection<PatientsLayoutViewModel> NavigateToPatients { get; set; }
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
                    NavigateToVisible = false;
                    ((PDAWindow)Application.Current.MainWindow).ContentFrame.Navigate(new Activities() { DataContext = new ActivitiesViewModel(WardNode) });
                    break;
                case NavigateToEnum.Patients:
                    NavigateToVisible = false;
                    ((PDAWindow)Application.Current.MainWindow).ContentFrame.Navigate(new Patients() { DataContext = new PatientsViewModel(new List<String>(), WardNode) });
                    break;
                case NavigateToEnum.Round:
                    NavigateToVisible = false;
                    var round = WardNode.ActivityCollection.First(a => a.Type == typeof(RoundActivity).Name && ((RoundActivity)a).ClinicianId == AuthenticationHelper.User.Id && !((RoundActivity)a).IsFinished) as RoundActivity;
                    ((PDAWindow)Application.Current.MainWindow).ContentFrame.Navigate(new Patients() { DataContext = new PatientsViewModel(round.GetPatientIds(), WardNode) });
                    break;
            }
        }

        public WindowViewModel(WardNode wardNode) : base(wardNode, new List<NotificationType>() {NotificationType.Notification, NotificationType.PushNotification} ) 
        {
            NavigateToPatients = new ObservableCollection<PatientsLayoutViewModel>();
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
            if (AuthenticationHelper.User == null || d.Location == null ) return;
            if (d.Id == WardNode.GetClientDevice().Id)
            {
                if (WardNode.GetClientDevice().Location != d.Location)
                {
                    WardNode.GetClientDevice().Location = d.Location;
                    switch (d.Location)
                    {
                        case "Halls":
                            var round = WardNode.ActivityCollection.FirstOrDefault(a => a.Type == typeof(RoundActivity).Name && ((RoundActivity)a).ClinicianId == AuthenticationHelper.User.Id && !((RoundActivity)a).IsFinished) as RoundActivity;
                            if (round != null) //There is an unfinished round
                            {
                                if (((PDAWindow)Application.Current.MainWindow).ContentFrame.NavigationService.Content is Patients) //User is already on a patients-view
                                {
                                    //Check whether it's a filtered or an unfiltered patients-view and navigate only if it's unfiltered according to the round activity
                                    var patients = ((PDAWindow)Application.Current.MainWindow).ContentFrame.NavigationService.Content as Patients;
                                    var vm = patients.DataContext as PatientsViewModel;
                                    var totalPatientCount = WardNode.UserCollection.Where(u => u.Type == typeof(Patient).Name).Count();
                                    if ((vm.Patients.Count == totalPatientCount && round.Visits.Count != totalPatientCount)) //The patients-view is not filtered according to the round (the viewmodel contains the total amount of patients and the round doesn't)
                                    {
                                        NavigateTo = NavigateToEnum.Round;
                                        NavigateToString = "rounds view";
                                        NavigateToVisible = true;
                                    }
                                }
                                else //User is not on a patients-view
                                {
                                    NavigateTo = NavigateToEnum.Round;
                                    NavigateToString = "rounds view";
                                    NavigateToVisible = true;
                                }
                            }
                            else //There's no unfinished round
                            {
                                if (!(((PDAWindow)Application.Current.MainWindow).ContentFrame.NavigationService.Content is Patients)) //User is not on a patients-view
                                {
                                    NavigateTo = NavigateToEnum.Patients;
                                    NavigateToString = "patients overview";
                                    NavigateToVisible = true;
                                }
                            }
                            break;
                        case "Whiteboard":
                            if (!(((PDAWindow)Application.Current.MainWindow).ContentFrame.NavigationService.Content is Activities)) //User is not on a activities-view
                            {
                                NavigateTo = NavigateToEnum.Activities;
                                NavigateToString = "activities overview";
                                NavigateToVisible = true;
                            }
                            break;
                        case "Room": //In a patient room
                            NavigateToPatients.Clear();
                            WardNode.UserCollection.Where(u => u.Type == typeof(Patient).Name && u.Location == d.Location).ToList().ForEach(p => NavigateToPatients.Add(new PatientsLayoutViewModel(p as Patient, WardNode)));
                            if (((PDAWindow)Application.Current.MainWindow).ContentFrame.NavigationService.Content is PatientView) //User is on a patient-view
                            {
                                //Remove the patient of the current view from the navigation list.
                                var patientView = ((PDAWindow)Application.Current.MainWindow).ContentFrame.NavigationService.Content as PatientView;
                                var vm = patientView.DataContext as PatientsLayoutViewModel;
                                var patient = NavigateToPatients.FirstOrDefault(p => p.Id == vm.Id);
                                if (patient != null) NavigateToPatients.Remove(patient);
                            }
                            if (NavigateToPatients.Count > 0)
                            {
                                NavigateTo = NavigateToEnum.Patient;
                                NavigateToPatientsVisible = true;
                            }                            
                            break;
                        default:
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
                            Patient p = (Patient) WardNode.UserCollection.FirstOrDefault(u => u.Id == item.ReferenceId);
                            WardNode.RemoveNotification(item.Id);
                            ((PDAWindow)Application.Current.MainWindow).ContentFrame.Navigate(new PatientView() { DataContext = new PatientsLayoutViewModel(p, WardNode) });
                            break;
                        case "EWS":
                            EWS ews = (EWS)WardNode.ResourceCollection.FirstOrDefault(r => r.Id == item.ReferenceId);
                            WardNode.RemoveNotification(item.Id);
                            ((PDAWindow)Application.Current.MainWindow).ContentFrame.Navigate(new EWSView() { DataContext = new UpdatableEWSViewModel(ews, WardNode) });
                            break;
                        case "Note":
                            Note n = (Note)WardNode.ResourceCollection.FirstOrDefault(r => r.Id == item.ReferenceId);
                            WardNode.RemoveNotification(item.Id);
                            ((PDAWindow)Application.Current.MainWindow).ContentFrame.Navigate(new NoteView() { DataContext = new UpdatableNoteViewModel(n, WardNode) });
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
