using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWard.Infrastructure.Location.Sonitor
{
    public class Detector
    {
        public string HostName { get; set; }
        public int Channel { get; set; }
        public string Name { get; set; }
        public Location Location { get; set; }
        public int FloorPlan { get; set; }
        public float Radius { get; set; }
        public OperationStatus Status { get; set; }
    }
}
