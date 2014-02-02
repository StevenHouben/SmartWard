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
    public class NotificationViewModelBase : ViewModelBase
    {
        public WardNode WardNode { get; set; }
        public ObservableCollection<NotificationViewModel> Notifications { get; set; }

        public NotificationViewModelBase(WardNode wardNode)
        {
            WardNode = wardNode;
            Notifications = new ObservableCollection<NotificationViewModel>();
            Notifications.CollectionChanged += Notifications_CollectionChanged;

            WardNode.NotificationAdded += WardNode_NotificationAdded;
            WardNode.NotificationRemoved += WardNode_NotificationRemoved;
            WardNode.NotificationChanged += WardNode_NotificationChanged;

            WardNode.NotificationCollection.ToList().ForEach(n => Notifications.Add(new NotificationViewModel((Notification)n)));
        }

        void Notifications_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var list = e.NewItems;
                foreach (var item in list)
                {
                    var notification = item as NotificationViewModel;
                    if (notification == null) return;
                    notification.NotificationUpdated += NotificationUpdated;
                }
            }
        }

        void WardNode_NotificationAdded(object sender, NooSphere.Model.Notifications.Notification notification)
        {
            Notifications.Add(new NotificationViewModel((Notification)notification));
        }

        void WardNode_NotificationChanged(object sender, NooSphere.Model.Notifications.Notification notification)
        {
            var index = -1;
            //Find notification
            var n = Notifications.FirstOrDefault(nn => nn.Id == notification.Id);
            if (n == null)
                return;

            index = Notifications.IndexOf(n);

            if (index == -1)
                return;

            Notifications[index] = new NotificationViewModel((Notification)notification);
            Notifications[index].NotificationUpdated += NotificationUpdated;
        }
        void WardNode_NotificationRemoved(object sender, NooSphere.Model.Notifications.Notification notification)
        {
            foreach (var n in Notifications.ToList())
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (n.Id == notification.Id)
                        Notifications.Remove(n);
                });
            }
        }

        protected void NotificationUpdated(object sender, EventArgs e)
        {
            WardNode.UpdateNotification((Notification)sender);
        }
    }
}
