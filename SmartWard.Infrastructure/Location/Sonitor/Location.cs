using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWard.Infrastructure.Location.Sonitor
{
    public class Location
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Location(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}
