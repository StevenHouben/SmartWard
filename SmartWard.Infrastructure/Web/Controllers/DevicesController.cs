using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
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
         public void Post(JObject device)
        {
            _system.AddDevice(Json.ConvertFromTypedJson<IDevice>(device.ToString()));
        }
        public void Delete(string id)
        {
            _system.RemoveDevice(id);
        }
        public void Put(JObject device)
        {
            _system.UpdateDevice(Json.ConvertFromTypedJson<IDevice>(device.ToString()));
        }
    }
}
