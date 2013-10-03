using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Patterson.Domain.Abstract;
using Patterson.Domain.Entities;

namespace Patterson.Domain.Concrete
{
    public class EFAttendanceRepository : IAttendanceRepository
    {
        private EFDbContext context = new EFDbContext();

        //List
        public IQueryable<Attendance> Attendances
        {
            get { return context.Attendances; }
        }

        //Get
        public Attendance GetAttendances (int id)
        {
            return context.Attendances.FirstOrDefault(d => d.StudentId == id);
        }

        //Add
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

        //Delete
        public void DeleteAttendance(Attendance attendance)
        {
            context.Attendances.Remove(attendance);
            context.SaveChanges();
        }
    }
}
