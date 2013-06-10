using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;
using SmartWard.Infrastructure.ActivityBase;
using SmartWard.Infrastructure.Web.Controllers;

namespace SmartWard.Infrastructure.Web
{
    public class ActivitySystemResolver : IDependencyResolver
    {
        public IDependencyScope BeginScope()
        {
            return this;
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(ActivitiesController))
                return new ActivitiesController(ActivitySystem.Instance);
            if (serviceType == typeof (UsersController))
                return new UsersController(ActivitySystem.Instance);
            return null;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return new List<object>();
        }

        public void Dispose()
        {
            // When BeginScope returns 'this', the Dispose method must be a no-op.
        }
    }
}
