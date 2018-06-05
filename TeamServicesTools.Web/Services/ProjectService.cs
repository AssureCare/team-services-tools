using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.TeamFoundation.Core.WebApi;

namespace TeamServicesTools.Web.Services
{
    public class ProjectService
    {
        public static async Task<IEnumerable<TeamProjectReference>> GetProjectsAsync()
        {
            return (await ClientService.ProjectHttpClient.GetProjects())
                .OrderBy(p => p.Name);
        }

        public static async Task<TeamProjectReference> GetProjectAsync(Guid projectGuid)
        {
            return await ClientService.ProjectHttpClient.GetProject(projectGuid.ToString());
        }

        public static async Task<List<SelectListItem>> GetProjectSelectListItemsAsync(Guid selectedProjectGuid)
        {
            var items = (await GetProjectsAsync())
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name,
                    Selected = p.Id.Equals(selectedProjectGuid)
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
