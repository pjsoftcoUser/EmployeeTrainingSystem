using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Patterson.Domain.Entities;

namespace Patterson.Domain.Abstract
{
    public interface IAttendanceRepository
    {
        IQueryable<Attendance> Attendances { get; }
        Attendance GetAttendances (int id);
        void SaveAttendance(Attendance attendance);
        void DeleteAttendance(Attendance attendance);
    }
}
