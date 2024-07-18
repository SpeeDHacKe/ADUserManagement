using Newtonsoft.Json;
using System.Text.Json;

namespace ADUserManagement.Common
{
    public static class Extension
    {
        public static T ConvertToModel<T>(this object dt)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.None,
                FloatFormatHandling = FloatFormatHandling.DefaultValue
            };
            var strDt = JsonConvert.SerializeObject(dt);
            return JsonConvert.DeserializeObject<T>(strDt, settings);
        }
    }
}
