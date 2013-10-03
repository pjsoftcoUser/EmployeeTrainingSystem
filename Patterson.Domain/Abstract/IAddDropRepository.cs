using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Patterson.Domain.Entities;

namespace Patterson.Domain.Abstract
{
    public interface IAddDropRepository
    {
        IQueryable<Course> Courses { get; }
        // IQueryable<Student> Students { get; }
        IQueryable<Class> Classes { get; }
        IQueryable<User> Users { get; }
        IQueryable<RosterEntry> Roster { get; }
        IQueryable<AlternateRosterEntry> AlternateRoster { get; }
        IQueryable<ClassHour> ClassHours { get; }
        IQueryable<Attendance> Attendances { get; }
        IQueryable<InstructorRosterEntry> InstructorRoster { get; }

        Course GetCourse(int id);
        Class GetClass(int id);
        ClassHour GetClassHours(int id);
        Attendance GetAttendances(int id);
        User GetUser(int id);

        //Roster
        int AddRoster(int cid, int sid);
        void DropRoster(RosterEntry student);
        void AddInstructor(int cid, int uid);
        void DropInstructorRoster(InstructorRosterEntry Instructor);
        InstructorRosterEntry TestInstructorRoster(InstructorRosterEntry Instructor);
        int AdjustCurrentEnrolled(int amount, Class thisClass);
        int AddAlternateRoster(int cid, int sid);
        void DropAlternateRoster(AlternateRosterEntry student);
        bool PromoteAlternate(int classID);
        //Course
        void AddCourse(string userName, string title, string coursenumber, string description, string skillset, int credits);
        void EditCourse(Course course);
        void DeleteCourse(Course course);
        //Class
        Class AddClass(string username, int courseid, string instructor, int sizelimit, DateTime startDate,
                        DateTime endDate, string title, string description, string location, string openRegistration);
        int EditClass(Class newClass);
        void DeleteClass(Class rClass);
        //Class Hour
        int SaveClassHour(ClassHour classHour);
        int DeleteClasshour(ClassHour classHour);

        //Attendance
        void SaveAttendance(Attendance attendance);
        void DeleteAttendance(Attendance attendance);

        //Uploaded files
        int UploadFile(byte[] filecontent, string filename, string mimetype, int classid, int courseid);
        FileUpload GetFile(int id);
        List<FileUpload> getFileList(int classId, int courseID);

        int SaveChanges();

        //SMTP
        IQueryable<SMTP> Smtp { get; }
        int SaveSMTP(SMTP smtp);

        IQueryable<RosterEntry> GetEnrolledClasses(long ID);
        IQueryable<RosterEntry> GetRoster(int id);
        IQueryable<InstructorRosterEntry> GetInstructorRoster(int id);
        IQueryable<InstructorRosterEntry> GetClassInstructors(int id);
        IQueryable<ClassHour> GetClassHoursList(int ID);
        IQueryable<Attendance> GetAttendancesList(int id);
        List<User> GetClassInstructorsList(List<InstructorRosterEntry> InstructorRoster);
        List<Class> GetInstructorClassList(List<InstructorRosterEntry> ClassList);
        List<Class> GetClassList(List<RosterEntry> ClassList);
        List<User> GetStudentRoster(List<RosterEntry> UserList);
        List<Class> GetCurrentClasses(List<Class> EnrolledList);

    }
}
