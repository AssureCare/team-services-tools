using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using TeamServicesTools.Web.Models.VariableGroups;
using TeamServicesTools.Web.Services;

namespace TeamServicesTools.Web.Controllers
{
    public class VariableGroupsController : Controller
    {
        [TokenRequired]
        public async Task<ActionResult> Index(Guid? projectGuid, string projectName)
        {
            if (!projectGuid.HasValue)
                return Redirect("/Home");

            var model = new VariableGroupsModel
            {
                ProjectGuid = projectGuid.GetValueOrDefault(),
                ProjectName = projectName,
                Groups = await VariableGroupService.GetVariableGroupsAsync(projectGuid.GetValueOrDefault())
            };

            return View(model);
        }

        [TokenRequired]
        public async Task<ActionResult> VariableGroup(Guid? projectGuid, string projectName, int groupId)
        {
            if (!projectGuid.HasValue)
                return Redirect("/Home");

            var model = new VariableGroupModel
            {
                ProjectGuid = projectGuid.GetValueOrDefault(),
                ProjectName = projectName,
                Group = await VariableGroupService.GetVariableGroupAsync(projectGuid.GetValueOrDefault(), groupId)
            };

            return View(model);
        }

        [TokenRequired]
        public async Task<ActionResult> Clone(Guid? projectGuid, string groupIds, string projectName)
        {
            if (!projectGuid.HasValue)
                return Redirect("/Home");

            var ids = groupIds.Split(',').Select(t => Convert.ToInt32(t));

            var model = new CloneModel
            {
                ProjectGuid = projectGuid.GetValueOrDefault(),
                ProjectName = projectName,
                GroupIds = groupIds.Split(',').Select(t => Convert.ToInt32(t)).ToArray(),
                Groups = (await VariableGroupService.GetVariableGroupsAsync(projectGuid.GetValueOrDefault()))
                    .Where(g => ids.Contains(g.Id)),
                Projects = await GetProjectsSelectListItems(Guid.Empty)
            };

            return View(model);
        }

        [TokenRequired, HttpPost]
        public async Task<ActionResult> Clone(Guid? sourceProjectGuid, Guid? targetProjectGuid, string groupIds, string action)
        {
            if (!sourceProjectGuid.HasValue)
                return Redirect("/Home");

            foreach (var id in GetFormGroupIdList(Request.Form, "groupName"))
                await VariableGroupService.CloneGroup(sourceProjectGuid.GetValueOrDefault(), id, 
                    targetProjectGuid.GetValueOrDefault(), Request.Form[$"groupName_{id}"]);

            var projectName = (await ProjectService.GetProjectAsync(targetProjectGuid.GetValueOrDefault())).Name;

            return Redirect($"/VariableGroups?projectGuid={targetProjectGuid}&projectName={projectName}");
        }

        [TokenRequired]
        public async Task<ActionResult> Rename(Guid? projectGuid, string groupIds, string projectName)
        {
            if (!projectGuid.HasValue)
                return Redirect("/Home");

            var ids = groupIds.Split(',').Select(t => Convert.ToInt32(t));

            var model = new RenameModel
            {
                ProjectGuid = projectGuid.GetValueOrDefault(),
                ProjectName = projectName,
                GroupIds = groupIds.Split(',').Select(t => Convert.ToInt32(t)).ToArray(),
                Groups = (await VariableGroupService.GetVariableGroupsAsync(projectGuid.GetValueOrDefault()))
                    .Where(g => ids.Contains(g.Id))
            };

            return View(model);
        }

        [TokenRequired, HttpPost]
        public async Task<ActionResult> Rename(Guid? projectGuid, string groupIds)
        {
            if (!projectGuid.HasValue)
                return Redirect("/Home");

            foreach (var id in GetFormGroupIdList(Request.Form, "groupName"))
                await VariableGroupService.RenameGroup(projectGuid.GetValueOrDefault(), id, Request.Form[$"groupName_{id}"]);

            var projectName = (await ProjectService.GetProjectAsync(projectGuid.GetValueOrDefault())).Name;

            return Redirect($"/VariableGroups?projectGuid={projectGuid}&projectName={projectName}");
        }

