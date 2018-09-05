using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using TeamServicesTools.Web.Models.TaskGroups;
using TeamServicesTools.Web.Services;
using TeamServicesTools.Web.Shared;

namespace TeamServicesTools.Web.Controllers
{
    public class TaskGroupsController : Controller
    {
        private static IEnumerable<Guid> GetFormGroupIdList(NameValueCollection form, string prefix)
        {
            return form.AllKeys
                .Where(k => k.StartsWith(prefix))
                .Select(k => k.Split('_')[1])
                .Select(Guid.Parse)
                .ToArray();
        }

        [TokenRequired]
        public async Task<ActionResult> Index(TaskGroupsModel model)
        {
            if (!model.ProjectGuid.HasValue)
                return Redirect("/Home");

            model.Groups = await TaskGroupService.GetTaskGroupsAsync(model.ProjectGuid.GetValueOrDefault());
            return View(model);
        }

        [TokenRequired]
        public async Task<ActionResult> Rename(RenameModel model)
        {
            if (!model.ProjectGuid.HasValue)
                return Redirect("/Home");

            model.Groups = (await TaskGroupService.GetTaskGroupsAsync(model.ProjectGuid.GetValueOrDefault()))
                .Where(g => model.GroupIds.Contains(g.Id));

            return View(model);
        }

        [TokenRequired, HttpPost, ActionName("Rename")]
        public async Task<ActionResult> Rename_Post(RenameModel model)
        {
            if (!model.ProjectGuid.HasValue)
                return Redirect("/Home");

            foreach (var id in GetFormGroupIdList(Request.Form, "groupName"))
                await TaskGroupService.RenameGroup(model.ProjectGuid.GetValueOrDefault(), id, Request.Form[$"groupName_{id}"]);

            var projectName = await ProjectService.GetProjectNameAsync(model.ProjectGuid.GetValueOrDefault());

            return Redirect($"/TaskGroups?ProjectGuid={model.ProjectGuid}&ProjectName={projectName}");
        }

        [TokenRequired, HttpPost]
        public async Task<ActionResult> Export(ExportModel model)
        {
            model.ProjectName = (await ProjectService.GetProjectAsync(model.ProjectGuid.GetValueOrDefault())).Name;

            var directory1 = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(directory1);

            foreach (var groupId in model.GroupIds)
            {
                var group = await TaskGroupService.GetTaskGroupAsync(model.ProjectGuid.GetValueOrDefault(), groupId);
                var file = $"{FileUtility.RemoveInvalidFileCharacters(group.Name)}.json";
                System.IO.File.WriteAllText(Path.Combine(directory1, file), JsonConvert.SerializeObject(group, Formatting.Indented));
            }

            var friendlyFileName = $"{model.ProjectName.Replace(" ", "-")}_TaskGroups_{DateTime.Now:yyyy-MM-dd_HH-mm}.zip";
            var archive = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            System.IO.Compression.ZipFile.CreateFromDirectory(directory1, archive);

            Directory.Delete(directory1, true);

            return File(System.IO.File.OpenRead(archive), "application/zip", friendlyFileName);
        }

        [TokenRequired]
        public async Task<ActionResult> Clone(CloneModel model)
        {
            if (!model.ProjectGuid.HasValue)
                return Redirect("/Home");

            model.Groups = (await TaskGroupService.GetTaskGroupsAsync(model.ProjectGuid.GetValueOrDefault()))
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
                await TaskGroupService.CloneGroup(model.SourceProjectGuid.GetValueOrDefault(), id,
                    model.TargetProjectGuid.GetValueOrDefault(), Request.Form[$"groupName_{id}"]);

            var projectName = await ProjectService.GetProjectNameAsync(model.TargetProjectGuid.GetValueOrDefault());

            return Redirect($"/TaskGroups?ProjectGuid={model.TargetProjectGuid}&ProjectName={projectName}");
        }

        public async Task<List<SelectListItem>> GetProjectsSelectListItems(Guid projectGuid)
        {
            return await ProjectService.GetProjectSelectListItemsAsync(projectGuid);
        }
    }
}
