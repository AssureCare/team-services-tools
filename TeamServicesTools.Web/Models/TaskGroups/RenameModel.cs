using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.DistributedTask.WebApi;

namespace TeamServicesTools.Web.Models.TaskGroups
{
    public class RenameModel : BaseTaskGroupModel
    {
        public Guid? ProjectGuid { get; set; }
        public string ProjectName { get; set; }
        public IEnumerable<TaskGroup> Groups { get; set; }
    }
}
