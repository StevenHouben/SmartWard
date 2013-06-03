using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWard.Infrastructure.Location.Sonitor
{
    public class GenericLocation<T>
    {
        public T X { get; set; }
        public T Y { get; set; }

        public Type GetType()
        {
            return typeof(T);
        }

        public GenericLocation(T x, T y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}
