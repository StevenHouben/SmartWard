﻿using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace NooSphere.Infrastructure.Events
{
    public class EventDispatcher : PersistentConnection
    {
        protected override Task OnReceived(IRequest request, string connectionId, string data)
        {
            return Connection.Broadcast(data);
        }
        protected override Task OnConnected(IRequest request, string connectionId)
        {
            return Connection.Send(connectionId, "Connected");
        }
        protected override Task OnReconnected(IRequest request, string connectionId)
        {
            return Connection.Send(connectionId, "ReConnected");
        }
        protected override Task OnDisconnected(IRequest request, string connectionId)
        {
            return Connection.Send(connectionId, "DisConnected");
        }
    }       
}
