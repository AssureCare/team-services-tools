using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Newtonsoft.Json;
using TeamServicesTools.Web.Models.VariableGroups;
using TeamServicesTools.Web.Services;

namespace TeamServicesTools.Web.Controllers
{
    public class VariableGroupsController : Controller
    {
        [TokenRequired]
        public async Task<ActionResult> Index(VariableGroupsModel model)
        {
            if (!model.ProjectGuid.HasValue)
                return Redirect("/Home");

            model.Groups = await VariableGroupService.GetVariableGroupsAsync(model.ProjectGuid.GetValueOrDefault());

            return View(model);
        }

        [TokenRequired]
        public async Task<ActionResult> VariableGroup(VariableGroupModel model)
        {
            if (!model.ProjectGuid.HasValue)
                return Redirect("/Home");

            model.Group = await VariableGroupService.GetVariableGroupAsync(
                model.ProjectGuid.GetValueOrDefault(), model.GroupId.GetValueOrDefault());

            return View(model);
        }

        [TokenRequired]
        public async Task<ActionResult> Clone(CloneModel model)
        {
            if (!model.ProjectGuid.HasValue)
                return Redirect("/Home");

            model.Groups = (await VariableGroupService.GetVariableGroupsAsync(model.ProjectGuid.GetValueOrDefault()))
                .Where(g => model.GroupIds.Contains(g.Id));

            model.Projects = await GetProjectsSelectListItems(Guid.Empty);

            return View(model);
        }

        [TokenRequired, HttpPost, ActionName("Clone")]
        public async Task<ActionResult> Clone_Post(CloneModel model)
        {
            if (!model.SourceProjectGuid.HasValue)
                return Redirect("/Home");

            foreach (var id in GetFormGroupIdList(Request.Form, "groupName"))
                await VariableGroupService.CloneGroup(model.SourceProjectGuid.GetValueOrDefault(), id,
                    model.TargetProjectGuid.GetValueOrDefault(), Request.Form[$"groupName_{id}"]);

            var projectName = await ProjectService.GetProjectNameAsync(model.TargetProjectGuid.GetValueOrDefault());

            return Redirect($"/VariableGroups?ProjectGuid={model.TargetProjectGuid}&ProjectName={projectName}");
        }

        [TokenRequired]
        public async Task<ActionResult> Rename(RenameModel model)
        {
            if (!model.ProjectGuid.HasValue)
                return Redirect("/Home");

            model.Groups = (await VariableGroupService.GetVariableGroupsAsync(model.ProjectGuid.GetValueOrDefault()))
                .Where(g => model.GroupIds.Contains(g.Id));

            return View(model);
        }

        [TokenRequired, HttpPost, ActionName("Rename")]
        public async Task<ActionResult> Rename_Post(RenameModel model)
        {
            if (!model.ProjectGuid.HasValue)
                return Redirect("/Home");

            foreach (var id in GetFormGroupIdList(Request.Form, "groupName"))
                await VariableGroupService.RenameGroup(model.ProjectGuid.GetValueOrDefault(), id, Request.Form[$"groupName_{id}"]);

            var projectName = await ProjectService.GetProjectNameAsync(model.ProjectGuid.GetValueOrDefault());

            return Redirect($"/VariableGroups?ProjectGuid={model.ProjectGuid}&ProjectName={projectName}");
        }

        [TokenRequired, HttpPost]
        public async Task<ActionResult> Export(ExportModel model)
        {
            model.ProjectName = (await ProjectService.GetProjectAsync(model.ProjectGuid.GetValueOrDefault())).Name;

            var directory1 = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(directory1);

            foreach (var groupId in model.GroupIds)
            {
                var group = await VariableGroupService.GetVariableGroupAsync(model.ProjectGuid.GetValueOrDefault(), Convert.ToInt32(groupId));
                var file = $"{RemoveInvalidFileCharacters(group.Name)}.json";
                System.IO.File.WriteAllText(Path.Combine(directory1, file), JsonConvert.SerializeObject(group, Formatting.Indented));
            }

            var friendlyFileName = $"{model.ProjectName.Replace(" ", "-")}_VariableGroups_{DateTime.Now:yyyy-MM-dd_HH-mm}.zip";
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
        public async Task<ActionResult> Compare(CompareModel model)
        {
            model.Group1Projects = await GetProjectsSelectListItems(model.Project1Guid.GetValueOrDefault());
            model.Group2Projects = await GetProjectsSelectListItems(model.Project2Guid.GetValueOrDefault());

            if (model.Project1Guid.HasValue)
                model.Project1Groups = await GetVariableGroupsSelectListItems(model.Project1Guid.Value, model.Group1Id.GetValueOrDefault());

            if (model.Project2Guid.HasValue)
                model.Project2Groups = await GetVariableGroupsSelectListItems(model.Project2Guid.GetValueOrDefault(), model.Group2Id.GetValueOrDefault());

            if (model.Group1Id.HasValue && model.Group2Id.HasValue)
            {
                model.Group1 = await VariableGroupService.GetVariableGroupAsync(model.Project1Guid.GetValueOrDefault(), model.Group1Id.GetValueOrDefault());
                model.Group2 = await VariableGroupService.GetVariableGroupAsync(model.Project2Guid.GetValueOrDefault(), model.Group2Id.GetValueOrDefault());
                model.Results = Comparer.Compare(model.Group1, model.Group2);
            }

            return View(model);
        }

        [TokenRequired]
        public ActionResult AddGroup(AddGroupModel model)
        {
            if (!model.ProjectGuid.HasValue)
                return Redirect("/Home");

            return View(model);
        }

        [TokenRequired, HttpPost, ActionName("AddGroup")]
        public async Task<ActionResult> AddGroup_Post(AddGroupModel model)
        {
            if (!model.ProjectGuid.HasValue)
                return Redirect("/Home");

            var group = await VariableGroupService.AddVariableGroupAsync(model.ProjectGuid.GetValueOrDefault(),
                new VariableGroup { Name = model.GroupName, Description = model.GroupDescription });

            var projectName = await ProjectService.GetProjectNameAsync(model.ProjectGuid.GetValueOrDefault());

            return Redirect($"/VariableGroups/VariableGroup?ProjectGuid={model.ProjectGuid}&ProjectName={projectName}&GroupId={group.Id}");
        }

        [TokenRequired]
        public async Task<ActionResult> AddVariable(AddVariableModel model)
        {
            if (!model.ProjectGuid.HasValue)
                return Redirect("/Home");

            model.Groups = (await VariableGroupService.GetVariableGroupsAsync(model.ProjectGuid.GetValueOrDefault()))
                .Where(g => model.GroupIds.Contains(g.Id));

            model.Projects = await GetProjectsSelectListItems(Guid.Empty);

            return View(model);
        }

        [TokenRequired, HttpPost, ActionName("AddVariable")]
        public async Task<ActionResult> AddVariable_Post(AddVariableModel model)
        {
            if (!model.ProjectGuid.HasValue)
                return Redirect("/Home");

            foreach (var id in model.GroupIds)
                await VariableGroupService.AddVariable(model.ProjectGuid.GetValueOrDefault(), id, model.Key,
                    new VariableValue { Value = model.Value, IsSecret = model.IsSecret });

            var projectName = await ProjectService.GetProjectNameAsync(model.ProjectGuid.GetValueOrDefault());

            return Redirect($"/VariableGroups?ProjectGuid={model.ProjectGuid}&ProjectName={projectName}");
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
