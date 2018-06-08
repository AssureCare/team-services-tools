using System;
using System.Collections.Generic;
using System.Linq;

namespace TeamServicesTools.Web.Models.VariableGroups
{
    public class BaseVariableGroupModel
    {
        public string GroupIdList { get; set; }
        public IEnumerable<int> GroupIds => GroupIdList?.Split(',').Select(t => Convert.ToInt32(t)) ?? new int[]{};
    }
}
