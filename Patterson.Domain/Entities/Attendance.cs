using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Patterson.Domain.Entities
{
    public class Attendance
    {
        public int id { get; set; }
        public int StudentId { get; set; }
        public DateTime AttendTime { get; set; }
        public int ClassHourId { get; set; }

        public virtual ClassHour ClassHour { get; set; }


    }
}
