 using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using SmartWard.Infrastructure.ActivityBase;
using SmartWard.Model;
using SmartWard.Users;

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
    }
    public class UsersController : ApiController
    {
        private readonly ActivitySystem _system;

        public UsersController(ActivitySystem system)
        {
            _system = system;
        }

        public List<IUser> Get()
        {
            return _system.Users.Values.ToList();
        }
    }
    public class Controller : ApiController
    {
        public string Get()
        {
            return "hello";
        }
    }
}
