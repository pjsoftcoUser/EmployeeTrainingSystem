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
    public class AddRemoveClassController : Controller
    {

        private IAddDropRepository repository;
        string[] weekDays = new string[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" };

        public AddRemoveClassController(IAddDropRepository addDropRepository)
        {
            repository = addDropRepository;
        }

        [Authorize(Roles = "admin, instructor, ClassManagement")]
        public ActionResult Index()
        {
            List<Course> courseList = repository.Courses.ToList();
            TempData["courses"] = courseList;
            //ViewData["classhour"] = new ClassHour();
            return View();
        }

        [Authorize(Roles = "admin, instructor, ClassManagement")]
        public ActionResult EditInstructors(int id)
        {
            Session["ClassID"] = id;
            return View("EditInstructors");
        }

        public ActionResult UserList()
        {
            return View("UserList");
        }

        //Create course table
        public ActionResult courseTable(string sidx, string sord, int page, int rows)
        {
            List<Course> CourseList = new List<Course>();
            CourseList = repository.Courses.ToList();

            var jsonData = new
            {
                total = 1,
                page = page,
                records = CourseList.Count(),

                rows = (from n in CourseList
                        select new
                        {
                            i = n.CourseID,
                            cell = new string[]{
                                n.CourseID.ToString(),
                                n.CourseNumber,
                                n.Title,
                                "<a href=\"/AddRemoveClass/ClassList/" + n.CourseID.ToString() + "\">" + "View/Edit Classes" + "</a>",
                                n.Active.ToString()
                                 }
                        }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);

        }



        [Authorize(Roles = "admin, instructor, ClassManagement")]
        public ActionResult ClassList(int id)
        {
            Session["CourseID"] = id;
            Course tmpCourse = repository.GetCourse(id);
            TempData["course"] = tmpCourse.Title;
            Session["ADClCourseID"] = id;
            return View("ClassList");
        }

        //Create Class table
        public ActionResult classTable(string sidx, string sord, int page, int rows)
        {

            int id = (int)Session["ADClCourseID"];

            Course course = repository.GetCourse(id);
            IEnumerable<Class> tempClasses = course.Classes;
            var qtempClasses = tempClasses.AsQueryable();

            var jsonData = new
            {
                total = 1,
                page = page,
                records = tempClasses.Count(),

                rows = (from n in qtempClasses
                        select new
                        {
                            i = n.ID,
                            cell = new string[]{
                                n.ID.ToString(),
                                n.Title,
                                n.StartDate.ToString(),
                                n.EndDate.ToString(),
                                n.CreatedBy,
                                n.CreatedOn.ToString(),
                                n.ModifiedBy,
                                n.ModifiedOn.ToString(),
                                "<a href=\"/AddRemoveClass/EditInstructors/" + n.ID.ToString() + "\">" + "Edit Instructors" + "</a>",
                                "<a href=\"/AddRemoveClass/EditClass/" + n.ID.ToString() + "\">" + "Edit Class" + "</a>",
                                "<a href=\"/AddRemoveClass/ClassHourList/" + n.ID.ToString() + "\">Edit sessions</a>",
                                n.ID.ToString(),
                                "<a href=\"/Test/ByClass/" + n.ID.ToString() +"\">Tests</a>",
                                 }
                        }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);

        }

        [AcceptVerbs(HttpVerbs.Post), Authorize(Roles = "admin, instructor")]
        public ActionResult Add(FormCollection formValues)
        {
            var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
            var username = ticket.Name;

            var courseID = Int32.Parse(formValues["coursedropdown"]);
            var sizeLimit = Int32.Parse(formValues["SizeLimit"]);
            var instructor = formValues["Instructor"];
            var startDate = DateTime.Parse(formValues["StartDate"]);
            var endDate = DateTime.Parse(formValues["EndDate"]);

            var Title = formValues["ClassTitle"];
            var Description = formValues["Description"];
            var location = formValues["location"];
            var OpenRegistration = formValues["OpenRegistration"];

            var tempclass = repository.AddClass(username, courseID, instructor, sizeLimit, startDate,
                                                endDate, Title, Description, location, OpenRegistration);
            // tempclass will be null if instructor wasn't a real person
            // in this case neither the class or instructor will be added.
            if (tempclass.ID != 0)
                TempData["message"] = string.Format("class with ID {0} has been saved.", tempclass.ID);
            else
                TempData["message"] = string.Format("Unsuccessful procedure!");
            return RedirectToAction("Edit", "AddRemoveClass");
        }

        //add classhour get
        [Authorize(Roles = "admin, instructor, ClassManagement")]
        public ActionResult AddClassHour(int id)
        {
            TempData["weekDays"] = new SelectList(weekDays, "M");
            ClassHour newClassHour = new ClassHour();
            newClassHour.classId = id;
            return View(newClassHour);
        }

        //list classhour
        //Get
        [Authorize(Roles = "admin, instructor, ClassManagement")]
        public ActionResult ClassHourList(int id)
        {
            Session["ClassID"] = id;
            return View("ClassHourList");
        }

        //Create Classhour table
        public ActionResult classHourTable(string sidx, string sord, int page, int rows)
        {
            string url = Request.UrlReferrer.ToString();
            string[] urlsplit = url.Split('/');
            string urlID = urlsplit[urlsplit.Count() - 1];
            int id = Convert.ToInt32(urlID);

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
                            i = n.id,
                            cell = new string[]{
                                n.id.ToString(),
                                n.classId.ToString(),
                                n.day.ToString(),
                                n.startTime.ToString(),
                                n.endTime.ToString(),
                                n.location.ToString(),
                                n.CreatedBy,
                                n.CreatedOn.ToString(),
                                n.ModifiedBy,
                                n.ModifiedOn.ToString(),
                                "<a href=\"/AddRemoveClass/EditClassHour/" + n.id.ToString() + "\">Edit session</a>",
                                n.id.ToString(),
                                 }
                        }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);

        }

        //edit classhour 
        //Get
        [Authorize(Roles = "admin, instructor, ClassManagement")]
        public ActionResult EditClassHour(int id)
        {

            ClassHour curClassHour = repository.ClassHours.FirstOrDefault(c => c.id == id);
            TempData["weekDays"] = new SelectList(weekDays, curClassHour.day);
            return View(curClassHour);
        }

        //add classhour post
        [AcceptVerbs(HttpVerbs.Post), Authorize(Roles = "admin, instructor")]
        public ActionResult AddClassHour(ClassHour classhour, FormCollection formValues)
        {
            classhour.day = formValues["selectedDay"];
            classhour.ModifiedBy = User.Identity.Name;
            classhour.ModifiedOn = DateTime.Now;
            var result = repository.SaveClassHour(classhour);
            if (result > 0)
                TempData["message"] = string.Format("Session with ID {0} has been saved.", classhour.id);
            else
                TempData["message"] = string.Format("Unsuccessful procedure!");

            return RedirectToAction("ClassHourList/" + Session["ClassID"]);
        }

        //create class hour
        [AcceptVerbs(HttpVerbs.Post), Authorize(Roles = "admin, instructor")]
        public ActionResult CreateClassHour(ClassHour classhour, FormCollection formValues)
        {
            classhour.id = 0;
            classhour.day = formValues["selectedDay"];
            classhour.CreatedBy = User.Identity.Name;
            classhour.CreatedOn = DateTime.Now;
            var result = repository.SaveClassHour(classhour);
            if (result > 0)
                TempData["message"] = string.Format("Session with ID {0} has been saved.", classhour.id);
            else
                TempData["message"] = string.Format("Unsuccessful procedure!");
            return RedirectToAction("ClassHourList/" + Session["ClassID"]);
        }

        [Authorize(Roles = "admin, instructor, ClassManagement")]
        public ActionResult DeleteClassHour(int? id)
        {
            ClassHour curClassHour = repository.ClassHours.FirstOrDefault(c => c.id == id);
            if (curClassHour != null)
            {
                var result = repository.DeleteClasshour(curClassHour);
                if (result > 0)
                    TempData["message"] = string.Format("Session with ID {0} was deleted", curClassHour.id);
                else
                    TempData["message"] = string.Format("Unsuccessful procedure!");
            }
            return RedirectToAction("ClassHourList/" + Session["ClassID"]);
        }

        [Authorize(Roles = "admin, instructor, ClassManagement")]
        public ActionResult Edit()
        {
            var courses = repository.Courses;
            return View(courses);
        }

        [Authorize(Roles = "admin, instructor, ClassManagement")]
        public ActionResult EditClass(int id)
        {
            var tempClass = repository.GetClass(id);
            return View(tempClass);
        }

        [AcceptVerbs(HttpVerbs.Post), Authorize(Roles = "admin, instructor, ClassManagement")]
        public ActionResult Edit(Class newClass)
        {
            var result = repository.EditClass(newClass);
            if (result > 0)
                TempData["message"] = string.Format("class with ID {0} has been saved.", newClass.ID);
            else
                TempData["message"] = string.Format("Unsuccessful procedure!");

            return RedirectToAction("ClassList/" + Session["CourseID"]);
        }

        [Authorize(Roles = "admin, instructor, ClassManagement")]
        public ActionResult DeleteClass(int id)
        {
            var rClass = repository.Classes.FirstOrDefault(c => c.ID == id);
            repository.DeleteClass(rClass);
            var result = repository.SaveChanges();
            if (result > 0)
                TempData["message"] = string.Format("Class with ID {0} was deleted.", rClass.ID);
            else
                TempData["message"] = string.Format("Unsuccessful procedure!");

            return RedirectToAction("Edit");
        }

        [Authorize(Roles = "admin, instructor, ClassManagement")]
        public ActionResult CreateInstructorList(string sidx, string sord, int page, int rows)
        {

            int cid = (int)Session["ClassID"];

            var Instructors = repository.GetClassInstructors(cid);
            var UserList = repository.GetClassInstructorsList(Instructors.ToList());
            var qUserList = UserList.AsQueryable();

            var jsonData = new
            {
                total = 1,
                page = page,
                records = qUserList.Count(),
                rows = (from n in qUserList
                        select new
                        {
                            i = n.id,
                            cell = new string[]{
                                 "<a href=\"../DropInstructor/" + n.id.ToString() + "\">Remove Instructor</a>",
                                n.id.ToString(),
                                n.userName.ToString()
                               }
                        }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "admin, instructor, ClassManagement")]
        public ActionResult DropInstructor(int id)
        {
            int cid = (int)Session["ClassID"];

            int userID = id;

            InstructorRosterEntry temp = new InstructorRosterEntry { InstuctorID = userID, ClassID = cid };
            repository.DropInstructorRoster(temp);
            repository.SaveChanges();
            TempData["message"] = string.Format("User {0} has been dropped from Class {1}.", temp.InstuctorID, temp.ClassID);
            return RedirectToAction("EditInstructors/" + cid.ToString());

        }

        [Authorize(Roles = "admin, instructor, ClassManagement")]
        public ActionResult AddNewInstructor(FormCollection formValues)
        {
            int cid = (int)Session["ClassID"];

            var uid = Int32.Parse(formValues["UserID"]);

            // avoid duplicate entries
            InstructorRosterEntry temp = new InstructorRosterEntry { InstuctorID = uid, ClassID = cid };
            if (repository.TestInstructorRoster(temp) == null)
                repository.AddInstructor(cid, uid);
            TempData["message"] = string.Format("User {0} has been added to the Instructors of class {1}.", temp.InstuctorID, temp.ClassID);
            return RedirectToAction("EditInstructors/" + cid.ToString());
        }

         [Authorize(Roles = "admin, instructor, ClassManagement")]
        public ActionResult AddNewInstructorByID(int id)
        {
            int cid = (int)Session["ClassID"];

            var uid = id;

            // avoid duplicate entries
            InstructorRosterEntry temp = new InstructorRosterEntry { InstuctorID = uid, ClassID = cid };
            if (repository.TestInstructorRoster(temp) == null)
                repository.AddInstructor(cid, uid);
            TempData["message"] = string.Format("User {0} has been added to the Instructors of class {1}.", temp.InstuctorID, temp.ClassID);
            return RedirectToAction("EditInstructors/" + cid.ToString());
        }

        [Authorize(Roles = "admin, instructor, ClassManagement")]
        public ActionResult CreateUserList(string sidx, string sord, int page, int rows)
        {
            var users = repository.Users.ToList();
            var qUsers = users.AsQueryable();

            var jsonData = new
            {
                total = 1,
                page = page,
                records = qUsers.Count(),
                rows = (from n in qUsers
                        select new
                        {
                            i = n.id,
                            cell = new string[]{
                                n.id.ToString(),
                                n.userName,
                                 "<a href=\"AddNewInstructorByID/" + n.id.ToString() + "\">Add Instructor</a>"
                               }
                        }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

    }
}
