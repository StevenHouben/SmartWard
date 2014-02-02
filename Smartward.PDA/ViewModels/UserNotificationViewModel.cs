using SmartWard.Infrastructure;
using SmartWard.Models.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWard.PDA.ViewModels
{
    public class UserNotificationViewModel : SmartWard.ViewModels.NotificationViewModel
    {

        public WardNode WardNode { get; set; }

        public UserNotificationViewModel(Notification n, WardNode wardNode) : base(n)
        {
            WardNode = wardNode;
        }
    }
}
