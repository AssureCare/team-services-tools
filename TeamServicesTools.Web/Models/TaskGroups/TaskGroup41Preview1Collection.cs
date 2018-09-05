using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TeamServicesTools.Web.Models.TaskGroups
{
    public class TaskGroup41Preview1Collection
    {
        [DataMember(Name = "count")]
        public int Count { get; set; }

        [DataMember(Name = "value")]
        public IEnumerable<TaskGroup41Preview1> Value { get; set; }
    }
}
