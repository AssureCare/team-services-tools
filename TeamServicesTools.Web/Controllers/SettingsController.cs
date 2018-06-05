using System.Web;
using System.Web.Mvc;
using TeamServicesTools.Web.Models.Settings;
using TeamServicesTools.Web.Services;

namespace TeamServicesTools.Web.Controllers
{
    public class SettingsController : Controller
    {
        private const string CookieName = "Settings";

        [TokenRequired]
        public ActionResult Index(string cookieValue)
        {
            return View(SettingsService.GetSettings());
        }

        public ActionResult Edit()
        {
            return View(SettingsService.GetSettings());
        }

        [HttpPost]
        public ActionResult Edit(SettingsModel model, string redirectUrl)
        {
            var protectedSettings = SettingsService.ProtectSettings(model);

            Response.Cookies.Add(new HttpCookie(CookieName, protectedSettings));

            ViewBag.Redirect = true;
            ViewBag.RedirectUrl = !string.IsNullOrWhiteSpace(redirectUrl) ? redirectUrl : "/Home";

            return View();
        }
    }
}