        [TokenRequired, HttpPost]
        public async Task<ActionResult> Export(Guid projectGuid, string groupIds)
        {
            var ids = groupIds.Split(',').Select(t => Convert.ToInt32(t));
            var projectName = (await ProjectService.GetProjectAsync(projectGuid)).Name;

            var directory1 = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            Directory.CreateDirectory(directory1);

            foreach (var groupId in ids)
            {
                var group = await VariableGroupService.GetVariableGroupAsync(projectGuid, Convert.ToInt32(groupId));
                var file = $"{RemoveInvalidFileCharacters(group.Name)}.json";
                System.IO.File.WriteAllText(Path.Combine(directory1, file), JsonConvert.SerializeObject(group, Formatting.Indented));
            }

            var friendlyFileName = $"{projectName.Replace(" ", "-")}_VariableGroups_{DateTime.Now:yyyy-MM-dd_HH-mm}.zip";
            var archive = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            System.IO.Compression.ZipFile.CreateFromDirectory(directory1, archive);

            Directory.Delete(directory1, true);

            return File(System.IO.File.OpenRead(archive), "application/zip", friendlyFileName);
        }

        private static string RemoveInvalidFileCharacters(string input)
        {
            return input
                .ToCharArray()
                .Where(c => !Path.GetInvalidFileNameChars().Contains(c))
                .Aggregate(string.Empty, (current, c) => current + c);
        }

        [TokenRequired]
        public async Task<ActionResult> Compare(Guid? project1Guid, int? group1Id, Guid? project2Guid, int? group2Id)
        {
            var model = new ComparisonModel
            {
                Group1Projects = await GetProjectsSelectListItems(project1Guid.GetValueOrDefault()),
                Group2Projects = await GetProjectsSelectListItems(project2Guid.GetValueOrDefault()),
                Project1Guid = project1Guid.GetValueOrDefault(),
                Group1Id = group1Id.GetValueOrDefault(),
                Project2Guid = project2Guid.GetValueOrDefault(),
                Group2Id = group2Id.GetValueOrDefault()
            };

            if (project1Guid.HasValue)
                model.Project1Groups = await GetVariableGroupsSelectListItems(model.Project1Guid, group1Id.GetValueOrDefault());

            if (project2Guid.HasValue)
                model.Project2Groups = await GetVariableGroupsSelectListItems(model.Project2Guid, group2Id.GetValueOrDefault());

            if (group1Id.HasValue && group2Id.HasValue)
            {
                model.Group1 = await VariableGroupService.GetVariableGroupAsync(project1Guid.GetValueOrDefault(), group1Id.GetValueOrDefault());
                model.Group2 = await VariableGroupService.GetVariableGroupAsync(project2Guid.GetValueOrDefault(), group2Id.GetValueOrDefault());
                model.Results = Comparer.Compare(model.Group1, model.Group2);
            }

            return View(model);
        }

        [TokenRequired]
        public async Task<ActionResult> AddVariable(Guid? projectGuid, string groupIds, string projectName)
        {
            if (!projectGuid.HasValue)
                return Redirect("/Home");

            var ids = groupIds.Split(',').Select(t => Convert.ToInt32(t));

            var model = new AddVariableModel
            {
                ProjectGuid = projectGuid.GetValueOrDefault(),
                ProjectName = projectName,
                GroupIds = groupIds.Split(',').Select(t => Convert.ToInt32(t)).ToArray(),
                Groups = (await VariableGroupService.GetVariableGroupsAsync(projectGuid.GetValueOrDefault()))
                    .Where(g => ids.Contains(g.Id)),
                Projects = await GetProjectsSelectListItems(Guid.Empty)
            };

            return View(model);
        }

        [TokenRequired, HttpPost]
        public async Task<ActionResult> AddVariable(Guid? projectGuid, string groupIds, string key, string value, bool isSecret)
        {
            if (!projectGuid.HasValue)
                return Redirect("/Home");

            foreach (var id in groupIds.Split(',').Select(i => Convert.ToInt32(i)))
                await VariableGroupService.AddVariableValue(projectGuid.GetValueOrDefault(), id, key, value, isSecret);

            var projectName = (await ProjectService.GetProjectAsync(projectGuid.GetValueOrDefault())).Name;

            return Redirect($"/VariableGroups?projectGuid={projectGuid}&projectName={projectName}");
        }

        private static IEnumerable<int> GetFormGroupIdList(NameValueCollection form, string prefix)
        {
            return form.AllKeys
                .Where(k => k.StartsWith(prefix))
                .Select(k => k.Split('_')[1])
                .Select(id => Convert.ToInt32(id))
                .ToArray();
        }

        public async Task<List<SelectListItem>> GetProjectsSelectListItems(Guid projectGuid)
        {
            return await ProjectService.GetProjectSelectListItemsAsync(projectGuid);
        }

        public async Task<List<SelectListItem>> GetVariableGroupsSelectListItems(Guid projectGuid, int groupId)
        {
            return await VariableGroupService.GetVariableGroupsSelectListItemsAsync(projectGuid, groupId);
        }
    }
}
