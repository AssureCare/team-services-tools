using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace TeamServicesTools.Web.Models.ReleaseDefinitions
{
    public class CloneModel : BaseReleaseDefinitionModel
    {
        public Guid? ProjectGuid { get; set; }
        public string ProjectName { get; set; }
        public IEnumerable<ReleaseDefinition> Definitions { get; set; }
        public List<SelectListItem> Projects { get; set; }
        public List<SelectListItem> Folders { get; set; }
        public Guid? SourceProjectGuid { get; set; }
        public Guid? TargetProjectGuid { get; set; }
        public string TargetFolderPath { get; set; }
    }
}
