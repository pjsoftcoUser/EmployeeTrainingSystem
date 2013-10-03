using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Patterson.Domain.Abstract;
using Patterson.Domain.Entities;

namespace Patterson.WebUI.Controllers
{
    public class StudentController : Controller
    {
        private IAddDropRepository repository;

        public StudentController(IAddDropRepository addDropRepository)
        {
            repository = addDropRepository;
        }
        //
        // GET: /Student/

        public ActionResult Index()
        {
            return View();
        }

        //mark present in attendance table for student at its time
        [Authorize(Roles = "Instructor")]
        public Boolean Present(int id)
        {
            var myAttendance = new Attendance();
            var classID = (int)Session["RosterID"];
            var ClassHours = repository.GetClassHoursList(classID);
            var curDay = Convert.ToString(DateTime.Now.DayOfWeek);
            try
            {
                var classHoursForDay = ClassHours.Where(d => d.day == curDay);
                var curClassHour = classHoursForDay.FirstOrDefault(t => t.startTime.Hours == DateTime.Now.Hour);
                myAttendance.StudentId = id;
                myAttendance.AttendTime = DateTime.Now;
                myAttendance.ClassHourId = curClassHour.id;
                repository.SaveAttendance(myAttendance);
                TempData["message"] = string.Format("Student {0} marked as present for session {1}!", id, curClassHour.id);
            }
            catch
            {
                TempData["message"] = string.Format("Student {0} is not enrolled in this session of the class!", id);
                return false;
            }
            return true;
        }

        //mark absent in attendance table for student at its time
        [Authorize(Roles = "Instructor")]
        public Boolean Absent(int id)
        {
            var myAttendance = new Attendance();
            var classID = (int)Session["RosterID"];
            var ClassHours = repository.GetClassHoursList(classID);
            var curDay = Convert.ToString(DateTime.Now.DayOfWeek);
            try
            {
                var classHoursForDay = ClassHours.Where(d => d.day == curDay);
                var curClassHour = classHoursForDay.FirstOrDefault(t => t.startTime.Hours == DateTime.Now.Hour);
                myAttendance.StudentId = id;
                myAttendance.AttendTime = DateTime.Now;
                myAttendance.ClassHourId = curClassHour.id;
                repository.DeleteAttendance(myAttendance);
                TempData["message"] = string.Format("Student {0} marked as absent for session {1}!", id, curClassHour.id);
            }
            catch
            {
                TempData["message"] = string.Format("Student {0} is not enrolled in this session of the class!", id);
                return false;
            }

            return true;
        }
    }
}
