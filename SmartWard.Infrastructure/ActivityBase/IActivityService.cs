using SmartWard.Devices;
using SmartWard.Infrastructure.Files;
using SmartWard.Infrastructure.Services;
using SmartWard.Model;
using SmartWard.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace SmartWard.Infrastructure
{
    [ServiceContract]
    public interface IActivityService : IServiceBase,IActivityNode
    {
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json, UriTemplate = "activities", Method = "POST")]
        void AddActivity(IActivity act, string deviceId);

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json, UriTemplate = "activities", Method = "PUT")]
        void UpdateActivity(IActivity act, string deviceId);

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json, UriTemplate = "activities", Method = "DELETE")]
        void RemoveActivity(string activityId, string deviceId);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "activities")]
        List<Activity> GetActivities();

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "activities/{id}")]
        Activity GetActivity(string id);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "activities/hello")]
        List<Activity> GetDefaultActivity();

        [OperationContract]
        [ServiceKnownType(typeof(string))]
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "devices", Method = "POST")]
        void Register(IDevice device);

        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json, UriTemplate = "devices/{deviceId}", Method = "DELETE")]
        void UnRegister(string deviceId);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "users")]
        List<User> GetUsers();

        [OperationContract]
        [ServiceKnownType(typeof(string))]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "helloworld")]
        string HelloWorld();
    }
}
