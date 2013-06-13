using System;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartWard.Devices;
using SmartWard.Infrastructure.Events;
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

           // Initialize();

            _eventHandler = new Connection(Address);
            _eventHandler.Received += eventHandler_Received;
            _eventHandler.Start().Wait();
            Initialize();
        }

        private void Initialize()
        {
            var acts = GetActivities();

            foreach (var item in acts)
                activities.AddOrUpdate(item.Id, item, (key, oldValue) => item);

            var usrs = GetUsers();
            foreach (var item in usrs)
                users.AddOrUpdate(item.Id, item, (key, oldValue) => item);

            var dvs = GetDevices();
            foreach (var item in dvs)
                devices.AddOrUpdate(item.Id, item, (key, oldValue) => item);


        }
        private void eventHandler_Received(string obj)
        {
            if (obj == "Connected")
            {
                return;
            }
            var content = JsonConvert.DeserializeObject<JObject>(obj);
            var eventType = content["Event"].ToString();
            var data = content["Data"].ToString();

            switch ((NotificationType)Enum.Parse(typeof(NotificationType),eventType))
            {
                case NotificationType.ActivityAdded:
                    OnActivityAdded(new ActivityEventArgs(Json.ConvertFromTypedJson<IActivity>(data)));
                    break;
                case NotificationType.ActivityChanged:
                    OnActivityChanged(new ActivityEventArgs(Json.ConvertFromTypedJson<IActivity>(data)));
                    break;
                case NotificationType.ActivityRemoved:
                    OnActivityRemoved(
                        new ActivityRemovedEventArgs(
                            JsonConvert.DeserializeObject<JObject>(data)["Id"].ToString()));
                    break;
                case NotificationType.UserAdded:
                    OnUserAdded(new UserEventArgs(Json.ConvertFromTypedJson<IUser>(data)));
                    break;
                case NotificationType.UserChanged:
                    OnUserAdded(new UserEventArgs(Json.ConvertFromTypedJson<IUser>(data)));
                    break;
                case NotificationType.UserRemoved:
                    OnActivityRemoved(
                        new ActivityRemovedEventArgs(
                            JsonConvert.DeserializeObject<JObject>(data)["Id"].ToString()));
                    break;
            }
        }

        public override void AddActivity(Model.IActivity activity)
        {
            Rest.Post(Address + Url.Activities, activity);
        }

        public override void AddUser(Users.IUser user)
        {
            Rest.Post(Address + Url.Users, user);
        }

        public override void RemoveUser(string id)
        {
            Rest.Delete(Address + Url.Users, id);
        }

        public override void UpdateUser(Users.IUser user)
        {
            Rest.Post(Address + Url.Users, user);
        }

        public override Users.IUser GetUser(string id)
        {
            return Json.ConvertFromTypedJson<IUser>(Rest.Get(Address + Url.Users, id));
        }

        public override void UpdateActivity(Model.IActivity act)
        {
            Rest.Post(Address + Url.Activities, act);
        }

        public override void RemoveActivity(string id)
        {
            Rest.Delete(Address + Url.Activities, id);
        }

        public override Model.IActivity GetActivity(string id)
        {
            return Json.ConvertFromTypedJson<IActivity>(Rest.Get(Address + Url.Activities, id));
        }
        public override List<IActivity> GetActivities()
        {
            return Json.ConvertArrayFromTypedJson<IActivity>(Rest.Get(Address + Url.Activities, ""));
        }

        public override void AddDevice(Devices.IDevice dev)
        {
            Rest.Post(Address + Url.Devices, dev);
        }

        public override void UpdateDevice(Devices.IDevice dev)
        {
            Rest.Post(Address + Url.Devices, dev);
        }

        public override void RemoveDevice(string id)
        {
            Rest.Delete(Address + Url.Devices, id);
        }

        public override Devices.IDevice GetDevice(string id)
        {
            return Json.ConvertFromTypedJson<IDevice>(Rest.Get(Address + Url.Devices, id));
        }

        public Type NotifierType { get; set; }

        public override List<IUser> GetUsers()
        {
            return Json.ConvertArrayFromTypedJson<IUser>(Rest.Get(Address + Url.Users, ""));
        }

        public override List<IDevice> GetDevices()
        {
            return Json.ConvertArrayFromTypedJson<IDevice>(Rest.Get(Address + Url.Devices, ""));
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
