using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using NooSphere.Infrastructure.ActivityBase;
using NooSphere.Devices;
using NooSphere.Infrastructure.Helpers;

namespace NooSphere.Infrastructure.Web.Controllers
{
    public class DevicesController : ApiController
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
