using ABC.Infrastructure.Helpers;

namespace SmartWard.Infrastructure
{
    public class WebConfiguration
    {
        public int Port { get; set; }
        public string Address { get; set; }

        public WebConfiguration()
        {
        }

        public WebConfiguration(string address, int port)
        {
            Address = address;
            Port = port;
        }
        public static WebConfiguration DefaultWebConfiguration = new WebConfiguration
            {
                Address = Net.GetIp(IpType.All),
                Port = 8080
            };
        public static WebConfiguration LocalWebConfiguration = new WebConfiguration
        {
            Address = "127.0.0.1",
            Port = 8080
        };
    }
}
