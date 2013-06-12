using Newtonsoft.Json;

namespace SmartWard.Infrastructure.Helpers
{
    public class Json
    {
        public static T ConvertFromTypedJson<T>(string json)
        {
           return (T)JsonConvert.DeserializeObject(json, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects });
        }

        public static string ConvertToTypedJson(object obj)
        {
              return JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.Objects});
        }
    }
}
