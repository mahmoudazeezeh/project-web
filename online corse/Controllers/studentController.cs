using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using online_corse.Models;

namespace online_corse.Controllers
{
    public class StudentController : Controller
    {
        private OnlineCoursesContext context { get; set; }
        public StudentController(OnlineCoursesContext ctx) => context = ctx;

        bool In() => HttpContext.Session.GetInt32("stid") != null;
        bool Admin() => HttpContext.Session.GetString("email") == "aaazeezeh@gmail.com";

        public IActionResult Index()
        {
            if (!Admin()) return RedirectToAction("Login");
            return View(context.Students.Include(s => s.Corse).ToList());
        }

        public IActionResult search(string searchKey)
        {
            if (!Admin()) return RedirectToAction("Login");
            searchKey ??= "";
            var Students = context.Students.Include(s => s.Corse)
                .Where(s => s.Name.Contains(searchKey) || s.Email.Contains(searchKey)).ToList();
            return View("Index", Students);
        }

        public IActionResult Delete(int id)
        {
            if (!Admin()) return RedirectToAction("Login");
            Student st = context.Students.Find(id);
            if (st != null && st.Email != "aaazeezeh@gmail.com")
            {
                context.Students.Remove(st);
                context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Add()
        {
            ViewBag.Corses = context.Corses.OrderBy(s => s.CorseName).ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Add(Student s)
        {
            if (s.Email == "aaazeezeh@gmail.com" || context.Students.Any(x => x.Email == s.Email))
            {
                ViewBag.Corses = context.Corses.OrderBy(c => c.CorseName).ToList();
                ViewBag.Error = "This email cannot be used.";
                return View();
            }
            context.Students.Add(s);
            context.SaveChanges();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (In()) return View("Welcom", HttpContext.Session.GetString("name"));
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string stpass)
        {
            Student s = context.Students.FirstOrDefault(x => x.Email == email && x.Password == stpass);
            if (s != null)
            {
                HttpContext.Session.SetInt32("stid", s.StudentId);
                HttpContext.Session.SetString("name", s.Name);
                HttpContext.Session.SetString("email", s.Email);
                return View("Welcom", s.Name);
            }
            ViewBag.Error = "Wrong email or password.";
            return View();
        }

        public IActionResult sDetails()
        {
            if (!In()) return RedirectToAction("Login");
            int? studentID = HttpContext.Session.GetInt32("stid");
            Student S = context.Students.Include(x => x.Corse).FirstOrDefault(x => x.StudentId == studentID);
            return View(S);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            if (!In()) return RedirectToAction("Login");
            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(string newPass)
        {
            if (!In()) return RedirectToAction("Login");
            Student S = context.Students.Find(HttpContext.Session.GetInt32("stid"));
            S.Password = newPass;
            context.SaveChanges();
            return RedirectToAction("sDetails");
        }

        public IActionResult Courses(string searchKey)
        {
            var list = string.IsNullOrWhiteSpace(searchKey) ? context.Corses
                : context.Corses.Where(c => c.CorseName.Contains(searchKey) || c.Instructor.Contains(searchKey));
            ViewBag.Enrolled = In() ? context.Students.Find(HttpContext.Session.GetInt32("stid"))?.CorseId : 0;
            ViewBag.Admin = Admin();
            return View(list.OrderBy(c => c.CorseName).ToList());
        }

        [HttpGet]
        public IActionResult AddCourse() => Admin() ? View("CourseForm", new Corse()) : RedirectToAction("Login");

        [HttpGet]
        public IActionResult EditCourse(int id) => Admin() ? View("CourseForm", context.Corses.Find(id)) : RedirectToAction("Login");

        [HttpPost]
        public IActionResult SaveCourse(Corse c)
        {
            if (!Admin()) return RedirectToAction("Login");
            if (c.CorseId == 0) context.Corses.Add(c); else context.Corses.Update(c);
            context.SaveChanges();
            return RedirectToAction("Courses");
        }

        public IActionResult DeleteCourse(int id)
        {
            if (!Admin()) return RedirectToAction("Login");
            Corse other = context.Corses.FirstOrDefault(x => x.CorseId != id);
            if (other == null) return RedirectToAction("Courses");
            foreach (var s in context.Students.Where(s => s.CorseId == id))
                s.CorseId = other.CorseId;
            Corse c = context.Corses.Find(id);
            if (c != null) { context.Corses.Remove(c); context.SaveChanges(); }
            return RedirectToAction("Courses");
        }

        public IActionResult Enroll(int id)
        {
            if (!In()) return RedirectToAction("Login");
            Student S = context.Students.Find(HttpContext.Session.GetInt32("stid"));
            S.CorseId = id;
            context.SaveChanges();
            return RedirectToAction("sDetails");
        }
    }
}
