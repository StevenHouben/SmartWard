using Newtonsoft.Json;
using SmartWard.Devices;
using SmartWard.Infrastructure.PubSub;
using SmartWard.Model;
using SmartWard.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SmartWard.Infrastructure
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple,IncludeExceptionDetailInFaults = true)]
    public class ActivityService:IActivityService
    {
        private ActivitySystem activitySystem;
        private RestPublisher publisher;

        public event InitializedHandler Initialized = delegate { };
        public event ConnectionEstablishedHandler ConnectionEstablished = delegate { };

        public ActivityService(ActivitySystem system)
        {
            publisher = new RestPublisher();
            Initialized(this, new EventArgs());

            activitySystem = system;
        }
        public void AddActivity(IActivity act, string deviceId)
        {
            activitySystem.AddActivity(act);
        }

        public void UpdateActivity(IActivity act, string deviceId)
        {
            activitySystem.UpdateActivity(act);
        }

        public void RemoveActivity(string activityId, string deviceId)
        {
            activitySystem.RemoveActivity(activityId);
        }

        public List<Activity> GetActivities()
        {
            return activitySystem.Activities.Values.ToList().ConvertAll(o => (Activity)o);
        }

        public Activity GetActivity(string id)
        {
            return (Activity)activitySystem.GetActivity(id);
        }

        public void Register(IDevice device)
        {
            activitySystem.AddDevice(device);
        }
        
        public void UnRegister(string deviceId)
        {
            activitySystem.RemoveDevice(deviceId);
        }

        public List<User> GetUsers()
        {
            List<User> users = activitySystem.Users.Values.ToList().ConvertAll(o => (User)o);
            return users;
        }

        public string HelloWorld()
        {
            string res = JsonConvert.SerializeObject(activitySystem.Users.Values.ToList().ConvertAll(o => (User)o));
            return res;
        }
        public bool Alive()
        {
            return true;
        }
        public List<Activity> GetDefaultActivity()
        {
            List<Activity> acts = new List<Activity>();
            acts.Add(new Activity());
            acts.Add(new Activity());
            return acts;
        }

        public void ServiceDown()
        {
            //publish service going down
        }
    }
}
