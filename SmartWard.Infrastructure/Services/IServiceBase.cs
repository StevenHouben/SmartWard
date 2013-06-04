using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace SmartWard.Infrastructure.Services
{
    [ServiceContract]
    public interface IServiceBase
    {
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "")]
        bool Alive();

        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, UriTemplate = "ServiceDown")]
        void ServiceDown();
    }

    public enum Status
    {
        ServiceDown
    }
}
