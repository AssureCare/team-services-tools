using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using TeamServicesTools.Web.Models.TaskGroups;
using TeamServicesTools.Web.Services;

namespace TeamServicesTools.Web.Controllers
{
    public class TaskGroupsController : Controller
    {
        [TokenRequired]
        public async Task<ActionResult> Index(Guid projectGuid, string projectName)
        {
            var model = new TaskGroupsModel
            {
                ProjectGuid = projectGuid,
                ProjectName = projectName,
                Groups = await TaskGroupService.GetTaskGroupsAsync(projectGuid)
            };
            return View(model);
        }
    }
}
