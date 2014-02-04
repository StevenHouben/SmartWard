using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWard.Models.Notifications
{
    public class PushNotification : Notification
    {

        public PushNotification(List<string> to, string referenceId, string referenceType, string message) : base(to, referenceId, referenceType, message) 
        {
            Type = typeof(PushNotification).Name;
        }

    }
}
