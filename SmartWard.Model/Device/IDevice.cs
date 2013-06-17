using SmartWard.Primitives;

namespace SmartWard.Devices
{
    public interface IDevice:INoo
    {
         DeviceType DeviceType { get; set; }
         DeviceRole DeviceRole { get; set; }
         DevicePortability DevicePortability { get; set; }

         string Location { get; set; }
         string BaseAddress { get; set; }
         string ConnectionId { get; set; }
    }
}
