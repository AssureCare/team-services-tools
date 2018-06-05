using System.Collections.Generic;
using System.Linq;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Common;

namespace TeamServicesTools.Web.Models.VariableGroups
{
    public class Comparer
    {
        public static List<ComparisonResult> Compare(VariableGroup group1, VariableGroup group2)
        {
            var results = new List<ComparisonResult>();

            var allKeys = new List<string>();
            allKeys.AddRange(group1.Variables.Keys);
            allKeys.AddRange(group2.Variables.Keys);
            allKeys = allKeys.Distinct().ToList();

            foreach (var key in allKeys)
            {
                var result = new ComparisonResult
                {
                    Key = key,
                    Group1Value = GetVariableValue(group1, key),
                    Group2Value = GetVariableValue(group2, key)
                };

                var inGroup1 = group1.Variables.Keys.Contains(key);
                var inGroup2 = group2.Variables.Keys.Contains(key);

                if (inGroup1 && !inGroup2)
                {
                    result.State = ComparisonState.Group1Only;
                    results.Add(result);
                    continue;
                }

                if (inGroup2 && !inGroup1)
                {
                    result.State = ComparisonState.Group2Only;
                    results.Add(result);
                    continue;
                }

                if (result.Group1Value == null && result.Group2Value != null)
                {
                    result.State = ComparisonState.Different;
                    results.Add(result);
                    continue;
                }

                if (result.Group2Value == null && result.Group1Value != null)
                {
                    result.State = ComparisonState.Different;
                    results.Add(result);
                    continue;
                }

                if (result.Group1Value == null && result.Group2Value == null)
                {
                    result.State = ComparisonState.Same;
                    results.Add(result);
                    continue;
                }

                if (result.Group1Value != null && result.Group2Value != null)
                {
                    result.State = result.Group1Value == result.Group2Value
                        ? ComparisonState.Same
                        : ComparisonState.Different;
                    results.Add(result);
                }
            }

            return results;
        }

        private static string GetVariableValue(VariableGroup group, string key)
        {
            if (key == null) return null;
            if (!group.Variables.Any()) return null;

            return group.Variables.Any(v => v.Key.Equals(key))
                ? group.Variables[key].Value
                : null;
        }
    }
}