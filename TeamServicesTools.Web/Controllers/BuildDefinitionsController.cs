﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using TeamServicesTools.Web.Models.BuildDefinitions;
using TeamServicesTools.Web.Services;
using TeamServicesTools.Web.Shared;

namespace TeamServicesTools.Web.Controllers
{
    public class BuildDefinitionsController : Controller
    {
        private static IEnumerable<int> GetFormDefinitionIdList(NameValueCollection form, string prefix)
        {
            return form.AllKeys
                .Where(k => k.StartsWith(prefix))
                .Select(k => k.Split('_')[1])
                .Select(id => Convert.ToInt32(id))
                .ToArray();
        }

        [TokenRequired]
        public async Task<ActionResult> Index(BuildDefinitionsModel model)
        {
            if (!model.ProjectGuid.HasValue)
                return Redirect("/Home");

            model.Definitions = await BuildDefinitionsService.GetDefinitionsAsync(model.ProjectGuid.GetValueOrDefault());
            return View(model);
        }

        [TokenRequired]
        public async Task<ActionResult> Rename(RenameModel model)
        {
            if (!model.ProjectGuid.HasValue)
                return Redirect("/Home");

            model.Definitions = (await BuildDefinitionsService.GetDefinitionsAsync(model.ProjectGuid.GetValueOrDefault()))
                .Where(g => model.DefinitionIds.Contains(g.Id));

            return View(model);
        }

        [TokenRequired, HttpPost, ActionName("Rename")]
        public async Task<ActionResult> Rename_Post(RenameModel model)
        {
            if (!model.ProjectGuid.HasValue)
                return Redirect("/Home");

            foreach (var id in GetFormDefinitionIdList(Request.Form, "definitionName"))
                await BuildDefinitionsService.RenameDefinition(model.ProjectGuid.GetValueOrDefault(), id, Request.Form[$"definitionName_{id}"]);

            var projectName = await ProjectService.GetProjectNameAsync(model.ProjectGuid.GetValueOrDefault());

            return Redirect($"/BuildDefinitions?ProjectGuid={model.ProjectGuid}&ProjectName={projectName}");
        }

        [TokenRequired, HttpPost]
        public async Task<ActionResult> Export(ExportModel model)
        {
            model.ProjectName = (await ProjectService.GetProjectAsync(model.ProjectGuid.GetValueOrDefault())).Name;

            var directory1 = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(directory1);

            foreach (var definitionId in model.DefinitionIds)
            {
                var definition = await BuildDefinitionsService.GetDefinitionAsync(model.ProjectGuid.GetValueOrDefault(), definitionId);
                var file = $"{FileUtility.RemoveInvalidFileCharacters(definition.Name)}.json";
                System.IO.File.WriteAllText(Path.Combine(directory1, file), JsonConvert.SerializeObject(definition, Formatting.Indented));
            }

            var friendlyFileName = $"{model.ProjectName.Replace(" ", "-")}_BuildDefinitions_{DateTime.Now:yyyy-MM-dd_HH-mm}.zip";
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

            model.Definitions = (await BuildDefinitionsService.GetDefinitionsAsync(model.ProjectGuid.GetValueOrDefault()))
                .Where(g => model.DefinitionIds.Contains(g.Id));

            model.Projects = await GetProjectsSelectListItems(Guid.Empty);

            model.Folders = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = "",
                    Text = "(select)"
                }
            };

            return View(model);
        }

        [TokenRequired, HttpPost, ActionName("Clone")]
        public async Task<ActionResult> Clone_Post(CloneModel model)
        {
            if (!model.SourceProjectGuid.HasValue)
                return Redirect("/Home");

            if (!string.IsNullOrWhiteSpace(model.TargetFolderPath))
            {
                foreach (var id in GetFormDefinitionIdList(Request.Form, "definitionName"))
                    await BuildDefinitionsService.CloneDefinition(model.SourceProjectGuid.GetValueOrDefault(), id,
                        model.TargetProjectGuid.GetValueOrDefault(), model.TargetFolderPath, Request.Form[$"definitionName_{id}"]);

                var projectName = await ProjectService.GetProjectNameAsync(model.TargetProjectGuid.GetValueOrDefault());

                return Redirect($"/BuildDefinitions?ProjectGuid={model.TargetProjectGuid}&ProjectName={projectName}");
            }

            model.Definitions = (await BuildDefinitionsService.GetDefinitionsAsync(model.ProjectGuid.GetValueOrDefault()))
                .Where(g => model.DefinitionIds.Contains(g.Id));

            model.Projects = await GetProjectsSelectListItems(Guid.Empty);

            if (model.TargetProjectGuid.HasValue)
            {
                model.Folders = await GetFoldersSelectListItems(model.TargetProjectGuid.GetValueOrDefault());
            }

            return View(model);
        }

        public async Task<List<SelectListItem>> GetProjectsSelectListItems(Guid projectGuid)
        {
            return await ProjectService.GetProjectSelectListItemsAsync(projectGuid);
        }

        public async Task<List<SelectListItem>> GetFoldersSelectListItems(Guid projectGuid)
        {
            return await BuildDefinitionsService.GetFoldersSelectListItemsAsync(projectGuid, string.Empty);
        }
    }
}
