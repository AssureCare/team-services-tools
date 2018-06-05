using System;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using TeamServicesTools.Web.Models.Settings;

namespace TeamServicesTools.Web.Services
{
    public class SettingsService
    {
        private const string CookieName = "Settings";

        private static HttpCookieCollection RequestCookies => HttpContext.Current.Request.Cookies;

        public static bool IsConfigured => GetSettings().IsConfigured;

        public static Uri GetBaseUrl()
        {
            return new Uri($"https://{GetSettings().AccountName}.visualstudio.com");
        }

        public static string GetPersonalAccessToken()
        {
            return GetSettings().PersonalAccessToken;
        }

        public static SettingsModel GetSettings()
        {
            if (RequestCookies[CookieName] == null)
            {
                return new SettingsModel
                {
                    AccountName = "",
                    PersonalAccessToken = ""
                };
            }

            return SettingsModel.FromJson(DecryptData(RequestCookies[CookieName].Value));
        }

        public static string ProtectSettings(SettingsModel model)
        {
            return EncryptData(model.ToJson());
        }

        private static string EncryptData(string data)
        {
            return Convert.ToBase64String(
                ProtectedData.Protect(
                    Encoding.UTF8.GetBytes(data), null, DataProtectionScope.LocalMachine));
        }

        private static string DecryptData(string data)
        {
            return Encoding.UTF8.GetString(
                ProtectedData.Unprotect(
                    Convert.FromBase64String(data), null, DataProtectionScope.LocalMachine));
        }
    }
}