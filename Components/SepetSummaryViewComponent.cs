using B2BUygulamasi.Services;
using Microsoft.AspNetCore.Mvc;

namespace B2BUygulamasi.Components
{
    public class SepetSummaryViewComponent : ViewComponent
    {
        private readonly ISepetService _sepetService;

        public SepetSummaryViewComponent(ISepetService sepetService)
        {
            _sepetService = sepetService;
        }

        public IViewComponentResult Invoke()
        {
            var count = _sepetService.GetSepetItemCount();
            return View(count);
        }
    }
}