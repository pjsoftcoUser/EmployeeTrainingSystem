using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Patterson.Domain.Abstract;
using Patterson.Domain.Entities;
using System.Web.Security;
using System.IO;

namespace Patterson.WebUI.Controllers
{
    public class InstructorScheduleController : Controller
    {
        private IAddDropRepository repository;


        public InstructorScheduleController(IAddDropRepository addDropRepository)
        {
            repository = addDropRepository;
        }

        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult AccessDenied()
        {
            return View();
        }

        [Authorize]
        public ActionResult Roster(int id)
        {
            Session["RosterID"] = id;
            Session["ClassStudentsID"] = id;
            //make sure the user is really the instructor, so that information can't be accessed by someone typing in URLs manually that isn't the instructor
            var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
            var username = ticket.Name;
            var instructor = repository.Users.SingleOrDefault(u => u.userName == username);
            int userid = instructor.id;
            InstructorRosterEntry test = repository.InstructorRoster.FirstOrDefault(u => (u.InstuctorID == userid) && (u.ClassID == id));

            if (test != null)
                return View();
            else
                return RedirectToAction("AccessDenied");
        }


        [Authorize]
        public ActionResult CreateGrid(string sidx, string sord, int page, int rows)
        {
            var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
            var username = ticket.Name;
            var student = repository.Users.SingleOrDefault(u => u.userName == username);

            int studentID = student.id;

            var ClassIDList = repository.GetInstructorRoster(studentID).ToList();
            List<Class> InstructorClassList = repository.GetInstructorClassList(ClassIDList);


            var qInstructorClassList = InstructorClassList.AsQueryable();

            var jsonData = new
            {
                total = 1,
                page = page,
                records = InstructorClassList.Count(),

                rows = (from n in qInstructorClassList
                        select new
                        {
                            i = n.ID,
                            cell = new string[]{
                                n.Course.Title,
                                n.Course.CourseID.ToString(),
                                n.ID.ToString(),
                                n.StartDate.ToString("d"), 
                                n.EndDate.ToString("d"),
                                "<a href=\"/InstructorSchedule/Roster/" + n.ID.ToString() + "\">View Roster</a>",
                                "<a href=\"/InstructorSchedule/CourseMaterials/" + n.ID.ToString() + "\">View/Upload Course Materials</a>"
                               }
                        }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult CreateStudentsTable(string sidx, string sord, int page, int rows)
        {

            int id = (int)Session["RosterID"];


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
                                "0",
                                n.email.ToString(), 
                                "<a href=\"/CourseHistory/GradeDetail/" + n.id.ToString() + "\">Grade Detail</a>",
                                "<a href=\"/CourseHistory/Attendance/" + n.id.ToString() + "\">Attendance Records</a>"
                                 }

                        }).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        public ActionResult CourseMaterials(int id)
        {
            if (TestInstructor(id) == false)
                return RedirectToAction("AccessDenied");

            if (Request.Files.Count > 0 && Request.Files[0] != null && Request.Files[0].ContentLength > 0)
            {
                HttpPostedFileBase file = Request.Files[0];

                int length = file.ContentLength;
                Stream fileStream = file.InputStream;
                string filename = Path.GetFileName(file.FileName);
                string mimeType = file.ContentType;
                byte[] fileContent = new byte[length];
                BinaryReader reader = new BinaryReader(fileStream);
                fileContent = reader.ReadBytes((Int32)fileStream.Length);
                //adding to a specific class only, so course id is set to -1 for this file.

                int result = repository.UploadFile(fileContent, filename, mimeType, id, -1);


                reader.Close();
                fileStream.Close();
                // fileStream.Read(fileContent, 0, length);
            }

            Session["ClassID"] = id;
            Session["CourseID"] = repository.GetCourse(repository.GetClass(id).CourseID).CourseID;
            return View();
        }


        [Authorize]
        public ActionResult CreateFileGrid(string sidx, string sord, int page, int rows)
        {

            List<FileUpload> FileList = repository.getFileList((int)Session["ClassID"], (int)Session["CourseID"]);
            var qFileList = FileList.AsQueryable();


            var jsonData = new
            {
                total = 1,
                page = page,
                records = FileList.Count(),

                rows = (from n in qFileList
                        select new
                        {
                            i = n.fileID,
                            cell = new string[]{
                                n.fileID.ToString(),
                                n.FileName.ToString(),
                                n.MimeType.ToString(),
                                "<a href=\"/InstructorSchedule/Download/" + n.fileID.ToString() + "\">Download file</a>"
                               }
                        }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Download(int id)
        {
            if (TestInstructor((int)Session["ClassID"]) == false)
                return RedirectToAction("AccessDenied");

            FileUpload file = repository.GetFile(id);
            byte[] contents = file.fileContent;


            return File(contents, System.Net.Mime.MediaTypeNames.Application.Octet, file.FileName);

        }

        public bool TestInstructor(int Classid)
        {
            //make sure the user is really the instructor, so that information can't be accessed by someone typing in URLs manually that isn't the instructor
            var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
            var username = ticket.Name;
            var instructor = repository.Users.SingleOrDefault(u => u.userName == username);
            int userid = instructor.id;
            InstructorRosterEntry test = repository.InstructorRoster.FirstOrDefault(u => (u.InstuctorID == userid) && (u.ClassID == Classid));

            if (test != null)
                return true;
            else
                return false;
        }

        public String getGradeDetailString(int sid, int cid)
        {
            var roster = repository.Roster.FirstOrDefault(r => r.StudentID == sid && r.ClassID == cid);
            return "<a href=\"/Exam/GradeDetail/" + roster.RosterID.ToString() + "\">Grade Details</a>";
            

        }
    }
}
