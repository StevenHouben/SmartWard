﻿using System;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Json;
using Microsoft.Owin.Hosting;
using Newtonsoft.Json;
using Owin;
using SmartWard.Infrastructure.Events;
using SmartWard.Infrastructure.Web.Controllers;

namespace SmartWard.Infrastructure.Web
{
    public class WebApiServer
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

        internal class ActivityWebService
        {
            public void Configuration(IAppBuilder app)
            {
                var config = new HttpConfiguration {DependencyResolver = new ControllerResolver()};
                config.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
                config.Formatters.JsonFormatter.SerializerSettings.TypeNameHandling = TypeNameHandling.Objects;
                config.Routes.MapHttpRoute("Default", "{controller}/{id}", new { id = RouteParameter.Optional });
                app.UseWebApi(config);
                app.MapConnection<EventDispatcher>("", new ConnectionConfiguration {EnableCrossDomain = true});
                app.MapHubs();

                var serializer = new JsonNetSerializer(new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects
                });

                GlobalHost.DependencyResolver.Register(typeof(IJsonSerializer), () => serializer); 
            }
        }
    }
}