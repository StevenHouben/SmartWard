using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using ABC.Model.Resources;
using Newtonsoft.Json.Linq;
using ABC.Infrastructure.ActivityBase;
using ABC.Infrastructure.Helpers;
using ABC.Infrastructure.Events;


namespace ABC.Infrastructure.Web.Controllers
{
    public class ResourcesController : ApiController
    {
        readonly ActivitySystem _system;

        public ResourcesController(ActivitySystem system)
        {
            _system = system;
        }

        public List<IResource> Get()
        {
            return _system.Resources.Values.ToList();
        }

        public IResource Get(string id)
        {
            return _system.Resources[id];
        }

        public void Post(JObject resource)
        {
            _system.AddResource(Helpers.Json.ConvertFromTypedJson<IResource>(resource.ToString()));
        }

        public void Delete(string id)
        {
            _system.RemoveUser(id);
        }

        public void Put(JObject resource)
        {
            _system.UpdateResource(Helpers.Json.ConvertFromTypedJson<IResource>(resource.ToString()));
        }
    }
}