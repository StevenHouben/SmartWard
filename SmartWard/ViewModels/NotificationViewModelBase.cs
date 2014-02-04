using NooSphere.Model.Users;
using SmartWard.Models.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWard.ViewModels
{
    public class NotificationViewModelBase : NooViewModelBase
    {       
        public event EventHandler NotificationUpdated;

        public void SeenBy(User user)
        {
            Notification.SetSeenBy(user);
        }

        public NotificationViewModelBase(Notification notification) : base(notification)
        {
            
        }

        public Notification Notification {
            get { return Noo as Notification; }
        }

        public string Id
        {
            get { return Notification.Id; }
            set
            {
                Notification.Id = value;
                OnPropertyChanged("Id");
            }
        }

        public string Message
        {
            get { return Notification.Message; }
            set
            {
                Notification.Message = value;
                OnPropertyChanged("Message");
            }
        }


    }
}
