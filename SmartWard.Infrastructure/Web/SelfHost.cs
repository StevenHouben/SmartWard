using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin.Hosting;
using Owin;

namespace SmartWard.Infrastructure.Web
{
    public class WebApiService
    {
        public string Address { get; private set; }
        public int Port { get; private set; }
        private static bool Running { get; set; }

        public bool IsRunning 
        { 
            get { return Running; }
        }

        public void Start(string addr,int port)
        {
            if (Running)
                return;
            Running = true;
            Address = addr;
            Port = port;
            Task.Factory.StartNew(() =>
                {

                    using (WebApplication.Start<ActivityWebService>( Helpers.Net.GetUrl(addr, port, "").ToString()))
                    {
                        Console.WriteLine("WebAPI running on {0}", Helpers.Net.GetUrl(addr, port, ""));
                        while(Running){}
                    }
                });

        }

        public void Stop()
        {
            if(Running)
                Running = false;
        }

        public class ActivityWebService
        {
            public void Configuration(IAppBuilder app)
            {
                var config = new HttpConfiguration {DependencyResolver = new ActivitySystemResolver()};
                config.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
                config.Routes.MapHttpRoute("Default", "{controller}/{id}", new { id = RouteParameter.Optional });
                app.UseWebApi(config);
                app.MapHubs();
            }
        }
    }
}