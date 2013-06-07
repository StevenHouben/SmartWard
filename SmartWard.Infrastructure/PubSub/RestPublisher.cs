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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartWard.Infrastructure.Helpers;
using SmartWard.Infrastructure.ActivityBase;
using SmartWard.Model;
using SmartWard.Infrastructure.Files;
using SmartWard.Devices;
using SmartWard.Users;
using System.Web;
using System.Web.Routing;

namespace SmartWard.Infrastructure.PubSub
{
    public class RestPublisher:HttpApplication
    {
        public void Application_Start()
        {
            // Register the default hubs route: ~/signalr
            RouteTable.Routes.MapHubs();
        }

        public void Publish(ActivityEvent activityEvent, Activity activity, object source = null, bool sendToSource = false)
        { 
        }
        public void Publish(FileEvent fileEvent, FileRequest file, object source = null, bool sendToSource = false)
        {
        }
        public void Publish(DeviceEvent deviceEvent, Device device, object source = null, bool sendToSource = false)
        {
        }
        public void Publish(UserEvent deviceEvent, User user, object source = null, bool sendToSource = false)
        {
        }
        /// <summary>
        /// Publishes an event to all suscribers
        /// </summary>
        /// <param name="publishUrl"> </param>
        /// <param name="netObject">The object that needs to be published</param>
        /// <param name="source">The source that whishes to publish </param>
        /// <param name="sendToSource">Enables or disable self-publishing to source</param>
        public void Publish(string publishUrl, object netObject, object source = null, bool sendToSource = false)
        {
            //Log.Out("Publisher", string.Format("Published {0}",publishUrl), LogCode.Net);
            var toRemove = new List<string>();

            var devices = Registry.ConnectedClients;
            for (var i = 0; i < devices.Count;i++ )
            {
                var addr = devices.Values.ToList()[i].Device.BaseAddress;
                try
                {
                    Task.Factory.StartNew(
                        delegate
                        {
                                Rest.Post(addr + publishUrl, netObject);
                                Log.Out("Publisher",
                                        string.Format("Published {0} to {1}", publishUrl, addr),
                                        LogCode.Net);
                            });
                }   
                catch (Exception)
                {
                    toRemove.Add(devices.Keys.ToList()[i]);
                }
            }
            foreach (var addr in toRemove)
                Registry.ConnectedClients.Remove(addr);
        }

        /// <summary>
        /// Publish event to one subscriber
        /// </summary>
        /// <param name="publishUrl"></param>
        /// <param name="netObject"></param>
        /// <param name="subscriber"> </param>
        public void PublishToSubscriber(string publishUrl, object netObject,object subscriber)
        {
            Task.Factory.StartNew(
                delegate
                {
                        Rest.Post(subscriber + publishUrl, netObject);
                        Log.Out("Publisher", string.Format("Publishing {0} to {1}", publishUrl, subscriber), LogCode.Net);
                });
        }
    }
}