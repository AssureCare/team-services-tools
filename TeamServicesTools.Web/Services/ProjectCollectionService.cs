using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Core.WebApi;

namespace TeamServicesTools.Web.Services
{
    public class ProjectCollectionService
    {
        public static async Task<IEnumerable<TeamProjectCollectionReference>> GetProjectCollectionsAsync()
        {
            return (await ClientService.ProjectCollectionHttpClient.GetProjectCollections())
                .OrderBy(p => p.Name);
        }
    }
}
