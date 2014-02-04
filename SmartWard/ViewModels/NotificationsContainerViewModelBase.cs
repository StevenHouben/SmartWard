using SmartWard.Infrastructure;
using SmartWard.Models.Notifications;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SmartWard.ViewModels
{
    public class NotificationsContainerViewModelBase : ViewModelBase
    {
        public WardNode WardNode { get; set; }
        public ObservableCollection<NotificationViewModelBase> Notifications { get; set; }
        public ObservableCollection<NotificationViewModelBase> PushNotifications { get; set; }

        public NotificationsContainerViewModelBase(WardNode wardNode, List<NotificationType> types)
        {
            WardNode = wardNode;
            foreach (NotificationType type in types)
            {
                switch (type)
                {
                    case NotificationType.Notification:
                        Notifications = new ObservableCollection<NotificationViewModelBase>();
                        Notifications.CollectionChanged += Notifications_CollectionChanged;

                        WardNode.NotificationCollection.Where(n => n.Type == typeof(Notification).Name).ToList().ForEach(n => Notifications.Add(new NotificationViewModelBase((Notification)n)));
                        break;
                    case NotificationType.PushNotification:
                        PushNotifications = new ObservableCollection<NotificationViewModelBase>();
                        PushNotifications.CollectionChanged += Notifications_CollectionChanged;

                        WardNode.NotificationCollection.Where(n => n.Type == typeof(PushNotification).Name).ToList().ForEach(n => PushNotifications.Add(new NotificationViewModelBase((PushNotification)n)));
                        break;
                    default:
                        break;
                }
                
            }

            if (types.Count > 0)
            {
                WardNode.NotificationAdded += WardNode_NotificationAdded;
                WardNode.NotificationRemoved += WardNode_NotificationRemoved;
                WardNode.NotificationChanged += WardNode_NotificationChanged;
            }
            
        }

        void Notifications_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var list = e.NewItems;
                foreach (var item in list)
                {
                    var notification = item as NotificationViewModelBase;
                    if (notification == null) return;
                    notification.NotificationUpdated += NotificationUpdated;
                }
            }
        }

        void WardNode_NotificationAdded(object sender, NooSphere.Model.Notifications.Notification notification)
        {
            switch (notification.Type)
            {
                case "PushNotification":
                    PushNotifications.Add(new NotificationViewModelBase((Notification)notification));
                    break;
                case "Notification":
                    Notifications.Add(new NotificationViewModelBase((Notification)notification));
                    break;
            }
            
        }

        void WardNode_NotificationChanged(object sender, NooSphere.Model.Notifications.Notification notification)
        {
            var index = -1;
            switch (notification.Type)
            {
                case "PushNotification":
                    var pn = PushNotifications.FirstOrDefault(nn => nn.Id == notification.Id);
                    
                    if (pn == null)
                        return;

                    index = Notifications.IndexOf(pn);

                    if (index == -1)
                        return;

                    PushNotifications[index] = new NotificationViewModelBase((Notification)notification);
                    PushNotifications[index].NotificationUpdated += NotificationUpdated;
                    break;
                case "Notification":
                    var n = Notifications.FirstOrDefault(nn => nn.Id == notification.Id);
                    
                    if (n == null)
                        return;
                    
                    index = Notifications.IndexOf(n);

                    if (index == -1)
                        return;

                    Notifications[index] = new NotificationViewModelBase((Notification)notification);
                    Notifications[index].NotificationUpdated += NotificationUpdated;    
                    break;
            }
            
        }
        void WardNode_NotificationRemoved(object sender, NooSphere.Model.Notifications.Notification notification)
        {
            foreach (var n in Notifications.ToList())
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (n.Id == notification.Id)
                    {
                        switch (notification.Type)
                        {
                            case "PushNotification":
                                PushNotifications.Remove(n);
                                break;
                            case "Notification":
                                Notifications.Remove(n);
                                break;
                        }
                    }
                        
                });
            }
        }
        
        protected void NotificationUpdated(object sender, EventArgs e)
        {
            WardNode.UpdateNotification((Notification)sender);
        }

        public enum NotificationType
        {
            Notification,
            PushNotification
        }
    }
}
