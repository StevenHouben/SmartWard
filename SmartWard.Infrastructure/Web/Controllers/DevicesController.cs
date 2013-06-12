using System.Collections.Generic;
using System.Linq;
using SmartWard.Infrastructure.ActivityBase;
using SmartWard.Devices;
using SmartWard.Infrastructure.Helpers;

namespace SmartWard.Infrastructure.Web.Controllers
{
    public class DevicesController
    {
        private readonly ActivitySystem _system;

        public DevicesController(ActivitySystem system)
        {
            _system = system;
        }
        public List<IDevice> Get()
        {
            return _system.Devices.Values.ToList();
        }
         public IDevice Get(string id)
        {
            return _system.Devices[id];
        }
        public void Post(IDevice device)
        {
            _system.AddDevice((IDevice)Json.ConvertFromTypedJson(device.ToString()));
        }
        public void Delete(string id)
        {
            _system.RemoveDevice(id);
        }
        public void Put(IDevice device)
        {
            _system.UpdateDevice((IDevice)Json.ConvertFromTypedJson(device.ToString()));
        }
    }
}
