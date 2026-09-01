using Microsoft.AspNetCore.Mvc;
using online_corse.Models;
using System.Diagnostics;

namespace online_corse.Controllers
{
    public class HomeController : Controller
    {
        private OnlineCoursesContext context { get; set; }

        public HomeController(OnlineCoursesContext ctx)
        {
            context = ctx;
        }

        public IActionResult Index()
        {
            var featured = (from c in context.Corses
                            orderby c.CorseName
                            select c).Take(6).ToList();
            return View(featured);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            string requestId;
            if (Activity.Current != null && Activity.Current.Id != null)
            {
                requestId = Activity.Current.Id;
            }
            else
            {
                requestId = HttpContext.TraceIdentifier;
            }
            return View(new ErrorViewModel { RequestId = requestId });
        }
    }
}
