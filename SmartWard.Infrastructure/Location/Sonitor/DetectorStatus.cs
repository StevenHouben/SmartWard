using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWard.Infrastructure.Location.Sonitor
{
    public class DetectorStatus
    {
        public string HostName { get; set; }
        public int Channel { get; set; }
        public bool Online { get; set; }
    }
}
