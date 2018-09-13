using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using TeamServicesTools.Web.Models.ReleaseDefinitions;

namespace TeamServicesTools.Web.Services
{
    public class ReleaseDefinitionsService
    {
        public static string GetBaseUrl(Guid projectGuid)
        {
            return $"{SettingsService.GetReleaseManagementBaseUrl()}/{projectGuid}/_apis/";
        }

        public static async Task<IEnumerable<ReleaseDefinition>> GetDefinitionsAsync(Guid projectGuid)
        {
            const string url = "release/definitions/?api-version=4.1-preview.3";

            var request = new RestRequest(url, Method.GET)
                .AddHeader("Authorization", $"Basic {SettingsService.GetBasicAuthorizationValue()}");

            var client = new RestClient(GetBaseUrl(projectGuid));

            var response = await client.ExecuteTaskAsync(request);

            var definitionCollection = JsonConvert.DeserializeObject<ReleaseDefinitionCollection>(response.Content);

            return definitionCollection?.Items;
        }

        public static async Task RenameDefinition(Guid projectGuid, int definitionId, string newName)
        {
            // This is not ideal, but release definitions are large objects and I'm concerned
            // about the consequences of sending a partial object in a PUT request

            var json = await GetDefinitionJsonAsync(projectGuid, definitionId);

            var token = JToken.Parse(json);
            token["name"] = newName;

            await UpdateDefinitionJsonAsync(projectGuid, token.ToString());
        }

        public static async Task<string> GetDefinitionJsonAsync(Guid projectGuid, int definitionId)
        {
            var url = $"release/definitions/{definitionId}?api-version=4.1-preview.3";

            var request = new RestRequest(url, Method.GET)
                .AddHeader("Authorization", $"Basic {SettingsService.GetBasicAuthorizationValue()}");

            var client = new RestClient(GetBaseUrl(projectGuid));

            var response = await client.ExecuteTaskAsync(request);

            return response.Content;
        }

        //public static async Task<ReleaseDefinition> GetDefinitionAsync(Guid projectGuid, int definitionId)
        //{
        //    return JsonConvert.DeserializeObject<ReleaseDefinition>(await GetDefinitionJsonAsync(projectGuid, definitionId));
        //}

        public static async Task<string> CreateDefinitionJsonAsync(Guid projectGuid, string json)
        {
            const string url = "release/definitions?api-version=4.1-preview.3";

            var request = new RestRequest(url, Method.POST)
                .AddHeader("Authorization", $"Basic {SettingsService.GetBasicAuthorizationValue()}")
                .AddHeader("Content-Type", "application/json")
                .AddParameter("application/json", json, ParameterType.RequestBody);

            var client = new RestClient(GetBaseUrl(projectGuid));

            var response = await client.ExecuteTaskAsync(request);

            return response.Content;
        }

        public static async Task<string> UpdateDefinitionJsonAsync(Guid projectGuid, string json)
        {
            const string url = "release/definitions?api-version=4.1-preview.3";

            var request = new RestRequest(url, Method.PUT)
                .AddHeader("Authorization", $"Basic {SettingsService.GetBasicAuthorizationValue()}")
                .AddHeader("Content-Type", "application/json")
                .AddParameter("application/json", json, ParameterType.RequestBody);

            var client = new RestClient(GetBaseUrl(projectGuid));

            var response = await client.ExecuteTaskAsync(request);

            return response.Content;
        }

        //public static async Task<ReleaseDefinition> UpdateDefinitionAsync(Guid projectGuid, ReleaseDefinition definition)
        //{
        //    return await UpdateDefinitionJsonAsync(projectGuid, JsonConvert.SerializeObject(definition));
        //}

        public static async Task CloneDefinition(Guid projectGuid, int definitionId, Guid targetProjectGuid, string targetFolderPath, string newName)
        {
            var json = await GetDefinitionJsonAsync(projectGuid, definitionId);

            var token = JToken.Parse(json);
            token["name"] = newName;
            token["path"] = targetFolderPath;
            token["id"] = 0;
            token["revision"] = 0;

            await CreateDefinitionJsonAsync(projectGuid, token.ToString());
        }

        public static async Task<IEnumerable<ReleaseFolder>> GetFoldersAsync(Guid projectGuid)
        {
            var definitions = await GetDefinitionsAsync(projectGuid);

            return definitions
                .Select(d => d.Path)
                .Distinct()
                .OrderBy(p => p)
                .Select(p => new ReleaseFolder { Path = p })
                .ToList();
        }

        public class ReleaseFolder
        {
            public string Path { get; set; }
        }

        public static async Task<List<SelectListItem>> GetFoldersSelectListItemsAsync(Guid projectGuid, string selectedFolderPath)
        {
            var items = (await GetFoldersAsync(projectGuid))
                .Select(p => new SelectListItem
                {
                    Value = p.Path.ToString(),
                    Text = p.Path,
                    Selected = p.Path.Equals(selectedFolderPath)
                })
                .OrderBy(i => i.Text)
                .ToList();

            items.Insert(0, new SelectListItem
            {
                Value = "",
                Text = "(select)"
            });

            return items;
        }
    }
}
