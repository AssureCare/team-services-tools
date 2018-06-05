﻿using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.TeamFoundation.DistributedTask.WebApi;

namespace TeamServicesTools.Web.Models.VariableGroups
{
    public class CloneModel
    {
        public Guid ProjectGuid { get; set; }
        public string ProjectName { get; set; }
        public int[] GroupIds { get; set; }
        public IEnumerable<VariableGroup> Groups { get; set; }
        public List<SelectListItem> Projects { get; set; }
    }
}
