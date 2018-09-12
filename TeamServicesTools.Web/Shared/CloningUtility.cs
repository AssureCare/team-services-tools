using Newtonsoft.Json;

namespace TeamServicesTools.Web.Shared
{
    public class CloningUtility
    {
        public static T Clone<T>(T original)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(original));
        }
    }
}
