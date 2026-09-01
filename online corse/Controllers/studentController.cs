using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using online_corse.Models;

namespace online_corse.Controllers
{
    public class StudentController : Controller
    {
        private OnlineCoursesContext context { get; set; }

        public StudentController(OnlineCoursesContext ctx)
        {
            context = ctx;
        }

        bool In()
        {
            if (HttpContext.Session.GetInt32("stid") != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        bool Admin()
        {
            if (HttpContext.Session.GetString("email") == "aaazeezeh@gmail.com")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public IActionResult Index()
        {
            if (!Admin())
            {
                return RedirectToAction("Login");
            }
            return View(context.Students.Include("Corse").ToList());
        }

        public IActionResult search(string searchKey)
        {
            if (!Admin())
            {
                return RedirectToAction("Login");
            }
            if (searchKey == null)
            {
                searchKey = "";
            }
            var Students = from s in context.Students.Include("Corse")
                           where s.Name.Contains(searchKey) || s.Email.Contains(searchKey)
                           select s;
            return View("Index", Students.ToList());
        }

        public IActionResult Delete(int id)
        {
            if (!Admin())
            {
                return RedirectToAction("Login");
            }
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
            ViewBag.Corses = (from s in context.Corses
                              orderby s.CorseName
                              select s).ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Add(Student s)
        {
            bool emailTaken = (from x in context.Students
                               where x.Email == s.Email
                               select x).Any();
            if (s.Email == "aaazeezeh@gmail.com" || emailTaken)
            {
                ViewBag.Corses = (from c in context.Corses
                                  orderby c.CorseName
                                  select c).ToList();
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
            if (In())
            {
                return View("Welcom", HttpContext.Session.GetString("name"));
            }
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string stpass)
        {
            if (email == null)
            {
                email = "";
            }
            email = email.Trim();
            if (stpass == null)
            {
                stpass = "";
            }
            Student s = (from x in context.Students
                         where x.Email == email && x.Password == stpass
                         select x).FirstOrDefault();
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
            if (!In())
            {
                return RedirectToAction("Login");
            }
            int? studentID = HttpContext.Session.GetInt32("stid");
            Student S = (from x in context.Students.Include("Corse")
                         where x.StudentId == studentID
                         select x).FirstOrDefault();
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
            if (!In())
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(string newPass)
        {
            if (!In())
            {
                return RedirectToAction("Login");
            }
            if (string.IsNullOrWhiteSpace(newPass))
            {
                ViewBag.Error = "Enter a new password.";
                return View();
            }
            int? sessionId = HttpContext.Session.GetInt32("stid");
            int id = sessionId.Value;
            Student S = (from x in context.Students
                         where x.StudentId == id
                         select x).FirstOrDefault();
            if (S == null)
            {
                return RedirectToAction("Login");
            }
            S.Password = newPass.Trim();
            context.SaveChanges();
            return RedirectToAction("sDetails");
        }

        public IActionResult Courses(string searchKey)
        {
            IQueryable<Corse> list;
            if (string.IsNullOrWhiteSpace(searchKey))
            {
                list = context.Corses;
            }
            else
            {
                list = from c in context.Corses
                       where c.CorseName.Contains(searchKey) || c.Instructor.Contains(searchKey)
                       select c;
            }

            if (In())
            {
                Student enrolledStudent = context.Students.Find(HttpContext.Session.GetInt32("stid"));
                if (enrolledStudent != null)
                {
                    ViewBag.Enrolled = enrolledStudent.CorseId;
                }
                else
                {
                    ViewBag.Enrolled = null;
                }
            }
            else
            {
                ViewBag.Enrolled = 0;
            }

            ViewBag.Admin = Admin();
            var ordered = from c in list
                          orderby c.CorseName
                          select c;
            return View(ordered.ToList());
        }

        [HttpGet]
        public IActionResult AddCourse()
        {
            if (Admin())
            {
                return View("CourseForm", new Corse());
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpGet]
        public IActionResult EditCourse(int id)
        {
            if (Admin())
            {
                return View("CourseForm", context.Corses.Find(id));
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        public IActionResult SaveCourse(Corse c)
        {
            if (!Admin())
            {
                return RedirectToAction("Login");
            }
            if (c.CorseId == 0)
            {
                context.Corses.Add(c);
            }
            else
            {
                context.Corses.Update(c);
            }
            context.SaveChanges();
            return RedirectToAction("Courses");
        }

        public IActionResult DeleteCourse(int id)
        {
            if (!Admin())
            {
                return RedirectToAction("Login");
            }
            Corse other = (from x in context.Corses
                           where x.CorseId != id
                           select x).FirstOrDefault();
            if (other == null)
            {
                return RedirectToAction("Courses");
            }
            var enrolled = from s in context.Students
                           where s.CorseId == id
                           select s;
            foreach (var s in enrolled)
            {
                s.CorseId = other.CorseId;
            }
            Corse c = context.Corses.Find(id);
            if (c != null)
            {
                context.Corses.Remove(c);
                context.SaveChanges();
            }
            return RedirectToAction("Courses");
        }

        public IActionResult Enroll(int id)
        {
            if (!In())
            {
                return RedirectToAction("Login");
            }
            Student S = context.Students.Find(HttpContext.Session.GetInt32("stid"));
            S.CorseId = id;
            context.SaveChanges();
            return RedirectToAction("sDetails");
        }
    }
}
