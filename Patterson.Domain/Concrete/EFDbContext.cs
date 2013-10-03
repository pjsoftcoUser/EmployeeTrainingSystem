using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using Patterson.Domain.Entities;

namespace Patterson.Domain.Concrete
{
   public class EFDbContext : DbContext
    {
       public DbSet<Attendance> Attendances { get; set; }
       public DbSet<Course> Courses { get; set; }
       public DbSet<Class> Classes { get; set; }
       public DbSet<RosterEntry> Roster { get; set; }
       public DbSet<AlternateRosterEntry> AlternateRoster { get; set; }
       public DbSet<User> Users { get; set; }
       public DbSet<ClassHour> ClassHours { get; set; }
       public DbSet<Team> Teams { get; set; }
       public DbSet<TeamRosterEntry> TeamRosterEntries { get; set; }
       public DbSet<Test> Tests { get; set; }
       public DbSet<Question> Questions { get; set; }
       public DbSet<Answer> Answers { get; set; }
       public DbSet<TestsInClass> TestsInClasses { get; set; }
       public DbSet<Exam> Exams { get; set; }
       public DbSet<ExamRecord> ExamRecords { get; set; }
       public DbSet<InstructorRosterEntry> InstructorRosterEntries { get; set; }
       public DbSet<FileUpload> FileUploads { get; set; }
       public DbSet<SMTP> Smtp { get; set; }
       public DbSet<Skillset> Skillsets { get; set; }
       public DbSet<Minor> Minors { get; set; }
       public DbSet<SkillsetRosterEntry> SkillsetRosterEntries { get; set; }
       public DbSet<MinorRosterEntry> MinorRosterEntries { get; set; }

    }
}
