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
    public class TestController : Controller
    {
        private ITestRepository testRepository;

        
        public TestController(ITestRepository repo)
        {
            testRepository = repo;
        }

        [Authorize]
        public ActionResult TestManagement()
        {
            return View();
        }

        [Authorize]
        public ActionResult ByClass(int id)
        {

            //            TestsInClass tic;
            Session["ClassID"] = id;

            return View(id);
        }

        [Authorize(Roles = "admin, TestManagement")]
        public ActionResult CreateTest()
        {          
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post), Authorize(Roles = "admin, TestManagement")]
        public ActionResult newTest(FormCollection formValues)
        {

            var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
            var username = ticket.Name;
            int cid = -1;
            if (Session["ClassID"] != null)
            {
                cid = Int32.Parse(Session["ClassID"].ToString());
            }
            var Title = formValues["Title"].ToString();
            var Ident = formValues["Identifier"].ToString();
            var Pass = Int32.Parse(formValues["PassingScore"].ToString());
            var newTest = testRepository.CreateTest(Title, Ident, username, Pass);
            if (cid != 1)
            {
                var result = testRepository.AddTest(newTest.ID, cid);
                if (result > 0)
                    TempData["message"] = string.Format("Test with ID {0} has been added to class {1}.", newTest.Title, cid);
                else
                    TempData["message"] = string.Format("Unsuccessful procedure!");
            }
            TempData["TestID"] = newTest.ID;
            return RedirectToAction("CreateQuestions", new { id = newTest.ID });
        }

        [Authorize(Roles = "admin, TestManagement")]
        public ActionResult CreateQuestions(int id)
        {        
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post), Authorize(Roles = "admin, TestManagement"), ValidateInput(false)]
        public ActionResult newQuestion(FormCollection formValues)
        {
            int tid = -1;
            if (TempData["TestID"] != null)
            {
                tid = Int32.Parse(TempData["TestID"].ToString());
            }
            TempData["TestID"] = tid;
            var newQuestion = testRepository.AddQuestion(tid, formValues["QuestionText"].ToString());
            testRepository.AddAnswer(newQuestion.ID, formValues["Answer A"], formValues["ACorrect"]);
            testRepository.AddAnswer(newQuestion.ID, formValues["Answer B"], formValues["BCorrect"]);
            testRepository.AddAnswer(newQuestion.ID, formValues["Answer C"], formValues["CCorrect"]);
            testRepository.AddAnswer(newQuestion.ID, formValues["Answer D"], formValues["DCorrect"]);
            var result = testRepository.AddAnswer(newQuestion.ID, formValues["Answer E"], formValues["ECorrect"]);
            if (result > 0)
                TempData["message"] = string.Format("Question with ID {0} has been added to Test {1}.", newQuestion.ID, tid);
            else
                TempData["message"] = string.Format("Unsuccessful procedure!");
            return View("CreateQuestions");
        }

        [Authorize(Roles = "admin, TestManagement")]
        public ActionResult EditTest(int id)
        {

           var test = testRepository.Tests.SingleOrDefault(t => t.ID == id);
           TempData["TestID"] = id;

            return View(test);
        }

        [AcceptVerbs(HttpVerbs.Post), Authorize(Roles = "admin, TestManagement")]
        public ActionResult EditTest(FormCollection formValues)
        {
            var tid = Int32.Parse(TempData["TestID"].ToString());
            var test = testRepository.Tests.SingleOrDefault(t => t.ID ==tid);

            var Title = formValues["Title"].ToString();
            var Ident = formValues["Identifier"].ToString();
            var Pass = Int32.Parse(formValues["PassingScore"].ToString());
            var Time = Int32.Parse(formValues["TimeLimit"].ToString());

            test.Title = Title;
            test.Identifier = Ident;
            test.PassingScore = Pass;
            test.TimeLimit = Time;

            var result = testRepository.EditTest(test);
            if (result > 0)
                TempData["message"] = string.Format("Test with ID {0} has been saved.", test.Title);
            else
                TempData["message"] = string.Format("Unsuccessful procedure!");
            //return RedirectToAction("EditQuestions", new { id = test.ID });
            return RedirectToAction("TestManagement");
        }

        [Authorize]
        public ActionResult ViewTest(int id)
        {
            var test = testRepository.Tests.SingleOrDefault(t => t.ID == id);

            return View(test);
        }

        [Authorize]
        public ActionResult CreateTestGrid(string sidx, string sord, int page, int rows)
        {
            var classQB = testRepository.Classes;
            var classlist = classQB.ToList().AsQueryable();
            IQueryable<Test> tests;


            try
            {
                var tic = testRepository.TestsInClasses;
                tests = testRepository.Tests.Where(t => tic.Any(s => s.TestID == t.ID));

            }
            catch
            {
                tests = null;
                

            }
            tests = tests.ToList().AsQueryable();

            var jsonData = new
            {
                total = 1,
                page = page,
                records = tests.Count(),



                rows = (from n in tests
                        select new
                        {
                            i = n.ID,
                            cell = new string[]{
                                n.ID.ToString(),
                                n.Title.ToString(),
                                "<a href=\"/Test/MakeActive/" + n.ID.ToString() + "\">Make Active</a>"
                               }
                        }).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        private String ActCheck(Test n)
        {
            var tic = testRepository.TestsInClasses.FirstOrDefault(i => i.ClassID == (int)Session["ClassID"] && n.ID == i.TestID);
            if(tic.Active <= 0){
                return "<a href=\"/Test/MakeActive" + n.ID.ToString() + "\">Tests</a>";
            }else{
                return "Active";
            }
        }

        [Authorize]
        public ActionResult MakeActive(int id)
        {
            var tic = testRepository.TestsInClasses.FirstOrDefault(i => i.TestID == id);
            testRepository.MakeActive(tic, (int)Session["ClassID"]);
            return RedirectToAction("ByClass", new { id = (int)Session["ClassID"] });
        }

        [Authorize]
        public ActionResult CreateTestClassGrid(string sidx, string sord, int page, int rows)
        {
            var classQB = testRepository.Classes;
            var classlist = classQB.ToList().AsQueryable();






            var jsonData = new
            {
                total = 1,
                page = page,
                records = classlist.Count(),



                rows = (from n in classlist
                        select new
                        {
                            i = n.ID,
                            cell = new string[]{
                                n.ID.ToString(),
                                n.Title.ToString(),
                                "<a href=\"/Test/ByClass/" + n.ID.ToString() + "\">Tests</a>"
                               /* getTest(n),
                                "<a href=\"Test/NewTest/" + n.ID.ToString() + "\">Create New Test</a>",
                                getDropString(n),*/
                                
                               }
                        }).ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

         public ActionResult DropTest(int id)
         {
             return View();
         }

         public ActionResult NewTest(int id)
         {
             return View();
         }
         private String getTest(Class Class)
         {
             String value;

             try
             {
              value = Class.TestsInClasses.First(i => i.ClassID >= 0).Test.Identifier.ToString();
             }
             catch
             {
                 value = "No Test";
             }
             return value;
         }

         private String getDropString(Class n)
         {
             String value;
             try{
                 if (n.TestsInClasses.Count > 0)
                 {
                     value = "<a href=\"Test/DropTest/" + n.ID.ToString() + "\">Drop Test</a>";
                 }
                 else
                 {
                     value = "";
                 }
             }catch{
                 value = "";
             }
             return value;

         }
    }


}
