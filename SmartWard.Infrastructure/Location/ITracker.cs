using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWard.Infrastructure.Location
{
    public interface ITracker
    {
        void Start();
        void Stop();
    }
}
