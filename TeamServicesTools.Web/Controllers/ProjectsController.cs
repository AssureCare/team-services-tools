using System;
using System.Threading.Tasks;
using System.Web.Mvc;
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
        public async Task<ActionResult> Project(Guid? projectGuid)
        {
            if (!projectGuid.HasValue)
                return Redirect("/Home");

            return View(await ProjectService.GetProjectAsync(projectGuid.GetValueOrDefault()));
        }
    }
}
