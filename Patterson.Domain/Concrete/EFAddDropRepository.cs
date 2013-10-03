using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Patterson.Domain.Abstract;
using Patterson.Domain.Entities;
using System.Net.Mail;

namespace Patterson.Domain.Concrete
{
    public class EFAddDropRepository : IAddDropRepository
    {
        private EFDbContext context = new EFDbContext();
        private ISMTPRepository repository;

        public EFAddDropRepository(ISMTPRepository repo)
        {
            repository = repo;
        }

        //Courses
        public IQueryable<Course> Courses
        {
            get { return context.Courses; }
        }

        //Classes
        public IQueryable<Class> Classes
        {
            get { return context.Classes; }
        }

        //Roster
        public IQueryable<RosterEntry> Roster
        {
            get { return context.Roster; }
        }

        //Roster
        public IQueryable<AlternateRosterEntry> AlternateRoster
        {
            get { return context.AlternateRoster; }
        }

        //Instructor Roster
        public IQueryable<InstructorRosterEntry> InstructorRoster
        {
            get { return context.InstructorRosterEntries; }
        }

        //Users
        public IQueryable<User> Users
        {
            get { return context.Users; }
        }

        //Class Hour
        public IQueryable<ClassHour> ClassHours
        {
            get { return context.ClassHours; }
        }

        //Attendance
        public IQueryable<Attendance> Attendances
        {
            get { return context.Attendances; }
        }

        //Gets
        public Course GetCourse(int id)
        {
            return context.Courses.SingleOrDefault(d => d.CourseID == id);

        }

        public Class GetClass(int id)
        {
            return context.Classes.SingleOrDefault(d => d.ID == id);
        }

        public User GetUser(int id)
        {
            return context.Users.SingleOrDefault(d => d.id == id);
        }

        public ClassHour GetClassHours(int id)
        {
            return context.ClassHours.SingleOrDefault(d => d.id == id);
        }

        public Attendance GetAttendances(int id)
        {
            return context.Attendances.FirstOrDefault(d => d.StudentId == id);
        }

        //add class hour
        public int SaveClassHour(ClassHour classHour)
        {
            if (classHour.id == 0)
            {
                context.ClassHours.Add(classHour);
            }
            else
            {
                context.Entry(classHour).State = System.Data.EntityState.Modified;
            }
            var result = context.SaveChanges();
            return result;
        }

        //Add attendance
        public void SaveAttendance(Attendance attendance)
        {
            if (attendance.id == 0)
            {
                context.Attendances.Add(attendance);
            }
            else
            {
                context.Entry(attendance).State = System.Data.EntityState.Modified;
            }
            context.SaveChanges();
        }

        //Delete attendance
        public void DeleteAttendance(Attendance attendance)
        {
            context.Attendances.Remove(attendance);
            context.SaveChanges();
        }

        //delete class hour
        public int DeleteClasshour(ClassHour classHour)
        {
            context.ClassHours.Remove(classHour);
            var result = context.SaveChanges();
            return result;
        }

        public int AddRoster(int cid, int sid)
        {
            //      Class tempClass = Classes.SingleOrDefault(c => c.ID == cid);
            // Users.SingleOrDefault(s => s.id == sid).Classes.Add(tempClass);

            var newRoster = new RosterEntry { ClassID = cid, StudentID = sid };

            var tempRoster = context.Roster;
            context.Roster.Add(newRoster);
            var result = context.SaveChanges();
            return result;
        }

        public void AddInstructor(int cid, int uid)
        {
            var newInstructor = new InstructorRosterEntry { InstuctorID = uid, ClassID = cid };

            context.InstructorRosterEntries.Add(newInstructor);
            context.SaveChanges();
        }

        public void DropRoster(RosterEntry student)
        {
            context.Roster.Remove(student);
        }

