using System.Runtime.Serialization;
using Microsoft.TeamFoundation.DistributedTask.WebApi;

namespace TeamServicesTools.Web.Models.TaskGroups
{
    public class TaskGroup41Preview1 : TaskGroup
    {
        [DataMember(EmitDefaultValue = true)]
        public int Revision { get; set; }
    }
}
