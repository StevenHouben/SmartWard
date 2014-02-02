using SmartWard.Infrastructure;
using SmartWard.Models.Notifications;
using SmartWard.PDA.Controllers;
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
    public class WindowViewModel : NotificationViewModelBase
    {
        public ObservableCollection<NotificationViewModel> FilteredNotifications { get; set; }
        public WindowViewModel(WardNode wardNode) : base(wardNode) 
        {
            base.Notifications.CollectionChanged += Notifications_CollectionChanged;
        }

        public void InitializeNotificationList()
        {
            FilteredNotifications = new ObservableCollection<NotificationViewModel>();
            FilteredNotifications.CollectionChanged += (s, e) => OnPropertyChanged("FilteredNotifications");
            Notifications.Where(n => n.Notification.To.Contains(AuthenticationController.User.Id) && !n.Notification.SeenBy.Contains(AuthenticationController.User.Id)).ToList().ForEach(n => FilteredNotifications.Add(n));
        }

        void Notifications_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var list = e.NewItems;
                foreach (var item in list)
                {
                    addNotification(item as NotificationViewModel);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var list = e.OldItems;
                foreach (var item in list)
                {
                    FilteredNotifications.Remove(item as NotificationViewModel);
                }
                
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (var item in e.OldItems)
                {
                    FilteredNotifications.Remove(item as NotificationViewModel);
                }
                foreach (var item in e.NewItems)
                {
                    addNotification(item as NotificationViewModel);
                }
            }
        }

        /// <summary>
        /// Adds a notication to the list, if user logged in is in To list and not in SeenBy list.
        /// </summary>
        /// <param name="notificationViewModel"></param>
        private void addNotification(NotificationViewModel notificationViewModel)
        {
            if (notificationViewModel == null) return;
            if (notificationViewModel.Notification.To.Contains(AuthenticationController.User.Id) && !notificationViewModel.Notification.SeenBy.Contains(AuthenticationController.User.Id))
            {
                notificationViewModel.NotificationUpdated += NotificationUpdated;
                FilteredNotifications.Add(notificationViewModel);
            }
        }
    }
}
