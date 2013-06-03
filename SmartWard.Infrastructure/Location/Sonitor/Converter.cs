using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWard.Infrastructure.Location.Sonitor
{
    #region Converters

    public class SonitorConverter
    {
        public static SonitorMessages DetermineMessage(string raw)
        {
            var dict = new Dictionary<string, SonitorMessages> 
            {
                { "DETECTION", SonitorMessages.DETECTION },
                { "DETECTORS", SonitorMessages.DETECTORS },
                { "DETECTORSTATUS", SonitorMessages.DETECTORSTATUS },
                { "MAPS", SonitorMessages.MAPS},
                { "PROTOCOLVERSION", SonitorMessages.PROTOCOLVERSION},
                { "TAGS", SonitorMessages.TAGS}
            };
            return dict[raw];
        }
        public static bool ConvertToField(int p)
        {
            return p == 1;
        }
        public static ButtonState ConvertToButtonState(int p)
        {
            if (p == 1)
                return ButtonState.Pressed;
            else if (p == 0)
                return ButtonState.NotPressed;
            else
                return ButtonState.Undefined;
        }
        public static BatteryStatus ConvertToBatteryStatus(int p)
        {
            if (p == 0)
                return BatteryStatus.Ok;
            else if (p == -1)
                return BatteryStatus.Undefined;
            else
                return BatteryStatus.Low;
        }
        public static MovingStatus ConvertToMovingStatus(int p)
        {
            if (p == -1)
                return MovingStatus.Undefined;
            else if (p == 1)
                return MovingStatus.Moving;
            else
                return MovingStatus.NonMoving;
        }
    }
    #endregion
}
