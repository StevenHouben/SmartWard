using Microsoft.AspNet.SignalR;

namespace SmartWard.Infrastructure.Web
{
    public class DefaultHub : Hub
    {
        public void Send(string message)
        {
            Clients.All.addMessage(message);
        }
    }
}
