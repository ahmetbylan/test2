using Microsoft.AspNetCore.Mvc.Rendering;

namespace B2BUygulamasi.Extensions
{
    public static class HtmlHelpers
    {
        public static string IsActive(this IHtmlHelper html,
                                    string controller,
                                    string action,
                                    string cssClass = "active")
        {
            var routeData = html.ViewContext.RouteData.Values;
            var currentController = routeData["controller"]?.ToString();
            var currentAction = routeData["action"]?.ToString();

            return (controller == currentController && action == currentAction)
                ? cssClass
                : string.Empty;
        }

        // İsteğe bağlı: Area desteği eklemek için
        public static string IsActiveWithArea(this IHtmlHelper html,
                                            string area,
                                            string controller,
                                            string action,
                                            string cssClass = "active")
        {
            var routeData = html.ViewContext.RouteData.Values;
            var currentArea = routeData["area"]?.ToString();
            var currentController = routeData["controller"]?.ToString();
            var currentAction = routeData["action"]?.ToString();

            return (area == currentArea &&
                   controller == currentController &&
                   action == currentAction)
                ? cssClass
                : string.Empty;
        }
    }
}