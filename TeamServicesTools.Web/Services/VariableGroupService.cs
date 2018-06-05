using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.TeamFoundation.DistributedTask.WebApi;

namespace TeamServicesTools.Web.Services
{
    public class VariableGroupService
    {
        public static async Task<List<VariableGroup>> GetVariableGroupsAsync(Guid projectGuid)
        {
            return await ClientService.TaskAgentHttpClient.GetVariableGroupsAsync(projectGuid);
        }

        public static async Task<VariableGroup> GetVariableGroupAsync(Guid projectGuid, int groupId)
        {
            return await ClientService.TaskAgentHttpClient.GetVariableGroupAsync(projectGuid, groupId);
        }

        public static async Task<List<SelectListItem>> GetVariableGroupsSelectListItemsAsync(Guid projectGuid, int selectedGroupId)
        {
            var items = (await GetVariableGroupsAsync(projectGuid))
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name,
                    Selected = p.Id.Equals(selectedGroupId)
                })
                .OrderBy(i => i.Text)
                .ToList();

            items.Insert(0, new SelectListItem
            {
                Value = "",
                Text = "(select)"
            });

            return items;
        }

        public static async Task RenameGroup(Guid projectGuid, int groupId, string newName)
        {
            var group = await ClientService.TaskAgentHttpClient.GetVariableGroupAsync(projectGuid, groupId);
            group.Name = newName;
            await ClientService.TaskAgentHttpClient.UpdateVariableGroupAsync(projectGuid, groupId, group);
        }

        public static async Task CloneGroup(Guid sourceProjectGuid, int groupId, Guid targetProjectGuid, string clonedName)
        {
            var group = await ClientService.TaskAgentHttpClient.GetVariableGroupAsync(sourceProjectGuid, groupId);
            var clonedGroup = group.Clone();
            clonedGroup.Name = clonedName;
            await ClientService.TaskAgentHttpClient.AddVariableGroupAsync(targetProjectGuid, clonedGroup);
        }

        public static async Task AddVariableValue(Guid projectGuid, int groupId, string key, string value, bool isSecret)
        {
            var group = await ClientService.TaskAgentHttpClient.GetVariableGroupAsync(projectGuid, groupId);
            if (group.Variables.ContainsKey(key)) return;
            group.Variables.Add(new KeyValuePair<string, VariableValue>(key, new VariableValue
            {
                Value = value,
                IsSecret = isSecret
            }));
            await ClientService.TaskAgentHttpClient.UpdateVariableGroupAsync(projectGuid, groupId, group);
        }
    }
}
