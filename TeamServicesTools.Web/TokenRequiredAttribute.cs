using System;
using System.Web.Mvc;
using TeamServicesTools.Web.Services;

namespace TeamServicesTools.Web
{
    public class TokenRequiredAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (SettingsService.IsConfigured) return;

            var redirectUrl = filterContext.HttpContext.Request.RawUrl;

            filterContext.RequestContext.HttpContext.Session["Message"] = "Please configure settings before proceeding";

            filterContext.Result = new RedirectResult($"/Settings/Edit?redirectUrl={Uri.EscapeUriString(redirectUrl)}");
        }

    }
}
