using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using SmartWard.Devices;
using SmartWard.Infrastructure.ActivityBase;

namespace SmartWard.Infrastructure.Events
{
    /// <summary>
    /// 
    /// </summary>
    public class EventDispatcher : PersistentConnection
    {
        protected override Task OnReceived(IRequest request, string connectionId, string data)
        {
            // Broadcast data to all clients
            return Connection.Broadcast(data);
        }
        protected override Task OnConnected(IRequest request, string connectionId)
        {
            return Connection.Send("null", "Connected");
        }
        protected override Task OnReconnected(IRequest request, string connectionId)
        {
            return Connection.Send("null", "ReConnected");
        }
        protected override Task OnDisconnected(IRequest request, string connectionId)
        {
            return Connection.Send("null", "DisConnected"); ;
        }
    }       
}
