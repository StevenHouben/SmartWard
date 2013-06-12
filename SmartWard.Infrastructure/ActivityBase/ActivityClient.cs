using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartWard.Infrastructure.Helpers;
using SmartWard.Model;
using SmartWard.Users;

namespace SmartWard.Infrastructure.ActivityBase
{
    public class ActivityClient:ActivityNode
    {
        private readonly Connection _eventHandler;

        private string Address { get; set; }

        public ActivityClient(string ip, int port)
        {
            Ip = ip;
            Port = port;

            Address = Net.GetUrl(ip, port, "").ToString();

            _eventHandler = new Connection(Address);
            _eventHandler.Received += eventHandler_Received;
            _eventHandler.Start().Wait();
        }

        private void eventHandler_Received(string obj)
        {
            if (obj == "Connected")
                return;
            var content = JsonConvert.DeserializeObject<JObject>(obj);
            var eventType = content["Event"].ToString();
            var data = content["Data"].ToString();


            switch (eventType)
            {
                case "ActivityAdded":
                    OnActivityAdded(new ActivityEventArgs(Json.ConvertFromTypedJson<IActivity>(data)));
                    break;
                case "ActivityUpdated":
                    OnActivityChanged(new ActivityEventArgs(Json.ConvertFromTypedJson<IActivity>(data)));
                    break;
                case "ActivityDeleted":
                    OnActivityRemoved(
                        new ActivityRemovedEventArgs(
                            JsonConvert.DeserializeObject<JObject>(data)["Id"].ToString()));
                    break;
                case "UserAdded":
                    OnUserAdded(new UserEventArgs(Json.ConvertFromTypedJson<IUser>(data)));
                    break;
            }
        }

        public override void AddActivity(Model.IActivity activity)
        {
            Rest.Post(Address + Url.Activities, activity);
        }

        public override void AddUser(Users.IUser user)
        {
            Rest.Post(Address + Url.Users+"/", user);
        }

        public override void RemoveUser(string id)
        {
            throw new System.NotImplementedException();
        }

        public override void UpdateUser(Users.IUser user)
        {
            throw new System.NotImplementedException();
        }

        public override Users.IUser GetUser(string id)
        {
            throw new System.NotImplementedException();
        }

        public override void UpdateActivity(Model.IActivity act)
        {
            throw new System.NotImplementedException();
        }

        public override void RemoveActivity(string id)
        {
            throw new System.NotImplementedException();
        }

        public override Model.IActivity GetActivity(string id)
        {
            throw new System.NotImplementedException();
        }

        public override void AddDevice(Devices.IDevice dev)
        {
            throw new System.NotImplementedException();
        }

        public override void UpdateDevice(Devices.IDevice dev)
        {
            throw new System.NotImplementedException();
        }

        public override void RemoveDevice(string id)
        {
            throw new System.NotImplementedException();
        }

        public override Devices.IDevice GetDevice(string id)
        {
            throw new System.NotImplementedException();
        }
    }
    public enum Url
    {
        Activities,
        Devices,
        Subscribers,
        Messages,
        Users,
        Files
    }
}
