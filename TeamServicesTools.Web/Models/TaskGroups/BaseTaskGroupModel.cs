using System;
using System.Collections.Generic;
using System.Linq;

namespace TeamServicesTools.Web.Models.TaskGroups
{
    public class BaseTaskGroupModel
    {
        public string GroupIdList { get; set; }
        public IEnumerable<Guid> GroupIds => GroupIdList?.Split(',').Select(Guid.Parse) ?? new Guid[]{};
    }
}
