using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

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
    }       
}
