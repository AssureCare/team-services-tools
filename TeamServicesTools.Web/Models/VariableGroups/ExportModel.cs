using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.DistributedTask.WebApi;

namespace TeamServicesTools.Web.Models.VariableGroups
{
    public class ExportModel : BaseVariableGroupModel
    {
        public Guid? ProjectGuid { get; set; }
        public string ProjectName { get; set; }
        public IEnumerable<VariableGroup> Groups { get; set; }
    }
}
