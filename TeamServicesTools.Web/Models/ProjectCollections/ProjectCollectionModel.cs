using System;
using Microsoft.TeamFoundation.Core.WebApi;

namespace TeamServicesTools.Web.Models.ProjectCollections
{
    public class ProjectCollectionModel
    {
        public Guid? ProjectCollectionGuid { get; set; }
        public TeamProjectCollectionReference ProjectCollection { get; set; }
    }
}
