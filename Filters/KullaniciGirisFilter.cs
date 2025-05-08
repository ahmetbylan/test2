using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace B2BUygulamasi.Filters
{
    public class KullaniciGirisFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var kullaniciID = context.HttpContext.Session.GetInt32("KullaniciID");

            if (kullaniciID == null)
            {
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary {
                        { "controller", "Account" },
                        { "action", "Login" }
                    });
            }

            base.OnActionExecuting(context);
        }
    }
}