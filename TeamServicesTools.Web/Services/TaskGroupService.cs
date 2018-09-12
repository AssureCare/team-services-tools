using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using TeamServicesTools.Web.Models.TaskGroups;
using TeamServicesTools.Web.Shared;

namespace TeamServicesTools.Web.Services
{
    public class TaskGroupService
    {
        public static async Task<IEnumerable<TaskGroup>> GetTaskGroupsAsync(Guid projectGuid)
        {
            return (await ClientService.TaskAgentHttpClient.GetTaskGroupsAsync(projectGuid))
                .OrderBy(p => p.Name);
        }

        public static async Task RenameGroup(Guid projectGuid, Guid groupId, string newName)
        {
            // ---------------------------------------------------------------------------
            // Microsoft.TeamFoundation.DistributedTask.Common 15.112.1 does not support 
            // Revision property of TaskGroup, and Revision property must match 
            // what is on the server when updating a task group or you will get a 
            // TaskGroupAlreadyUpdatedException.
            //
            // The Revision property is set by VSTS when updating a task group.
            //
            // JGB 2018-09-05
            // ---------------------------------------------------------------------------

            var group = await GetTaskGroupAsync(projectGuid, groupId);

            // Update name
            group.Name = newName;
            group.FriendlyName = newName;
            group.InstanceNameFormat = $"Task group: {newName}";

            await UpdateTaskGroupAsync(projectGuid, group);
        }

        public static string GetBaseUrl(Guid projectGuid)
        {
            return $"{SettingsService.GetBaseUrl()}/{projectGuid}/_apis/";
        }

        public static async Task<TaskGroup41Preview1> GetTaskGroupAsync(Guid projectGuid, Guid groupId)
        {
            var url = $"distributedtask/taskgroups/{groupId}?api-version=4.1-preview.1";

            var request = new RestRequest(url, Method.GET)
                .AddHeader("Authorization", $"Basic {SettingsService.GetBasicAuthorizationValue()}");

            var client = new RestClient(GetBaseUrl(projectGuid));

            var response = await client.ExecuteTaskAsync(request);

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var groups = JsonConvert.DeserializeObject<TaskGroup41Preview1Collection>(response.Content, settings);

            return groups?
                .Value?
                .FirstOrDefault();
        }

        public static async Task<TaskGroup41Preview1> UpdateTaskGroupAsync(Guid projectGuid, TaskGroup41Preview1 group)
        {
            var url = $"distributedtask/taskgroups/{group.Id}?api-version=4.1-preview.1";

            var request = new RestRequest(url, Method.PUT)
                .AddHeader("Authorization", $"Basic {SettingsService.GetBasicAuthorizationValue()}")
                .AddJsonBody(group);

            var client = new RestClient(GetBaseUrl(projectGuid));

            return (await client.ExecuteTaskAsync<TaskGroup41Preview1>(request)).Data;
        }

        public static async Task CloneGroup(Guid sourceProjectGuid, Guid groupId, Guid targetProjectGuid, string clonedName)
        {
            var group = await GetTaskGroupAsync(sourceProjectGuid, groupId);
            var clonedGroup = CloningUtility.Clone(group);

            clonedGroup.Id = Guid.NewGuid();
            clonedGroup.Version = new TaskVersion("1.0.0");
            clonedGroup.Name = clonedName;
            clonedGroup.FriendlyName = clonedName;
            clonedGroup.InstanceNameFormat = $"Task group: {clonedName}"; ;

            await ClientService.TaskAgentHttpClient.AddTaskGroupAsync(targetProjectGuid, clonedGroup);
        }
    }
}