        public void DropInstructorRoster(InstructorRosterEntry Instructor)
        {
            InstructorRosterEntry i = context.InstructorRosterEntries.FirstOrDefault((e => (e.ClassID == Instructor.ClassID) && (e.InstuctorID == Instructor.InstuctorID)));
            context.InstructorRosterEntries.Remove(i);
        }

        public void AddCourse(string userName, string title, string coursenumber, string description, string skillset, int credits)
        {
            var newCourse = new Course { Title = title, SkillSet = skillset, Credits = credits, CreatedBy = userName, CreatedOn = DateTime.Now, Active = true, SelfStudy = false, Description = description, CourseNumber = coursenumber };
            // var tempRoster = context.Roster;
            context.Courses.Add(newCourse);
            context.SaveChanges();
        }

        public void EditCourse(Course course)
        {
            context.Entry(course).State = System.Data.EntityState.Modified;
            context.SaveChanges();
        }
        public void DeleteCourse(Course course)
        {
            foreach (var classes in course.Classes.ToList())
            {
                DeleteClass(classes);
            }
            context.Courses.Remove(course);

        }

        public Class AddClass(string username, int courseid, string instructor, int sizelimit, DateTime startDate, DateTime endDate, string title, string description, string location, string openRegistration)
        {
            var newClass = new Class
            {
                CourseID = courseid,
                Instructor = instructor,
                SizeLimit = sizelimit,
                StartDate = startDate,
                EndDate = endDate,
                CreatedBy = username,
                CreatedOn = DateTime.Now,
                Title = title,
                Description = description,
                location = location
            };

            newClass.CourseNumber = GetCourse(newClass.CourseID).CourseNumber;

            if (openRegistration.Equals("true,false"))
                newClass.OpenRegistration = true;

            if (title.Equals(""))
                newClass.Title = GetCourse(newClass.CourseID).Title;
            if (description.Equals(""))
                newClass.Description = GetCourse(newClass.CourseID).Description;
            if ((location.Equals("")) && (GetCourse(newClass.CourseID).SelfStudy == true))
                newClass.location = "virtual";


            User newInstructor = context.Users.SingleOrDefault(u => u.userName == instructor);

            if (newInstructor != null) //if the given Instructor is a real user
            {
                context.Classes.Add(newClass);
                context.SaveChanges();

                // get the id of the class that was just inserted.
                int classID = (from n in context.Classes
                               orderby n.ID descending
                               select n.ID).FirstOrDefault();

                int InstructorID = newInstructor.id;
                AddInstructor(classID, InstructorID);
            }
            return newClass; // will return null if the instructor's name was not found
        }


        public int EditClass(Class newClass)
        {
            User newInstructor = context.Users.SingleOrDefault(u => u.userName == newClass.Instructor);

            if (newInstructor != null) //if the given Instructor is a real user
            {
                context.Entry(newClass).State = System.Data.EntityState.Modified;


                // get the id of the class that was just inserted.
                int classID = (from n in context.Classes
                               orderby n.ID descending
                               select n.ID).FirstOrDefault();

                int instructorID = newInstructor.id;
                // only add instructor if they aren't already the instructor.
                if (context.InstructorRosterEntries.SingleOrDefault(n => n.InstuctorID == instructorID && n.ClassID == classID) == null)
                    AddInstructor(classID, instructorID);

            }
            var result = context.SaveChanges();
            return result;
        }

        public void DeleteClass(Class rClass)
        {
            foreach (var student in rClass.roster.ToList())
            {
                DropRoster(student);
            }

            foreach (var instructor in rClass.InstructorRosterEntries.ToList())
            {
                DropInstructorRoster(instructor);
            }

            foreach (var ClassHour in rClass.ClassHours.ToList())
            {
                DeleteClasshour(ClassHour);
            }

            foreach (var test in rClass.TestsInClasses.ToList())
            {
                context.TestsInClasses.Remove(test);
            }

            context.Classes.Remove(rClass);
        }

