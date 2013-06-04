using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWard.Infrastructure
{
    public class NetEventArgs
    {
        public string Raw { get; set; }
        public NetEventArgs() { }
        public NetEventArgs(string raw)
        {
           Raw = raw;
        }
    }
}
