using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Patterson.Domain.Abstract;
using Patterson.Domain.Entities;
using System.Web.Security;

namespace Patterson.WebUI.Controllers
{
    [Authorize]
    public class AddDropClassController : Controller
    {
        private IAddDropRepository repository;

        public AddDropClassController(IAddDropRepository addDropRepository)
        {
            repository = addDropRepository;
        }

        [Authorize]
        public ViewResult Index()
        {
            return View();
        }

        [Authorize]
        public ViewResult Table()
        {
            return View("Table");
        }

        [Authorize]
        public ViewResult ClassHour(int id)
        {
            var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
            var username = ticket.Name;
            var user = repository.Users.SingleOrDefault(u => u.userName == User.Identity.Name);

            var Sid = user.id;
            var ClassIDList = repository.GetEnrolledClasses(Sid);
            var ClassList = repository.GetCurrentClasses(repository.GetClassList(ClassIDList.ToList())).AsQueryable();
            Class thisClass = repository.GetClass(id);
            var test = ClassList.FirstOrDefault(c => c.CourseID == thisClass.CourseID);
            var AlternateTest = repository.AlternateRoster.SingleOrDefault(u => u.class_id == id && u.user_id == Sid);

            Session["addDisabled"] = "";
            Session["addText"] = "Register for this Class as an Alternate";
            Session["addMethod"] = "AddAlternate";

            if (thisClass.CurrentEnrolled < thisClass.SizeLimit || test != null)
            {
                if (test != null)// already enrolled, so disable button
                {
                    Session["addDisabled"] = "disabled";
                    Session["addText"] = "You have already registered for this Course.";
                }

                else
                    Session["addText"] = "Add this Class to my Schedule";

                Session["addMethod"] = "Add";

            }
            else if ((thisClass.CurrentEnrolled >= thisClass.SizeLimit) && AlternateTest != null)
            {
                Session["addDisabled"] = "disabled";
                Session["addText"] = "You have already registered as an Alternate for this Class.";
            }

            Session["ClassID"] = id;
            return View("ClassHour");
        }

