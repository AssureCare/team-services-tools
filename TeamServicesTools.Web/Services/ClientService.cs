using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Account.Client;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Organization.Client;
using Microsoft.VisualStudio.Services.WebApi;

namespace TeamServicesTools.Web.Services
{
    public class ClientService
    {
        public static AccountHttpClient AccountHttpClient => GetClient<AccountHttpClient>();

        public static OrganizationHttpClient OrganizationHttpClient => GetClient<OrganizationHttpClient>();

        public static TaskAgentHttpClient TaskAgentHttpClient => GetClient<TaskAgentHttpClient>();

        public static ProjectCollectionHttpClient ProjectCollectionHttpClient => GetClient<ProjectCollectionHttpClient>();

        public static ProjectHttpClient ProjectHttpClient => GetClient<ProjectHttpClient>();

        public static BuildHttpClient BuildClient => GetClient<BuildHttpClient>();

        private static TClient GetClient<TClient>() where TClient : VssHttpClientBase
        {
            return new VssConnection(
                    SettingsService.GetBaseUrl(),
                    new VssBasicCredential("pat", SettingsService.GetPersonalAccessToken()))
                .GetClient<TClient>();
        }
    }
}
