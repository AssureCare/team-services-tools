using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.Build.WebApi;

namespace TeamServicesTools.Web.Models.BuildDefinitions
{
    public class ExportModel : BaseBuildDefinitionModel
    {
        public Guid? ProjectGuid { get; set; }
        public string ProjectName { get; set; }
        public IEnumerable<BuildDefinitionReference> Definitions { get; set; }
    }
}
