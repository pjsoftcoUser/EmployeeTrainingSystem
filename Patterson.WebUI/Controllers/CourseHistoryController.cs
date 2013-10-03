using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Patterson.Domain.Abstract;
using Patterson.Domain.Entities;
using System.Web.Security;

//used to give detailed course history to admins
namespace Patterson.WebUI.Controllers
{
    public class CourseHistoryController : Controller
    {
        //
        // GET: /CourseHistory/

        private IAddDropRepository repository;

        public CourseHistoryController(IAddDropRepository addDropRepository)
        {
            repository = addDropRepository;
        }

        [Authorize(Roles = "admin, CourseManagement")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "admin, CourseManagement")]
        public ActionResult Classes(int id)
        {
            Session["ClassID"] = id;
            return View();
        }

        [Authorize(Roles = "admin, CourseManagement")]
        public ActionResult ClassHours(int id)
        {
            Session["ClassHoursID"] = id;
            return View();
        }

        [Authorize(Roles = "admin, CourseManagement")]
        public ActionResult Students(int id)
        {
            Session["ClassStudentsID"] = id;
            return View();
        }

        public ActionResult Instructors(int id)
        {
            Session["ClassInstructorsID"] = id;
            return View();
        }

        [Authorize(Roles = "Instructor, CourseManagement")]
        public ActionResult Attendance(int id)
        {
            return View();
        }

        public ActionResult CreateAttendanceTable(string sidx, string sord, int page, int rows)
        {
            string url = Request.UrlReferrer.ToString();
            string[] urlsplit = url.Split('/');
            string urlID = urlsplit[urlsplit.Count() - 1];
            int id = Convert.ToInt32(urlID);

            IEnumerable<Attendance> tmpAttendances = repository.GetAttendancesList(id);

            var jsonData = new
            {
                total = 1,
                page = page,
                records = tmpAttendances.Count(),

                rows = (from n in tmpAttendances
                        select new
                        {
                            i = n.id,
                            cell = new string[]{
                                n.StudentId.ToString(),
                                n.ClassHour.Class.Instructor.ToString(),
                                n.ClassHour.Class.Course.Title.ToString(),
                                n.ClassHour.classId.ToString(),
                                n.ClassHour.startTime.ToString(), 
                                n.ClassHour.endTime.ToString(),
                                n.AttendTime.ToString()
                                }

                        }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "admin, CourseManagement")]
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
                                "<a href=\"/CourseHistory/Classes/" + n.CourseID.ToString() + "\">View Classes</a>",
                                n.Title,
                                n.CourseID.ToString(),
                                n.Credits.ToString(), 
                                n.SkillSet
                                 }
                        }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "admin, CourseManagement")]
        public ActionResult CreateClassTable(string sidx, string sord, int page, int rows)
        {

            int id = (int)Session["ClassID"];


            Course course = repository.GetCourse(id);
            IEnumerable<Class> tempClasses = course.Classes;

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
                                n.ID.ToString(),
                                "<a href=\"/CourseHistory/ClassHours/" + n.ID.ToString() + "\">View Hours</a>",
                                "<a href=\"/CourseHistory/Students/" + n.ID.ToString() + "\">View Student Info</a>",
                                "<a href=\"/CourseHistory/Instructors/" + n.ID.ToString() + "\">View Instructor Info</a>",
                                n.SizeLimit.ToString(), 
                                n.StartDate.ToString("d"), 
                                n.EndDate.ToString("d")                                
                                 }

                        }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "admin, CourseManagement")]
        public ActionResult CreateClassHoursTable(string sidx, string sord, int page, int rows)
        {

            int id = (int)Session["ClassHoursID"];


            Class curClass = repository.GetClass(id);
            IEnumerable<ClassHour> tempClassHours = curClass.ClassHours;

            var jsonData = new
            {
                total = 1,
                page = page,
                records = tempClassHours.Count(),

                rows = (from n in tempClassHours
                        select new
                        {
                            i = n.classId,
                            cell = new string[]{
                                n.day,
                                n.startTime.ToString(), 
                                n.endTime.ToString(), 
                                 }

                        }).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "admin, CourseManagement")]
        public ActionResult CreateStudentsTable(string sidx, string sord, int page, int rows)
        {

            int id = (int)Session["ClassStudentsID"];


            List<RosterEntry> ClassRoster = repository.GetRoster(id).ToList();
            List<User> StudentList = repository.GetStudentRoster(ClassRoster);
            var qStudentList = StudentList.AsQueryable();

            var jsonData = new
            {
                total = 1,
                page = page,
                records = qStudentList.Count(),

                rows = (from n in qStudentList
                        select new
                        {
                            i = n.id,
                            cell = new string[]{
                                n.id.ToString(),
                                n.name.ToString(), 
                                n.email.ToString(), 
                                "<a href=\"/CourseHistory/GradeDetail/" + n.id.ToString() + "\">Grade Detail</a>",
                                "<a href=\"/CourseHistory/Attendance/" + n.id.ToString() + "\">Attendance Records</a>"
                                 }

                        }).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "admin, CourseManagement")]
        public ActionResult CreateInstructorsTable(string sidx, string sord, int page, int rows)
        {

            int id = (int)Session["ClassInstructorsID"];


            List<InstructorRosterEntry> Instructors = repository.GetClassInstructors(id).ToList();
            List<User> InstructorList = repository.GetClassInstructorsList(Instructors);
            var qInstructorList = InstructorList.AsQueryable();

            var jsonData = new
            {
                total = 1,
                page = page,
                records = qInstructorList.Count(),

                rows = (from n in qInstructorList
                        select new
                        {
                            i = n.id,
                            cell = new string[]{
                                n.id.ToString(),
                                n.name.ToString(), 
                                n.email.ToString() 
                                 }

                        }).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GradeDetail(int id)
        {
            int classid = (int)Session["ClassStudentsID"];
            var roster = repository.Roster.FirstOrDefault(r => r.ClassID == classid && r.StudentID == id);
            return RedirectToAction("CGradeDetails/" + roster.RosterID,"Exam" );
        }
    }
}
