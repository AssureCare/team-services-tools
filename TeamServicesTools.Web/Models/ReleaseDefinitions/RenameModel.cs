using System;
using System.Collections.Generic;

namespace TeamServicesTools.Web.Models.ReleaseDefinitions
{
    public class RenameModel : BaseReleaseDefinitionModel
    {
        public Guid? ProjectGuid { get; set; }
        public string ProjectName { get; set; }
        public IEnumerable<ReleaseDefinition> Definitions { get; set; }
    }
}
