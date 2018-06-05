namespace TeamServicesTools.Web.Models.VariableGroups
{
    public class ComparisonResult
    {
        public string Key { get; set; }
        public string Group1Value { get; set; }
        public string Group2Value { get; set; }
        public ComparisonState State { get; set; }
    }
}
