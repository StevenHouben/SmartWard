using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWard.Infrastructure.PubSub
{
    public enum EventType
    {
        ActivityEvents,
        DeviceEvents,
        ComEvents,
        FileEvents,
        UserEvent,
        StatusEvent
    }
}
