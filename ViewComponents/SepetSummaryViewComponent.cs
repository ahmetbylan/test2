using Microsoft.AspNetCore.Mvc;
using B2BUygulamasi.Services;

namespace B2BUygulamasi.ViewComponents
{
    public class SepetSummaryViewComponent : ViewComponent
    {
        private readonly ISepetService _sepetService;

        public SepetSummaryViewComponent(ISepetService sepetService)
        {
            _sepetService = sepetService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var count = await _sepetService.GetSepetItemCountAsync();
            return View(count);
        }
    }
}
