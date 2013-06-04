using SmartWard.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace SmartWard.Infrastructure.Services
{
    public class ConnectedClient
    {
        public string Name { get; private set; }
        public string Ip { get; private set; }
        public Device Device { get; set; }
        public ConnectedClient(string name, string ip, Device devi)
        {
            Name = name;
            Ip = ip;
            Device = devi;
        }
        public override string ToString()
        {
            return Ip;
        }
    }

}
