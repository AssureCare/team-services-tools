using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TeamServicesTools.Web.Models.ProjectCollections;
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
        public async Task<ActionResult> ProjectCollection(ProjectCollectionModel model)
        {
            if (!model.ProjectCollectionGuid.HasValue)
                return Redirect("/Home");

            model.ProjectCollection = (await ProjectCollectionService.GetProjectCollectionsAsync())
                .Single(pc => pc.Id.Equals(model.ProjectCollectionGuid));

            return View(model);
        }
    }
}
