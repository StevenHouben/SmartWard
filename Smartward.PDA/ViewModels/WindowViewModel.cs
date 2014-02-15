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

namespace SmartWard.PDA.ViewModels
{
    public class WindowViewModel : NotificationsContainerViewModelBase
    {
        public ObservableCollection<NotificationViewModelBase> FilteredNotifications { get; set; }
        public ObservableCollection<NotificationViewModelBase> FilteredPushNotifications { get; set; }

        public WindowViewModel(WardNode wardNode) : base(wardNode, new List<NotificationType>() {NotificationType.Notification, NotificationType.PushNotification} ) 
        {
            base.Notifications.CollectionChanged += Notifications_CollectionChanged;
            base.PushNotifications.CollectionChanged += PushNotifications_CollectionChanged;
            //Hook up to user changes to track the location of the authenticated user - Don't think we need to worry about deletion.
            wardNode.UserChanged += WardNode_UserChanged;
            wardNode.UserAdded += WardNode_UserAdded;

        }

        private void WardNode_UserAdded(object sender, NooSphere.Model.Users.User e)
        {
            //An authenticated user is only added upon login - autonomous navigation
            if (e.Id == AuthenticationHelper.User.Id)
            {
                //TODO: Check location names
                if (e.Location == "Halls")
                {
                    //TODO: Navigate to rounds view if there are any assigned patients, otherwise activities
                }
                else if (e.Location == "Whiteboard")
                {
                    //TODO: Navigate to activities view
                }
                else
                {
                    //TODO check which patients are at the same location and display a ListBox to select which patient you want if any
                }
            }
        }

        private void WardNode_UserChanged(object sender, NooSphere.Model.Users.User e)
        {
            //An authenticated user is only added upon login - autonomous navigation
            if (e.Id == AuthenticationHelper.User.Id)
            {
                //TODO: Check location names
                if (e.Location == "Halls")
                {
                    //TODO: Ask if the clinician wants to navigate to rounds view if there are any assigned patients - or patients overview
                }
                else if (e.Location == "Whiteboard")
                {
                    //TODO: Ask if the clinician wants to navigate to activities view
                }
                else
                {
                    //TODO Ask if the clinician wants to see a certain person. If yes: Check which patients are at the same location and display a ListBox to select which patient you want if any
                }
            }
        }

        public void InitializeNotificationList()
        {
            FilteredNotifications = new ObservableCollection<NotificationViewModelBase>();
            FilteredNotifications.CollectionChanged += (s, e) => OnPropertyChanged("FilteredNotifications");
            Notifications.Where(n => n.Notification.To.Contains(AuthenticationHelper.User.Id) && !n.Notification.SeenBy.Contains(AuthenticationHelper.User.Id)).ToList().ForEach(n => FilteredNotifications.Add(n));

            FilteredPushNotifications = new ObservableCollection<NotificationViewModelBase>();
            FilteredPushNotifications.CollectionChanged += Push;
            PushNotifications.Where(n => n.Notification.To.Contains(AuthenticationHelper.User.Id) && !n.Notification.SeenBy.Contains(AuthenticationHelper.User.Id)).ToList().ForEach(n => FilteredPushNotifications.Add(n));
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
    }
}