        public int SaveChanges()
        {
            var result = context.SaveChanges();
            return result;
        }

        //returns the Attendances for a specific Student ID
        public IQueryable<Attendance> GetAttendancesList(int id)
        {
            return from Attendance in context.Attendances
                   where Attendance.StudentId == id
                   select Attendance;
        }

        //returns the Classhours for a specific Class ID
        public IQueryable<ClassHour> GetClassHoursList(int ID)
        {
            return from ClassHour in context.ClassHours
                   where ClassHour.classId == ID
                   select ClassHour;
        }

        //returns the RosterEntries for a specific Student ID
        public IQueryable<RosterEntry> GetEnrolledClasses(long ID)
        {
            return from RosterEntry in context.Roster
                   where RosterEntry.StudentID == ID
                   select RosterEntry;
        }

        // get the roster of a specific class
        public IQueryable<RosterEntry> GetRoster(int id)
        {
            return from RosterEntry in context.Roster
                   where RosterEntry.ClassID == id
                   select RosterEntry;
        }

        // get the classes for a specific instructor
        public IQueryable<InstructorRosterEntry> GetInstructorRoster(int id)
        {
            return from InstructorRosterEntry in context.InstructorRosterEntries
                   where InstructorRosterEntry.InstuctorID == id
                   select InstructorRosterEntry;
        }

        // get the instructors for a specific class
        public IQueryable<InstructorRosterEntry> GetClassInstructors(int id)
        {
            return from InstructorRosterEntry in context.InstructorRosterEntries
                   where InstructorRosterEntry.ClassID == id
                   select InstructorRosterEntry;
        }

        // get the actual users from a list of InstructorRosterEntries
        public List<User> GetClassInstructorsList(List<InstructorRosterEntry> InstructorRoster)
        {
            List<User> instructorList = new List<User>();
            foreach (InstructorRosterEntry i in InstructorRoster)
            {
                instructorList.Add(GetUser(i.InstuctorID));
            }
            return instructorList;
        }

        //Takes a list of InstructorRosterEntries and returns the corresponding (current) Classes
        public List<Class> GetInstructorClassList(List<InstructorRosterEntry> ClassList)
        {
            List<Class> TaughtList = new List<Class>();
            for (int i = 0; i < ClassList.Count(); i++)
            {
                Class tempclass = GetClass(ClassList[i].ClassID);
                if (tempclass != null)
                {
                    if (tempclass.EndDate > DateTime.Now)
                        TaughtList.Add(tempclass);
                }
            }
            return TaughtList;
        }

        // returns null if the user is not currently an instructor of this class
        public InstructorRosterEntry TestInstructorRoster(InstructorRosterEntry Instructor)
        {
            InstructorRosterEntry temp = context.InstructorRosterEntries.FirstOrDefault(u => (u.InstuctorID == Instructor.InstuctorID) && (u.ClassID == Instructor.ClassID));
            return temp;
        }

        //Takes a list of RosterEntries and returns the corresponding Classes
        public List<Class> GetClassList(List<RosterEntry> ClassList)
        {
            List<Class> EnrolledList = new List<Class>();
            for (int i = 0; i < ClassList.Count(); i++)
            {
                EnrolledList.Add(GetClass(ClassList[i].ClassID));
            }
            return EnrolledList;
        }

        public List<User> GetStudentRoster(List<RosterEntry> UserList)
        {
            List<User> StudentList = new List<User>();
            for (int i = 0; i < UserList.Count(); i++)
            {
                StudentList.Add(GetUser(UserList[i].StudentID));
            }
            return StudentList;
        }

        public List<Class> GetCurrentClasses(List<Class> EnrolledList)
        {
            List<Class> CurrentList = new List<Class>();

            foreach (Class i in EnrolledList)
            {
                if (i.EndDate > DateTime.Now)
                    CurrentList.Add(i);
            }

            return CurrentList;
        }

