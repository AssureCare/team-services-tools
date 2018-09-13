using System;
using System.Collections.Generic;
using Newtonsoft.Json;

// This file exists because the Microsoft.TeamFoundation NuGet packages are not compatible with each other
namespace TeamServicesTools.Web.Models.ReleaseDefinitions
{
    public class Avatar
    {
        [JsonProperty("href")]
        public string Href { get; set; }
    }

    public class Links
    {
        [JsonProperty("avatar")]
        public Avatar Avatar { get; set; }

        [JsonProperty("self")]
        public Self Self { get; set; }

        [JsonProperty("web")]
        public Web Web { get; set; }
    }

    public class IdentityRef
    {
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("_links")]
        public Links Links { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("uniqueName")]
        public string UniqueName { get; set; }

        [JsonProperty("imageUrl")]
        public string ImageUrl { get; set; }

        [JsonProperty("descriptor")]
        public string Descriptor { get; set; }
    }   

    public class Properties
    {
    }

    public class Self
    {
        [JsonProperty("href")]
        public string Href { get; set; }
    }

    public class Web
    {
        [JsonProperty("href")]
        public string Href { get; set; }
    }

    public class ReleaseDefinition
    {
        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("revision")]
        public int Revision { get; set; }

        [JsonProperty("description")]
        public object Description { get; set; }

        [JsonProperty("createdBy")]
        public IdentityRef CreatedBy { get; set; }

        [JsonProperty("createdOn")]
        public DateTime CreatedOn { get; set; }

        [JsonProperty("modifiedBy")]
        public IdentityRef ModifiedBy { get; set; }

        [JsonProperty("modifiedOn")]
        public DateTime ModifiedOn { get; set; }

        [JsonProperty("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonProperty("variableGroups")]
        public List<int> VariableGroups { get; set; }

        [JsonProperty("releaseNameFormat")]
        public string ReleaseNameFormat { get; set; }

        [JsonProperty("properties")]
        public Properties Properties { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("projectReference")]
        public object ProjectReference { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("_links")]
        public Links Links { get; set; }
    }

    public class ReleaseDefinitionCollection
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("value")]
        public List<ReleaseDefinition> Items { get; set; }
    }
}
