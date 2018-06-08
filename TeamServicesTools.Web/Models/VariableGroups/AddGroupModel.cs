using System;

namespace TeamServicesTools.Web.Models.VariableGroups
{
    public class AddGroupModel
    {
        public Guid? ProjectGuid { get; set; }
        public string ProjectName { get; set; }
        public string GroupName { get; set; }
    }
}
