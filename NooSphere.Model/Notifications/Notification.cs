using ABC.Model.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABC.Model.Notifications
{
    public class Notification : Noo, INotification
    {
        public Notification()
		{
            Type = typeof( INotification ).Name;
		}
    }
}
