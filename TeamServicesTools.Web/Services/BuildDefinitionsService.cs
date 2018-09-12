using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.TeamFoundation.Build.WebApi;
using TeamServicesTools.Web.Shared;

namespace TeamServicesTools.Web.Services
{
    public class BuildDefinitionsService
    {
        public static async Task<IEnumerable<BuildDefinitionReference>> GetDefinitionsAsync(Guid projectGuid)
        {
            return (await ClientService.BuildClient.GetDefinitionsAsync(projectGuid))
                .OrderBy(p => p.Name);
        }

        public static async Task RenameDefinition(Guid projectGuid, int definitionId, string newName)
        {
            var definition = await GetDefinitionAsync(projectGuid, definitionId);

            definition.Name = newName;

            await ClientService.BuildClient.UpdateDefinitionAsync(definition);
        }

        public static async Task<BuildDefinition> GetDefinitionAsync(Guid projectGuid, int definitionId)
        {
            return await ClientService.BuildClient.GetDefinitionAsync(projectGuid, definitionId);
        }

        public static async Task CloneDefinition(Guid projectGuid, int definitionId, Guid targetProjectGuid, string targetFolderPath, string newName)
        {
            var definition = await GetDefinitionAsync(projectGuid, definitionId);

            var clonedDefinition = CloningUtility.Clone(definition);

            clonedDefinition.Id = 0;
            clonedDefinition.Name = newName;
            clonedDefinition.Path = targetFolderPath;

            await ClientService.BuildClient.CreateDefinitionAsync(clonedDefinition);
        }

        public static async Task<IEnumerable<Folder>> GetFoldersAsync(Guid projectGuid)
        {
            return await ClientService.BuildClient.GetFoldersAsync(projectGuid);
        }

        public static async Task<List<SelectListItem>> GetFoldersSelectListItemsAsync(Guid projectGuid, string selectedFolderPath)
        {
            var items = (await GetFoldersAsync(projectGuid))
                .Select(p => new SelectListItem
                {
                    Value = p.Path.ToString(),
                    Text = p.Path,
                    Selected = p.Path.Equals(selectedFolderPath)
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
    }
}
