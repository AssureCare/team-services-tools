using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.DistributedTask.WebApi;

namespace TeamServicesTools.Web.Services
{
    public class TaskGroupService
    {
        public static async Task<IEnumerable<TaskGroup>> GetTaskGroupsAsync(Guid projectGuid)
        {
            return (await ClientService.TaskAgentHttpClient.GetTaskGroupsAsync(projectGuid))
                .OrderBy(p => p.Name);
        }
    }
}
