using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TeamServicesTools.Web.Services;

namespace TeamServicesTools.Web.Controllers
{
    public class ProjectCollectionsController : Controller
    {
        [TokenRequired]
        public async Task<ActionResult> Index()
        {
            return View(await ProjectCollectionService.GetProjectCollectionsAsync());
        }

        [TokenRequired]
        public async Task<ActionResult> ProjectCollection(Guid? projectCollectionGuid)
        {
            if (!projectCollectionGuid.HasValue)
                return Redirect("/Home");

            var projectCollection = (await ProjectCollectionService.GetProjectCollectionsAsync())
                .Single(pc => pc.Id.Equals(projectCollectionGuid));

            return View(projectCollection);
        }
    }
}
