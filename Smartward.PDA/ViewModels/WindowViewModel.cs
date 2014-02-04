﻿using SmartWard.Infrastructure;
using SmartWard.Models.Notifications;
using SmartWard.PDA.Helpers;
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
                        default:
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
                FilteredNotifications.Add(notificationViewModel);
            }
        }
    }
}
