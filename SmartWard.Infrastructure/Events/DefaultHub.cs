using System;
using System.Linq;
using Microsoft.AspNet.SignalR;

namespace SmartWard.Infrastructure.Events
{
    public class DefaultHub : Hub
    {
        public void Send(string message)
        {
            Console.WriteLine(message);
            var it = new string(message.Reverse().ToArray());
            Clients.All.broadCastToClients(it);
        }
    }
}
