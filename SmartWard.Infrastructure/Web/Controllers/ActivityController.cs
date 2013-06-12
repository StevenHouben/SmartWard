 using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using SmartWard.Infrastructure.ActivityBase;
 using SmartWard.Infrastructure.Helpers;
 using SmartWard.Model;
 using SmartWard.Users;
using Newtonsoft.Json.Linq;

namespace SmartWard.Infrastructure.Web.Controllers
{
    public class ActivitiesController : ApiController
    {
        private readonly ActivitySystem _system;


        public ActivitiesController(ActivitySystem system)
        {
            _system = system;
        }

        public List<IActivity> Get()
        {
            return _system.Activities.Values.ToList();
        }
        public IActivity Get(string id)
        {
            return _system.Activities[id];
        }
        public void Post(JObject activity)
        {
            _system.AddActivity(Json.ConvertFromTypedJson<IActivity>(activity.ToString()));
        }
        public void Delete(string id)
        {
            _system.RemoveActivity(id);
        }
        public void Put(JObject activity)
        {
            _system.UpdateActivity(Json.ConvertFromTypedJson<IActivity>(activity.ToString()));
        }
    }
}
