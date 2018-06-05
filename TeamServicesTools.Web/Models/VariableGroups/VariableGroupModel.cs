using System;
using Microsoft.TeamFoundation.DistributedTask.WebApi;

namespace TeamServicesTools.Web.Models.VariableGroups
{
    public class VariableGroupModel
    {
        public Guid ProjectGuid { get; set; }
        public string ProjectName { get; set; }
        public VariableGroup Group { get; set; }
    }
}