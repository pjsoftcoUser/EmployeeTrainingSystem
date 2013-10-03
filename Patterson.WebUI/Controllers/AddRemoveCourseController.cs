using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Patterson.Domain.Abstract;
using System.Web.Security;
using Patterson.Domain.Entities;
using Patterson.WebUI.Models;
using System.IO;

namespace Patterson.WebUI.Controllers
{
    public class AddRemoveCourseController : Controller
    {
        private IAddDropRepository repository;

        public AddRemoveCourseController(IAddDropRepository addDropRepository)
        {
            repository = addDropRepository;
        }

        [Authorize(Roles = "admin, CourseManagement")]
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post), Authorize(Roles = "admin, CourseManagement")]
        public ActionResult Add(FormCollection formValues) // still needs a checkbox for self study
        {
            var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
            var username = ticket.Name;
            var title = formValues["Title"];
            var coursenumber = formValues["CourseNumber"];
            var credits = Int32.Parse(formValues["Credits"]);
            var description = formValues["Description"];
            var skillset = formValues["SkillSet"];

            repository.AddCourse(username, title, coursenumber, description, skillset, credits);
            TempData["message"] = string.Format("Course with Title of {0} has been saved.", title);
            return RedirectToAction("Edit", "AddRemoveCourse");
        }

        [Authorize(Roles = "admin, CourseManagement")]
        public ViewResult Edit()
        {  
            return View();
        }

        //Create course table
        public ActionResult courseTable(string sidx, string sord, int page, int rows)
        {
            List<Course> courseList = new List<Course>();
            courseList = repository.Courses.ToList();

            var jsonData = new
            {
                total = 1,
                page = page,
                records = courseList.Count(),

                rows = (from n in courseList
                        select new
                        {
                            i = n.CourseID,
                            cell = new string[]{
                                n.CourseID.ToString(),
                                n.CourseNumber,
                                n.Title,
                                n.Active.ToString(),
                                n.SelfStudy.ToString() ,
                                n.CreatedBy,
                                n.CreatedOn.ToString(),
                                n.ModifiedBy,
                                n.ModifiedOn.ToString(),
                                "<a href=\"/AddRemoveCourse/EditCourse/" + n.CourseID.ToString() + "\">" + "Edit Course" + "</a>",
                                "<a href=\"/AddRemoveCourse/CourseMaterials/" + n.CourseID.ToString() + "\">" + "Default Course Materials" + "</a>"
                                 }
                        }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);

        }

        [Authorize(Roles = "admin, CourseManagement")]
        public ViewResult EditCourse(int id)
        {
            var course = repository.GetCourse(id);
            return View("EditCourse", course);
        }

        [AcceptVerbs(HttpVerbs.Post), Authorize(Roles = "admin, CourseManagement")]
        public ActionResult Edit(Course course)
        {
            if (ModelState.IsValid)
            {

                var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
                var username = ticket.Name;
                //                var user = repository.Users.SingleOrDefault(u => u.userName == username);

                course.ModifiedBy = username;
                course.ModifiedOn = DateTime.Now;

                repository.EditCourse(course);
            }
            TempData["message"] = string.Format("Course with ID {0} has been saved.", course.CourseID);
            return RedirectToAction("Edit", "AddRemoveCourse");
        }



        [Authorize(Roles = "admin, CourseManagement")]
        public ActionResult DeleteCourse(int id)
        {
            var course = repository.GetCourse(id);
            repository.DeleteCourse(course);
            repository.SaveChanges();
            TempData["message"] = string.Format("Course with ID {0} was deleted.", course.CourseID);
            return RedirectToAction("Edit");
        }

        [Authorize]
        public ActionResult CreateFileGrid(string sidx, string sord, int page, int rows)
        {
            // -2 for class id just to be sure to only get default, course specific materials.
            List<FileUpload> FileList = repository.getFileList(-2, (int)Session["CourseID"]);
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
                                "<a href=\"/AddRemoveCourse/Download/" + n.fileID.ToString() + "\">Download file</a>"
                               }
                        }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult CourseMaterials(int id)
        {


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
                //adding to a specific course only, so class id is set to -1 for this file.

                int result = repository.UploadFile(fileContent, filename, mimeType, -1, id);


                reader.Close();
                fileStream.Close();
                // fileStream.Read(fileContent, 0, length);
            }


            Session["CourseID"] = id;
            return View();
        }

        public ActionResult Download(int id)
        {


            FileUpload file = repository.GetFile(id);
            byte[] contents = file.fileContent;

            return File(contents, System.Net.Mime.MediaTypeNames.Application.Octet, file.FileName);

        }
    }
}
