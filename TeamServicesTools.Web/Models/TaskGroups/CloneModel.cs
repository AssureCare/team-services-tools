using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.TeamFoundation.DistributedTask.WebApi;

namespace TeamServicesTools.Web.Models.TaskGroups
{
    public class CloneModel : BaseTaskGroupModel
    {
        public Guid? ProjectGuid { get; set; }
        public string ProjectName { get; set; }
        public IEnumerable<TaskGroup> Groups { get; set; }
        public List<SelectListItem> Projects { get; set; }
        public Guid? SourceProjectGuid { get; set; }
        public Guid? TargetProjectGuid { get; set; }
    }
}
