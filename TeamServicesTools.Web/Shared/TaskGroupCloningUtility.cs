using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Newtonsoft.Json;

namespace TeamServicesTools.Web.Shared
{
    public class TaskGroupCloningUtility
    {
        public static TaskGroup Clone(TaskGroup original)
        {
            return JsonConvert.DeserializeObject<TaskGroup>(JsonConvert.SerializeObject(original));
        }
    }
}
