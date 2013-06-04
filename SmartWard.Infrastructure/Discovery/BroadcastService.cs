﻿/****************************************************************************
 (c) 2012 Steven Houben(shou@itu.dk) and Søren Nielsen(snielsen@itu.dk)

 Pervasive Interaction Technology Laboratory (pIT lab)
 IT University of Copenhagen

 This library is free software; you can redistribute it and/or 
 modify it under the terms of the GNU GENERAL PUBLIC LICENSE V3 or later, 
 as published by the Free Software Foundation. Check 
 http://www.gnu.org/licenses/gpl.html for details.
****************************************************************************/

using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery;
using Mono.Zeroconf.Providers.Bonjour;
using SmartWard.Infrastructure.Helpers;
using System.Diagnostics;

namespace SmartWard.Infrastructure.Discovery
{
    public class BroadcastService
    {
        #region Private Members
        private ServiceHost discoveryHost;
        private RegisterService service;
        #endregion

        #region Properties
        /// <summary>
        /// Local callback address
        /// </summary>
        /// <remarks>
        /// Composed of IP and Port
        /// </remarks>
        public string Address { get; set; }

        /// <summary>
        /// Client IP
        /// </summary>
        private string Ip { get; set; }

        /// <summary>
        /// Local callback port
        /// </summary>
        private int Port { get; set; }

        /// <summary>
        /// Indicates if the service is running
        /// </summary>
        public bool IsRunning { get; set; }

        /// <summary>
        /// Type of discovery
        /// </summary>
        public DiscoveryType DiscoveryType { get; set; }
        #endregion

        #region Constructor
        public BroadcastService()
        {
            IsRunning = false;
        }
        #endregion

        #region Public Members

        /// <summary>
        /// Start new broadcast service
        /// </summary>
        /// <param name="type">Type of discovery</param>
        /// <param name="nameToBroadcast">The name of the service that needs to be broadcasted</param>
        /// <param name="physicalLocation">The physical location of the service that needs to be broadcasted</param>
        /// <param name="addressToBroadcast">The address of the service that needs to be broadcasted</param>
        /// <param name="broadcastPort">The port of the broadcast service. Default=56789</param>
        public void Start(DiscoveryType type,string nameToBroadcast,string physicalLocation,string code,Uri addressToBroadcast,int broadcastPort=7892)
        {
            DiscoveryType = type;

            switch (DiscoveryType)
            {
                case DiscoveryType.WSDiscovery:
                    {
                        Ip = Net.GetIp(IPType.All);
                        Port = broadcastPort;
                        Address = "http://" + Ip + ":" + Port + "/";

                        discoveryHost = new ServiceHost(new DiscoveyService());

                        var serviceEndpoint = discoveryHost.AddServiceEndpoint(typeof(IDiscovery), new WebHttpBinding(), 
                                                                                Net.GetUrl(Ip, Port, ""));
                        serviceEndpoint.Behaviors.Add(new WebHttpBehavior());

                        var broadcaster = new EndpointDiscoveryBehavior();

                        broadcaster.Extensions.Add(nameToBroadcast.ToXElement<string>());
                        broadcaster.Extensions.Add(physicalLocation.ToXElement<string>());
                        broadcaster.Extensions.Add(addressToBroadcast.ToString().ToXElement<string>());
                        broadcaster.Extensions.Add(code.ToXElement<string>());

                        serviceEndpoint.Behaviors.Add(broadcaster);
                        discoveryHost.Description.Behaviors.Add(new ServiceDiscoveryBehavior());
                        discoveryHost.Description.Endpoints.Add(new UdpDiscoveryEndpoint());
                        discoveryHost.Open();

                        IsRunning = true;
                        Debug.WriteLine(DiscoveryType.ToString() + " is started");
                    }
                    break;
                case DiscoveryType.Zeroconf:
                    {
                       service = new RegisterService
                                          {Name = nameToBroadcast, RegType = "_am._tcp", ReplyDomain = "local", Port = 3689};

 
                        // TxtRecords are optional
                        var txtRecord = new TxtRecord(){
                                                {"name", nameToBroadcast},
                                                {"addr", addressToBroadcast.ToString()},
                                                {"loc", physicalLocation},
                                                {"code", code}
                                            };
                        service.TxtRecord = txtRecord;
                        service.Response += service_Response;
                        service.Register();
                        Debug.WriteLine(DiscoveryType.ToString() + " is started");

                    }
                    break;
            }
        }

        void service_Response(object o, Mono.Zeroconf.RegisterServiceEventArgs args)
        {
            IsRunning = args.IsRegistered;
            if (!IsRunning)
                throw new Exception(args.Service.Name + " not registered");
        }

        /// <summary>
        /// Stops the broadcast service
        /// </summary>
        public void Stop()
        {
            if (DiscoveryType != DiscoveryType.WSDiscovery)
                if (service != null)
                    service.Dispose();
            else
                if(discoveryHost!=null)
                    discoveryHost.Close();

            IsRunning = false;
            Debug.WriteLine(DiscoveryType.ToString() + " is stopped");
        }
        #endregion
    }
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single, IncludeExceptionDetailInFaults = true)]
    public class DiscoveyService : IDiscovery
    {
        public bool Alive()
        { return true; }
        public void  ServiceDown()
        {
        }
    }
}
