using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.TeamFoundation.DistributedTask.WebApi;

namespace TeamServicesTools.Web.Models.VariableGroups
{
    public class AddVariableModel : BaseVariableGroupModel
    {
        public Guid? ProjectGuid { get; set; }
        public string ProjectName { get; set; }
        public IEnumerable<VariableGroup> Groups { get; set; }
        public List<SelectListItem> Projects { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public bool IsSecret { get; set; }
    }
}