        [Authorize]
        public ActionResult CreateClassHourTable(string sidx, string sord, int page, int rows)
        {
            int id = (int)Session["ClassID"];

            Class curClass = repository.GetClass(id);
            IEnumerable<ClassHour> tempClassHours = curClass.ClassHours;
            var jsonData = new
            {
                total = 1,
                page = page,
                record = tempClassHours.Count(),

                rows = (from n in tempClassHours
                        select new
                        {
                            i = n.classId,
                            cell = new string[]{
                                n.day,
                                n.startTime.ToString(), 
                                n.endTime.ToString(), 
                                "<a href=\"/AddDropClass/Add/" + n.classId.ToString() + "\">Add Class</a>"
                                 }

                        }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ViewResult Classes(int id)
        {
            Session["CourseID"] = id;
            return View("Classes");
        }


        [Authorize]
        public ViewResult DropTable()
        {
            return View("DropTable");
        }

        [Authorize]
        public ActionResult CreateDropGrid(string sidx, string sord, int page, int rows)
        {
            var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
            var username = ticket.Name;
            var student = repository.Users.SingleOrDefault(u => u.userName == username);

            long studentID = student.id;

            var ClassIDList = repository.GetEnrolledClasses(studentID).ToList();
            List<Class> ClassList = repository.GetClassList(ClassIDList);
            List<Class> EnrolledClassList = repository.GetCurrentClasses(ClassList);

            var qEnrolledClassList = EnrolledClassList.AsQueryable();

            var jsonData = new
            {
                total = 1,
                page = page,
                records = EnrolledClassList.Count(),

                rows = (from n in qEnrolledClassList
                        select new
                        {
                            i = n.ID,
                            cell = new string[]{
                                n.ID.ToString(),
                                n.Course.Title.ToString(),
                                n.Instructor.ToString(),
                                n.StartDate.ToString("d"), 
                                n.EndDate.ToString("d"),
                                n.ID.ToString()
                                 }
                        }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        // used to create the jqgrid table of courses to add
        [Authorize]
        public ActionResult CreateCourseTable(string sidx, string sord, int page, int rows)
        {
            var CourseList = repository.Courses.ToList();
            var qCourseList = CourseList.AsQueryable();

            var jsonData = new
            {
                total = 1,
                page = page,
                records = CourseList.Count(),

                rows = (from n in qCourseList
                        select new
                        {
                            i = n.CourseID,
                            cell = new string[]{
                                "<a href=\"/AddDropClass/Classes/" + n.CourseID.ToString() + "\">View Classes</a>",
                                n.Title,
                                n.CourseID.ToString(),
                                n.Credits.ToString(), 
                                n.SkillSet
                                 }
                        }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        // used to create the jqgrid table of classes to add
        [Authorize]
        public ActionResult CreateClassTable(string sidx, string sord, int page, int rows)
        {

            int id = (int)Session["CourseID"];


            Course course = repository.GetCourse(id);
            IEnumerable<Class> tempClasses = course.Classes.Where(c => (c.EndDate > DateTime.Now) && (c.OpenRegistration == true) );

            var jsonData = new
            {
                total = 1,
                page = page,
                records = tempClasses.Count(),

                rows = (from n in tempClasses
                        select new
                        {
                            i = n.CourseID,
                            cell = new string[]{
                                
                                "<a href=\"/AddDropClass/ClassHour/" + n.ID.ToString() + "\">View sessions</a>",
                                n.Instructor,
                                n.SizeLimit.ToString(),
                                (n.SizeLimit - n.CurrentEnrolled).ToString(),
                                n.StartDate.ToString("d"), 
                                n.EndDate.ToString("d")                                
                                 }

                        }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult Add(int id)
        {

            var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
            var username = ticket.Name;
            var user = repository.Users.SingleOrDefault(u => u.userName == username);

            var Sid = user.id;

            // find the course id of the class to be added, and compare it against the courses
            // the user has already been enrolled in.
            Class addedClass = repository.GetClass(id);
            int courseID = addedClass.CourseID;
            var ClassIDList = repository.GetEnrolledClasses(Sid);
            var ClassList = repository.GetCurrentClasses(repository.GetClassList(ClassIDList.ToList())).AsQueryable();

            var test = ClassList.SingleOrDefault(c => c.CourseID == courseID);

            //If user isn't already enrolled in the course and a spot is open, add them to the roster.
            //OpenRegistration is also checked before adding the class to prevent users
            //from typing in the action url (~/Add/ClassId) to try to get into classes that are not open.
            if (test == null && (addedClass.CurrentEnrolled < addedClass.SizeLimit) && (addedClass.OpenRegistration == true))
            {
                var result = repository.AddRoster(id, Sid);
                repository.AdjustCurrentEnrolled(1, repository.GetClass(id));
                if(result > 0)
                    TempData["message"] = string.Format("class with ID {0} has been added to your schedule.", id);
                else
                    TempData["message"] = string.Format("Unsuccessful procedure!");
            }

            return View("Index");
        }

        [Authorize]
        public ActionResult AddAlternate(int id)
        {

            var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
            var username = ticket.Name;
            var user = repository.Users.SingleOrDefault(u => u.userName == User.Identity.Name);

            var Sid = user.id;

            // find the course id of the class to be added, and compare it against the courses
            // the user has already been enrolled in.
            Class addedClass = repository.GetClass(id);
            int courseID = addedClass.CourseID;
            var ClassIDList = repository.GetEnrolledClasses(Sid);
            var ClassList = repository.GetCurrentClasses(repository.GetClassList(ClassIDList.ToList())).AsQueryable();

            var test = ClassList.SingleOrDefault(c => c.CourseID == courseID);

            if (test == null)//if user isn't already enrolled in the course 
            {
                var result = repository.AddAlternateRoster(id, Sid);
                if (result > 0)
                    TempData["message"] = string.Format("class with ID {0} has been added as an Alternate.", id);
                else
                    TempData["message"] = string.Format("Unsuccessful procedure!");
            }
            return View("Index");
        }


        [Authorize]
        public ActionResult Drop(int id)
        {
            var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
            var username = ticket.Name;
            var student = repository.Users.SingleOrDefault(u => u.userName == User.Identity.Name);


            var roster = repository.Roster.FirstOrDefault(r => r.ClassID == id && r.StudentID == student.id);
            repository.DropRoster(roster);



            bool promote = repository.PromoteAlternate(id);

            if (promote == false)
            {
                var result = repository.AdjustCurrentEnrolled(-1, repository.GetClass(id));
                repository.SaveChanges();
                if (result > 0)
                    TempData["message"] = string.Format("class with ID {0} was dropped.", id);
                else
                    TempData["message"] = string.Format("Unsuccessful procedure!");
            }
            return RedirectToAction("DropTable");
        }
    }
}
