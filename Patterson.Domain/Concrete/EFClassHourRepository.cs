using System.Linq;
using Patterson.Domain.Abstract;
using Patterson.Domain.Entities;
using System.Data.Entity;

namespace Patterson.Domain.Concrete
{
    public class EFClassHourRepository : IClassHourRepository
    {
        private EFDbContext context = new EFDbContext();

        public IQueryable<ClassHour> ClassHours 
        {
            get { return context.ClassHours; }
        }

        public void SaveClassHour(ClassHour classHour)
        {
            context.ClassHours.Add(classHour);
            context.SaveChanges();
        }

        public void DeleteClasshour(ClassHour classHour)
        {
            context.ClassHours.Remove(classHour);
            context.SaveChanges();
        }
    }
}
