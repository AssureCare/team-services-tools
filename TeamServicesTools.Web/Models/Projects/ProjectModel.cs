using System;
using Microsoft.TeamFoundation.Core.WebApi;

namespace TeamServicesTools.Web.Models.Projects
{
    public class ProjectModel
    {
        public Guid? ProjectGuid { get; set; }
        public TeamProjectReference Project { get; set; }
    }
}
