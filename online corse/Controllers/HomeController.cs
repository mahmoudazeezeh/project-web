using Microsoft.AspNetCore.Mvc;
using online_corse.Models;
using System.Diagnostics;

namespace online_corse.Controllers
{
    public class HomeController : Controller
    {
        private OnlineCoursesContext context { get; set; }
        public HomeController(OnlineCoursesContext ctx) => context = ctx;

        public IActionResult Index()
        {
            return View(context.Corses.OrderBy(c => c.CorseName).Take(6).ToList());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
