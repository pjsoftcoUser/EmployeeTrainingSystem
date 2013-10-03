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
    public class ClassScheduleController : Controller
    {
        private IAddDropRepository repository;
        private ITestRepository repo;

        public ClassScheduleController(IAddDropRepository addDropRepository, ITestRepository testRepository)
        {
            repository = addDropRepository;
            repo = testRepository;
        }

        [Authorize]
        public ActionResult Index()
        {
            return View("Index");
        }


        [Authorize]
        public ActionResult CreateGrid(string sidx, string sord, int page, int rows)
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
                                 "<a href=\"/ClassSchedule/CourseHome/" + n.ID.ToString() + "\">Course Home</a>",
                                n.CourseID.ToString(),
                                n.Course.Title,
                                n.Instructor.ToString(),
                                n.StartDate.ToString("d"), 
                                n.EndDate.ToString("d")
                               }
                        }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult CourseHome(int id)
        {
            Session["ClassID"] = id;
            Session["CourseID"] = repository.GetCourse(repository.GetClass(id).CourseID).CourseID;
            return View("CourseHome", id);
        }

        public ActionResult Download(int id)
        {


            FileUpload file = repository.GetFile(id);
            byte[] contents = file.fileContent;

            return File(contents, System.Net.Mime.MediaTypeNames.Application.Octet, file.FileName);

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
                                "<a href=\"/ClassSchedule/Download/" + n.fileID.ToString() + "\">Download file</a>"
                               }
                        }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


    }
}
