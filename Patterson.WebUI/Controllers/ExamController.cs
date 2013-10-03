using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Patterson.Domain.Abstract;
using Patterson.Domain.Entities;
using Patterson.WebUI.Models;

namespace Patterson.WebUI.Controllers
{
    public class ExamController : Controller
    {
        private ITestRepository repo;
        private IUserRepository urepo;

        public ExamController(ITestRepository testrepo, IUserRepository userrepo)
        {
            repo = testrepo;
            urepo = userrepo;
        }


        public ActionResult Index(int id)
        {

            return RedirectToAction("Exams");
        }

        public ActionResult Exams(int id)
        {
            var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
            var username = ticket.Name;

            var user = repo.Users.FirstOrDefault(u => u.userName == username);
            var roster = repo.RosterEntries.FirstOrDefault(r => r.StudentID == user.id && r.ClassID == id);
            var exams = repo.Exams.Where(e => e.RosterID == roster.RosterID);
            var tests = repo.Tests.Where(t => t.TestsInClasses.Any(i => i.ClassID == id));
            var testlist = tests.Where(t => exams.All(e => e.TestID != t.ID));
            //            var Names = tests.Where(t => exams.All(e => e.TestID == t.ID));

          //  BigClassHomeModel BCHModel = new BigClassHomeModel { TestToTake = testlist, ExamsTaken = exams };
            TempData["RosterID"] = roster.RosterID;
            TempData["ClassID"] = id;
            return View(testlist);
        }

        public ActionResult TakeExam(int id)
        {

                        var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
            var username = ticket.Name;

            var user = repo.Users.FirstOrDefault(u => u.userName == username);
            var test = repo.Tests.Single(t => t.ID == id);
            var Tic = repo.TestsInClasses.First(i => i.TestID == test.ID && i.Active >= 0);
            var roster = repo.RosterEntries.FirstOrDefault(r => r.ClassID == Tic.ClassID && r.StudentID == user.id);
                       TempData["RosterID"] = roster.RosterID;
            TempData["TestID"] = test.ID;
                      repo.AddExam(roster.RosterID, test.ID);
            return View(test);
        }
        [HttpPost]
        public ActionResult SubmitExam(FormCollection formValues)
        {
            int tid = Int32.Parse(TempData["TestID"].ToString());
            var test = repo.Tests.First(t => t.ID == tid);
                        int rid = Int32.Parse(TempData["RosterID"].ToString());
                        var Exam = repo.Exams.Single(e => e.RosterID == rid && e.TestID == tid);
                        

            int correct = 0;
            int count = 0;
            foreach (var q in test.Questions)
            {
                count++;
                try
                {
                    var ans = Int32.Parse(formValues[q.ID.ToString()].ToString());

                    if (q.Answers.First(a => a.ID == ans).Correctness == 1)
                    {
                        correct++;
                    }
                    repo.SubmitAnswer(Exam.ExamID, q.ID, ans);
                }
                catch
                {
                }
            }
            Exam.score = correct;
            repo.SubmitExam(Exam);
            ViewData["correct"] = correct;
            ViewData["count"] = count;
        
            TempData["RosterID"] = rid;
            return View("ExamGrades",Exam);
        }

        public ActionResult GradeDetails(int id)
        {


            var roster = repo.RosterEntries.FirstOrDefault(r => r.RosterID == id);
            var exam = repo.Exams.FirstOrDefault(e => e.RosterID == roster.RosterID);
            TempData["RosterID"] = id;
           // var grade = repo.ExamRecords.Where(g => g.ExamID == exam.ExamID);

            return View("ExamGrades", exam);
        }

        public ActionResult ExamDetails(int id)
        {


            var roster = repo.RosterEntries.FirstOrDefault(r => r.RosterID == id);
            var exam = repo.Exams.FirstOrDefault(e => e.RosterID == roster.RosterID);
            TempData["RosterID"] = id;
             var grade = repo.ExamRecords.Where(g => g.ExamID == exam.ExamID);

            return View("ExamDetails", grade);
        }
        public ActionResult ExamSchedule()
        {
            return View();

        }

