using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
            var json = await GetTaskGroupJsonAsync(projectGuid, groupId);

            return JsonConvert.DeserializeObject<TaskGroup41Preview1>(json);
        }

        public static async Task<string> GetTaskGroupJsonAsync(Guid projectGuid, Guid groupId)
        {
            var url = $"distributedtask/taskgroups/{groupId}?api-version=4.1-preview.1";

            var request = new RestRequest(url, Method.GET)
                .AddHeader("Authorization", $"Basic {SettingsService.GetBasicAuthorizationValue()}");

            var client = new RestClient(GetBaseUrl(projectGuid));

            var response = await client.ExecuteTaskAsync(request);

            var token = JToken.Parse(response.Content);

            return token["value"].First.ToString();
        }

        public static async Task<TaskGroup41Preview1> UpdateTaskGroupAsync(Guid projectGuid, TaskGroup41Preview1 group)
        {
            var json = JsonConvert.SerializeObject(group);

            var result = await UpdateTaskGroupJsonAsync(projectGuid, json);

            return JsonConvert.DeserializeObject<TaskGroup41Preview1>(result);
        }

        public static async Task<string> UpdateTaskGroupJsonAsync(Guid projectGuid, string json)
        {
            var token = JToken.Parse(json);

            var groupId = token["id"];

            var url = $"distributedtask/taskgroups/{groupId}?api-version=4.1-preview.1";

            var request = new RestRequest(url, Method.PUT)
                .AddHeader("Authorization", $"Basic {SettingsService.GetBasicAuthorizationValue()}")
                .AddHeader("Content-Type", "application/json")
                .AddParameter("application/json", json, ParameterType.RequestBody);

            var client = new RestClient(GetBaseUrl(projectGuid));

            return (await client.ExecuteTaskAsync(request)).Content;
        }

        public static async Task CloneGroup(Guid sourceProjectGuid, Guid groupId, Guid targetProjectGuid, string clonedName)
        {
            var json = await GetTaskGroupJsonAsync(sourceProjectGuid, groupId);

            var token = JToken.Parse(json);

            token["id"] = Guid.NewGuid();
            token["version"] = "1.0.0";
            token["name"] = clonedName;
            token["friendlyName"] = clonedName;
            token["instanceNameFormat"] = $"Task group: {clonedName}"; ;

            await CreateTaskGroupJsonAsync(targetProjectGuid, token.ToString());
        }

        public static async Task<string> CreateTaskGroupJsonAsync(Guid projectGuid, string json)
        {
            const string url = "distributedtask/taskgroups/?api-version=4.1-preview.1";

            var request = new RestRequest(url, Method.POST)
                .AddHeader("Authorization", $"Basic {SettingsService.GetBasicAuthorizationValue()}")
                .AddHeader("Content-Type", "application/json")
                .AddParameter("application/json", json, ParameterType.RequestBody);

            var client = new RestClient(GetBaseUrl(projectGuid));

            var response = await client.ExecuteTaskAsync(request);

            return response.Content;
        }
    }
}
