using System;
using System.Collections.Generic;
using System.Linq;

namespace TeamServicesTools.Web.Models.BuildDefinitions
{
    public class BaseBuildDefinitionModel
    {
        public string DefinitionIdList { get; set; }
        public IEnumerable<int> DefinitionIds => DefinitionIdList?.Split(',').Select(t => Convert.ToInt32(t)) ?? new int[]{};
    }
}
