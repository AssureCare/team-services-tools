using System;
using System.Web.Mvc;
using TeamServicesTools.Web.Services;

namespace TeamServicesTools.Web
{
    /// <summary>
    /// Indicates that an authentication token is required to use the controller action.
    /// </summary>
    // ReSharper disable once InheritdocConsiderUsage
    public class TokenRequiredAttribute : ActionFilterAttribute
    {
        /// <inheritdoc />
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
