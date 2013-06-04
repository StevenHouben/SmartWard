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
    public interface IActivityService : IServiceBase, IFileServer,IActivityNode
    {
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json, UriTemplate = "activities", Method = "POST")]
        void AddActivity(Activity act, string deviceId);

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json, UriTemplate = "activities", Method = "PUT")]
        void UpdateActivity(Activity act, string deviceId);

        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json, UriTemplate = "activities/{id}", Method = "POST")]
        void SwitchActivity(string id, string deviceId);

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json, UriTemplate = "activities", Method = "DELETE")]
        void RemoveActivity(string activityId, string deviceId);

        [OperationContract]
        [ServiceKnownType(typeof(string))]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "activities")]
        List<Activity> GetActivities();

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "activities/{id}")]
        Activity GetActivity(string id);

        [OperationContract]
        [ServiceKnownType(typeof(string))]
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "devices", Method = "POST")]
        Guid Register(Device device);

        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json, UriTemplate = "devices/{deviceId}", Method = "DELETE")]
        void UnRegister(string deviceId);

        [OperationContract]
        [ServiceKnownType(typeof(string))]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "users")]
        List<User> GetUsers();

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "messages", Method = "POST")]
        void SendMessage(Message message, string deviceId);
    }
}
