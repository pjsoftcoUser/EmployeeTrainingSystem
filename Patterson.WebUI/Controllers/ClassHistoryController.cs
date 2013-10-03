using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Patterson.Domain.Abstract;
using Patterson.Domain.Entities;
using System.Web.Security;


// Note: Link to grade detail goes nowhere since we have no grades to display yet.


namespace Patterson.WebUI.Controllers
{
    public class ClassHistoryController : Controller
    {
        private IAddDropRepository repository;

        public ClassHistoryController(IAddDropRepository addDropRepository)
        {
            repository = addDropRepository;
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
            List<Class> EnrolledClassList = repository.GetClassList(ClassIDList);

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
                            cell = new string[]
                            {
                                n.Course.Title,
                                n.CourseID.ToString(), 
                                n.Instructor.ToString(),
                                n.StartDate.ToString("d"), 
                                n.EndDate.ToString("d"), 
                                "<a href=\"/Exam/GradeDetailsBC/" + n.ID.ToString() + "\">Grade Detail</a>",
                            }
                        }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

    }
}

