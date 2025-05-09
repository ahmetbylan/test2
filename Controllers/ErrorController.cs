using Microsoft.AspNetCore.Mvc;

namespace B2BUygulamasi.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult NotFound()
        {
            Response.StatusCode = 404;
            return View();
        }

        public IActionResult ServerError()
        {
            Response.StatusCode = 500;
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}