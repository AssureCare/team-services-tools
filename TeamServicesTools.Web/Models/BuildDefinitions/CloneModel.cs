using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.TeamFoundation.Build.WebApi;

namespace TeamServicesTools.Web.Models.BuildDefinitions
{
    public class CloneModel : BaseBuildDefinitionModel
    {
        public Guid? ProjectGuid { get; set; }
        public string ProjectName { get; set; }
        public IEnumerable<BuildDefinitionReference> Definitions { get; set; }
        public List<SelectListItem> Projects { get; set; }
        public List<SelectListItem> Folders { get; set; }
        public Guid? SourceProjectGuid { get; set; }
        public Guid? TargetProjectGuid { get; set; }
        public string TargetFolderPath { get; set; }
    }
}
