using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace B2BUygulamasi.TagHelpers
{
    [HtmlTargetElement("a", Attributes = "active-controller, active-action")]
    public class ActiveMenuTagHelper : TagHelper
    {
        [HtmlAttributeName("active-controller")]
        public string ActiveController { get; set; }

        [HtmlAttributeName("active-action")]
        public string ActiveAction { get; set; }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var currentController = ViewContext.RouteData.Values["controller"]?.ToString();
            var currentAction = ViewContext.RouteData.Values["action"]?.ToString();

            var isActive = string.Equals(ActiveController, currentController, StringComparison.OrdinalIgnoreCase) &&
                          string.Equals(ActiveAction, currentAction, StringComparison.OrdinalIgnoreCase);

            var existingClass = output.Attributes["class"]?.Value?.ToString();
            var newClass = string.IsNullOrEmpty(existingClass)
                ? "nav-link"
                : existingClass;

            if (isActive)
            {
                newClass += " active";
                output.Attributes.SetAttribute("aria-current", "page");
            }

            output.Attributes.SetAttribute("class", newClass);
        }
    }
}