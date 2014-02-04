using NooSphere.Model.Device;
using NooSphere.Model.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWard.ViewModels
{
    public class DeviceViewModelBase : NooViewModelBase
    {
        public DeviceViewModelBase(Device device) : base(device) { }

        public Device Device { get { return Noo as Device;  } }
        public DeviceType DeviceType { get { return Device.DeviceType; } set { Device.DeviceType = value; OnPropertyChanged("DeviceType"); } }
        public DeviceRole DeviceRole { get { return Device.DeviceRole; } set { Device.DeviceRole = value; OnPropertyChanged("DeviceRole"); } }
        public DevicePortability DevicePortability { get { return Device.DevicePortability; } set { Device.DevicePortability = value; OnPropertyChanged("DevicePortability"); } }
        public string TagValue { get { return Device.TagValue; } set { Device.TagValue = value; OnPropertyChanged("TagValue"); } }

        public string Location { get { return Device.Location; } set { Device.Location = value; OnPropertyChanged("Location"); } }
        public string BaseAddress { get { return Device.BaseAddress; } set { Device.BaseAddress = value; OnPropertyChanged("BaseAddress"); } }
        public string ConnectionId { get { return Device.ConnectionId; } set { Device.ConnectionId = value; OnPropertyChanged("ConnectionId"); } }

        public IUser Owner { get { return Device.Owner; } set { Device.Owner = value; OnPropertyChanged("Owner"); } }

        public List<IUser> Users { get { return Device.Users; } set { Device.Users = value; OnPropertyChanged("Users"); } } 
    }
}
