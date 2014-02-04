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

        public Notification Notification
        {
            get { return Noo as Notification; }
        }

        public IList<string> To { get { return Notification.To; } set { Notification.To = value; } }
        public IList<string> SeenBy { get { return Notification.SeenBy; } set { Notification.SeenBy = value; } }
        public string ReferenceId { get { return Notification.ReferenceId; } set { Notification.ReferenceId = value; } }
        public string ReferenceType { get { return Notification.ReferenceType; } set { Notification.ReferenceType = value; } }
        public string Message { get { return Notification.Message; } set { Notification.Message = value; } }
        public NotificationViewModelBase(Notification notification)
            : base(notification) { }

    }
}
