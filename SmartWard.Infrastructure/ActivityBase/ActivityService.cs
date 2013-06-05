using SmartWard.Infrastructure.PubSub;
using SmartWard.Model;
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

        public ActivityService(string address,string name)
        {
            activitySystem = new ActivitySystem(address, name);
            publisher = new RestPublisher();
        }
        public void AddActivity(Activity act, string deviceId)
        {
            throw new NotImplementedException();
        }

        public void UpdateActivity(Activity act, string deviceId)
        {
            throw new NotImplementedException();
        }

        public void SwitchActivity(string id, string deviceId)
        {
            throw new NotImplementedException();
        }

        public void RemoveActivity(string activityId, string deviceId)
        {
            throw new NotImplementedException();
        }

        public List<Model.Activity> GetActivities()
        {
            throw new NotImplementedException();
        }

        public Model.Activity GetActivity(string id)
        {
            throw new NotImplementedException();
        }

        public Guid Register(Devices.Device device)
        {
            throw new NotImplementedException();
        }

        public void UnRegister(string deviceId)
        {
            throw new NotImplementedException();
        }

        public List<Users.User> GetUsers()
        {
            throw new NotImplementedException();
        }

        public void SendMessage(Services.Message message, string deviceId)
        {
            throw new NotImplementedException();
        }

        public bool Alive()
        {
            throw new NotImplementedException();
        }

        public void ServiceDown()
        {
            throw new NotImplementedException();
        }

        public void AddFile(Files.FileRequest file)
        {
            throw new NotImplementedException();
        }

        public System.IO.Stream GetFile(string activityId, string resourceId)
        {
            throw new NotImplementedException();
        }

        public System.IO.Stream GetTestFile()
        {
            throw new NotImplementedException();
        }

        public void RemoveFile(Model.Resource resource)
        {
            throw new NotImplementedException();
        }

        public void UpdateFile(string activityId, string resourceId, System.IO.Stream stream)
        {
            throw new NotImplementedException();
        }

        public event InitializedHandler Initialized;

        public event ConnectionEstablishedHandler ConnectionEstablished;
    }
}
