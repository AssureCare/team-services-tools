using Newtonsoft.Json;

namespace TeamServicesTools.Web.Models.Settings
{
    public class SettingsModel
    {
        public string AccountName { get; set; }
        public string PersonalAccessToken { get; set; }

        public string MaskedPersonalAccessToken =>
            string.IsNullOrWhiteSpace(PersonalAccessToken) ?
                "(not set)" : "******************************************************";

        public static SettingsModel FromJson(string json)
        {
            return JsonConvert.DeserializeObject<SettingsModel>(json);
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public bool IsConfigured
        {
            get
            {
                if (string.IsNullOrWhiteSpace(AccountName) || AccountName.Contains("not set")) return false;
                return !string.IsNullOrWhiteSpace(PersonalAccessToken) && !PersonalAccessToken.Contains("not set");
            }
        }
    }
}
