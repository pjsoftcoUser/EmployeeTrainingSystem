using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Patterson.Domain.Abstract;
using Patterson.Domain.Entities;
using DDay.iCal;
using DDay.iCal.Serialization;
using System.Text;
using System.IO;
using System.Net.Mail;

namespace Patterson.WebUI.Controllers
{
    public class SchedulesController : Controller
    {
        private IAddDropRepository repository;


        public SchedulesController(IAddDropRepository addDropRepository)
        {
            repository = addDropRepository;
        }
        

        public ActionResult GiveMySchedule(int id)
        {
            List<ClassHour> UserClassHours = new List<ClassHour>();
            string role = (string)Session["selectedRoles"];
            if (role.Equals("instructor"))
            {
                var InstructorClasses = repository.GetInstructorRoster(id).ToList();
                foreach (var c in InstructorClasses)
                {
                    var classHours = repository.GetClassHoursList(c.ClassID).ToList();
                    foreach (var h in classHours)
                    {
                        UserClassHours.Add(h);
                    }
                }
            }
            else if (role.Equals("student"))
            {
                var StudentClasses = repository.GetEnrolledClasses(id).ToList();
                foreach (var c in StudentClasses)
                {
                    var classHours = repository.GetClassHoursList(c.ClassID).ToList();
                    foreach (var h in classHours)
                    {
                        UserClassHours.Add(h);
                    }
                }
            }

            DDay.iCal.iCalendar iCal = new DDay.iCal.iCalendar();

            // Create the event, and add it to the iCalendar
            
            foreach(var h in UserClassHours)
            {
                StringBuilder des = new StringBuilder();
                // Set information about the event
                Event evt = iCal.Create<Event>();
                //evt.Start.Add(h.startTime);
                
                // 
                IRecurrencePattern pattern = new RecurrencePattern();
                pattern.Frequency = FrequencyType.Weekly; // Weekly
                pattern.Interval = 1; // Every 1
                pattern.ByDay.Add(new WeekDay(h.day)); // 
                pattern.Until = h.Class.EndDate;
                iCalDateTime Date = new iCalDateTime(h.Class.StartDate);
                
                evt.RecurrenceRules.Add(pattern);
                evt.Start = Date.AddHours(h.startTime.Hours);
                evt.End = Date.AddHours(h.endTime.Hours);
                des.Append(" -Session ID: " + h.id);
                des.Append(" -Istructor: " + h.Class.Instructor);
                des.Append(" -Course Name: " + h.Class.Course.Title);             
                evt.Description = des.ToString();
                evt.Location = h.location;
                evt.Summary = "Class Event";
            }
            // Create a serialization context and serializer factory.
            // These will be used to build the serializer for our object.
            ISerializationContext ctx = new SerializationContext();
            ISerializerFactory factory = new DDay.iCal.Serialization.iCalendar.SerializerFactory();
            // Get a serializer for our object
            IStringSerializer serializer = factory.Build(iCal.GetType(), ctx) as IStringSerializer;

            string output = serializer.SerializeToString(iCal);
            var contentType = "text/calendar";
            var bytes = Encoding.UTF8.GetBytes(output);

            return File(bytes, contentType, "MySchedule.ics");
        }

        public ActionResult EmailMySchedule(int id)
        {
            List<ClassHour> UserClassHours = new List<ClassHour>();
            string role = (string)Session["selectedRoles"];
            if (role.Equals("instructor"))
            {
                var InstructorClasses = repository.GetInstructorRoster(id).ToList();
                foreach (var c in InstructorClasses)
                {
                    var classHours = repository.GetClassHoursList(c.ClassID).ToList();
                    foreach (var h in classHours)
                    {
                        UserClassHours.Add(h);
                    }
                }
            }
            else if (role.Equals("student"))
            {
                var StudentClasses = repository.GetEnrolledClasses(id).ToList();
                foreach (var c in StudentClasses)
                {
                    var classHours = repository.GetClassHoursList(c.ClassID).ToList();
                    foreach (var h in classHours)
                    {
                        UserClassHours.Add(h);
                    }
                }
            }

            DDay.iCal.iCalendar iCal = new DDay.iCal.iCalendar();

            // Create the event, and add it to the iCalendar

            foreach (var h in UserClassHours)
            {
                StringBuilder des = new StringBuilder();
                // Set information about the event
                Event evt = iCal.Create<Event>();
                //evt.Start.Add(h.startTime);

                // 
                IRecurrencePattern pattern = new RecurrencePattern();
                pattern.Frequency = FrequencyType.Weekly; // Weekly
                pattern.Interval = 1; // Every 1
                pattern.ByDay.Add(new WeekDay(h.day)); // 
                pattern.Until = h.Class.EndDate;
                iCalDateTime Date = new iCalDateTime(h.Class.StartDate);

                evt.RecurrenceRules.Add(pattern);
                evt.Start = Date.AddHours(h.startTime.Hours);
                evt.End = Date.AddHours(h.endTime.Hours);
                des.Append(" -Session ID: " + h.id);
                des.Append(" -Istructor: " + h.Class.Instructor);
                des.Append(" -Course Name: " + h.Class.Course.Title);
                evt.Description = des.ToString();
                evt.Location = h.location;
                evt.Summary = "Class Event";
            }
            // Create a serialization context and serializer factory.
            // These will be used to build the serializer for our object.
            ISerializationContext ctx = new SerializationContext();
            ISerializerFactory factory = new DDay.iCal.Serialization.iCalendar.SerializerFactory();
            // Get a serializer for our object
            IStringSerializer serializer = factory.Build(iCal.GetType(), ctx) as IStringSerializer;

            string output = serializer.SerializeToString(iCal);
            var contentType = "text/calendar";
            var bytes = Encoding.UTF8.GetBytes(output);
            MemoryStream stream = new MemoryStream(bytes);
            
            var MySmtp = repository.Smtp.First();
            var user = repository.Users.FirstOrDefault(u => u.userName == User.Identity.Name);
            // send notification email through gmail
            // email address "pattersontrainingprogram@gmail.com"
            // password "P@ttersonetsemail" (Patterson employee training system email)
            System.Net.Mail.MailMessage email = new System.Net.Mail.MailMessage();
            email.To.Add(user.email);
            email.Subject = "Your schedule from P.E.T.S";
            var fromAddress = new MailAddress(MySmtp.user);
            email.From = fromAddress;
            email.Body = "This is the schedule for " + user.name + " that has been requested, Thank you.";
            System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(stream, "MySchedule.ics", contentType);
            email.Attachments.Add(attachment);
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(MySmtp.server);
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential(fromAddress.Address, MySmtp.password);
            smtp.EnableSsl = true;
            smtp.Port = MySmtp.port;
            smtp.Send(email);
            TempData["message"] = string.Format("Your schedule has been sent to {0} .", user.email);
            return RedirectToAction("Index", "Home");
        }
    }
}
