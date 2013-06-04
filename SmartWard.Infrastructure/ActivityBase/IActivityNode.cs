using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWard.Infrastructure
{
    public interface IActivityNode
    {
        event InitializedHandler Initialized;
        event ConnectionEstablishedHandler ConnectionEstablished;

    }
}
