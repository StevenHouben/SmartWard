using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWard.Infrastructure.Location.Sonitor
{
    public enum ButtonState : int { Pressed = 1, NotPressed = 0, Undefined = -1 }
    public enum OperationStatus { Offline, Online }
    public enum MovingStatus : int { Moving = 1, NonMoving, Undefined = -1 }
    public enum BatteryStatus : int { Ok = 0, Undefined = -1, Low }
}
