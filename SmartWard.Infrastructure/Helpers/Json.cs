using Newtonsoft.Json;

namespace SmartWard.Infrastructure.Helpers
{
    public class Json
    {
        public static object ConvertFromTypedJson(string json)
        {
           return JsonConvert.DeserializeObject(json, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects });
        }
    }
}
