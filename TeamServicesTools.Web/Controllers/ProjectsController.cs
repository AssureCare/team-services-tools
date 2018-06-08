using System.Threading.Tasks;
using System.Web.Mvc;
using TeamServicesTools.Web.Models.Projects;
using TeamServicesTools.Web.Services;

namespace TeamServicesTools.Web.Controllers
{
    public class ProjectsController : Controller
    {
        [TokenRequired]
        public async Task<ActionResult> Index()
        {
            return View(await ProjectService.GetProjectsAsync());
        }

        [TokenRequired]
        public async Task<ActionResult> Project(ProjectModel model)
        {
            if (!model.ProjectGuid.HasValue)
                return Redirect("/Home");

            model.Project = await ProjectService.GetProjectAsync(model.ProjectGuid.GetValueOrDefault());

            return View(model);
        }
    }
}