        [Authorize]
        public ActionResult CreateExamGrid(string sidx, string sord, int page, int rows)
        {
            var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
            var username = ticket.Name;
            var user = repo.Users.Single(u => u.userName == username);
            var Rosters = repo.RosterEntries.Where(r => r.StudentID == user.id);

            var Exams = repo.Exams.Where(e => Rosters.Any(r => e.RosterID == r.RosterID));
            var ExamList = Exams.ToList();
            var ExamLQ = ExamList.AsQueryable();



            var jsonData = new
            {
                total = 1,
                page = page,
                records = ExamLQ.Count(),



                rows = (from n in ExamLQ
                        select new
                        {
                            i = n.ExamID,
                            cell = new string[]{
                                
                                n.Test.Identifier.ToString(),
                                n.score.ToString() ,
                                n.Test.PassingScore.ToString(),
                                "<a href=\"/Exam/GradeDetails/" + getRoster(n, user.id)+ "\">Details</a>"
                               }
                        }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult CreateTestGrid(string sidx, string sord, int page, int rows)
        {
            var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
            var username = ticket.Name;

            var user = repo.Users.FirstOrDefault(u => u.userName == username);
            var rosters = repo.RosterEntries.Where(r => r.StudentID == user.id);
            var exams = repo.Exams.Where(e => rosters.Any(r => e.RosterID == r.RosterID));
            var TiC = repo.TestsInClasses.Where(i => rosters.Any(r => i.ClassID == r.ClassID && i.Active >=0));
            var tests = repo.Tests.Where(t => TiC.Any(i => i.TestID == t.ID));
            var testlist = tests.Where(t => exams.All(e => e.TestID != t.ID));
            testlist = testlist.ToList().AsQueryable();



            var jsonData = new
            {
                total = 1,
                page = page,
                records = testlist.Count(),



                rows = (from n in testlist
                        select new
                        {
                            i = n.ID,
                            cell = new string[]{
                                
                                n.Identifier.ToString(),
                               
                                "<a href=\"/Exam/TakeExam/" + n.ID + "\">Take Exam</a>"
                               }
                        }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        private String getRoster(Exam exam, int sid)
        {
            String roster = repo.RosterEntries.FirstOrDefault(r => r.RosterID == exam.RosterID).RosterID.ToString();
            
            return roster;
        }



        public ActionResult GradeDetailsBC(int id)
        {
            var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
            var username = ticket.Name;
            var user = urepo.Users.FirstOrDefault(u => u.userName == username);

            var Class = repo.Classes.FirstOrDefault(c => c.ID == id);
            var roster = repo.RosterEntries.FirstOrDefault(r => r.ClassID == Class.ID && r.StudentID == user.id);
            var exam = repo.Exams.FirstOrDefault(e => e.RosterID == roster.RosterID);
            TempData["RosterID"] = roster.RosterID;
            // var grade = repo.ExamRecords.Where(g => g.ExamID == exam.ExamID);

            return View("ExamGrades", exam);
        }

        public ActionResult StudentCourseGrades(int id)
        {
            var roster =repo.RosterEntries.FirstOrDefault(r => r.StudentID == id && (int)Session["ClassID"] == r.ClassID);
            return RedirectToAction("GradeDetails", "Exam", roster.RosterID);


        }

        public ActionResult CGradeDetails(int id)
        {


            var roster = repo.RosterEntries.FirstOrDefault(r => r.RosterID == id);
            var exam = repo.Exams.FirstOrDefault(e => e.RosterID == roster.RosterID);
            TempData["RosterID"] = id;
            // var grade = repo.ExamRecords.Where(g => g.ExamID == exam.ExamID);

            return View("CExamGrades", exam);
        }

        public ActionResult CExamDetails(int id)
        {
            var roster = repo.RosterEntries.FirstOrDefault(r => r.RosterID == id);
            var exam = repo.Exams.FirstOrDefault(e => e.RosterID == roster.RosterID);
            TempData["RosterID"] = id;
            var grade = repo.ExamRecords.Where(g => g.ExamID == exam.ExamID);

            return View("CExamDetails", grade);
        }

    }
    
}