        public int UploadFile(byte[] filecontent, string filename, string mimetype, int classid, int courseid)
        {
            var newFile = new FileUpload
            {
                fileContent = filecontent,
                FileName = filename,
                MimeType = mimetype,
                ClassID = classid,
                CourseID = courseid
            };

            context.FileUploads.Add(newFile);
            var result = context.SaveChanges();
            return result;
        }

        public List<FileUpload> getFileList(int classId, int courseID)
        {
            List<FileUpload> FileList = new List<FileUpload>();

            foreach (FileUpload f in context.FileUploads)
            {
                if (f.CourseID == courseID)
                {
                    FileList.Add(f);
                }

                if (f.ClassID == classId)
                {
                    FileList.Add(f);
                }
            }

            return FileList;
        }

        public FileUpload GetFile(int id)
        {
            FileUpload file = context.FileUploads.FirstOrDefault(n => n.fileID == id);
            return file;
        }


        //changes the value representing number of users enrolled in a class. For use when adding/dropping classes. 
        public int AdjustCurrentEnrolled(int amount, Class thisClass)
        {
            thisClass.CurrentEnrolled = thisClass.CurrentEnrolled + amount;
            var result = context.SaveChanges();
            return result;
        }

        public int AddAlternateRoster(int cid, int uid)
        {
            var newAlternateRoster = new AlternateRosterEntry { user_id = uid, class_id = cid };
            context.AlternateRoster.Add(newAlternateRoster);
            var result = context.SaveChanges();
            return result;
        }

        public void DropAlternateRoster(AlternateRosterEntry student)
        {
            context.AlternateRoster.Remove(student);
        }

        public bool PromoteAlternate(int classID)
        {
            if ((context.AlternateRoster.SingleOrDefault(u => u.class_id == classID)) == null)
                return false;
            else
            {

                int entryID = context.AlternateRoster.Where(q => q.class_id == classID).Min(q => q.ID);

                AlternateRosterEntry entry = context.AlternateRoster.SingleOrDefault(u => u.ID == entryID);

                var newRoster = new RosterEntry { ClassID = entry.class_id, StudentID = entry.user_id };
                context.Roster.Add(newRoster);
                DropAlternateRoster(entry);

                Class tempclass = GetClass(classID);
                //delete entries for this student for other classes within the course
                foreach (AlternateRosterEntry a in context.AlternateRoster)
                {
                    Class tempclass2 = GetClass(a.class_id);
                    if ((a.user_id == entry.user_id) && (tempclass2.CourseID == tempclass.CourseID))
                        DropAlternateRoster(a);
                }

                context.SaveChanges();

                var MySmtp = repository.Smtp.First();
                // send notification email through gmail
                // email address "pattersontrainingprogram@gmail.com"
                // password "P@ttersonetsemail" (Patterson employee training system email)
                System.Net.Mail.MailMessage email = new System.Net.Mail.MailMessage();
                email.To.Add(GetUser(entry.user_id).email);
                email.Subject = "Alternate Promotion";
                var fromAddress = new MailAddress(MySmtp.user);
                email.From = fromAddress;
                email.Body = "This is an automated message from 'Patterson Employee Training System' to inform you that you have been promoted from an alternate " +
                "attendee in " + tempclass.Course.Title + ", and are now officially registered as a student for this course.";

                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(MySmtp.server);
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential(fromAddress.Address, MySmtp.password);
                smtp.EnableSsl = true;
                smtp.Port = MySmtp.port;

                smtp.Send(email);
                return true;

            }
        }
        //SMTP repository
        public IQueryable<SMTP> Smtp
        {
            get { return context.Smtp; }
        }

        public int SaveSMTP(SMTP smtp)
        {
            if (smtp.SMTPid == 0)
            {
                context.Smtp.Add(smtp);
            }
            else
            {
                context.Entry(smtp).State = System.Data.EntityState.Modified;
            }
            var result = context.SaveChanges();
            return result;
        }
    }
}
