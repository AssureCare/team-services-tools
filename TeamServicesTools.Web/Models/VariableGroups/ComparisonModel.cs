using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.TeamFoundation.DistributedTask.WebApi;

namespace TeamServicesTools.Web.Models.VariableGroups
{
    public class ComparisonModel
    {
        public List<SelectListItem> Group1Projects { get; set; }
        public List<SelectListItem> Group2Projects { get; set; }
        public List<SelectListItem> Project1Groups { get; set; }
        public List<SelectListItem> Project2Groups { get; set; }

        public Guid Project1Guid { get; set; }
        public Guid Project2Guid { get; set; }

        public int Group1Id { get; set; }
        public int Group2Id { get; set; }

        public VariableGroup Group1 { get; set; }
        public VariableGroup Group2 { get; set; }

        public List<ComparisonResult> Results { get; set; }
    }
}
