using NooSphere.Model.Users;
using SmartWard.Models.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWard.ViewModels
{
    public class NotificationViewModel : ViewModelBase
    {
        private readonly Notification _notification;

        public event EventHandler NotificationUpdated;

        public void SeenBy(User user)
        {
            _notification.SetSeenBy(user);
        }

        public NotificationViewModel(Notification notification)
        {
            _notification = notification;
        }

        public Notification Patient{
            get { return _notification; }
        }

        public string Id
        {
            get { return _notification.Id; }
            set
            {
                _notification.Id = value;
                OnPropertyChanged("Id");
            }
        }

        public string Message
        {
            get { return _notification.Message; }
            set
            {
                _notification.Message = value;
                OnPropertyChanged("Message");
            }
        }


    }
}
