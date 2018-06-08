using System.Threading.Tasks;
using System.Web.Mvc;
using TeamServicesTools.Web.Models.TaskGroups;
using TeamServicesTools.Web.Services;

namespace TeamServicesTools.Web.Controllers
{
    public class TaskGroupsController : Controller
    {
        [TokenRequired]
        public async Task<ActionResult> Index(TaskGroupsModel model)
        {
            if (!model.ProjectGuid.HasValue)
                return Redirect("/Home");

            model.Groups = await TaskGroupService.GetTaskGroupsAsync(model.ProjectGuid.GetValueOrDefault());
            return View(model);
        }
    }
}
