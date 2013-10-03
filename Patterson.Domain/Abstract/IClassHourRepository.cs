using System.Linq;
using Patterson.Domain.Entities;

namespace Patterson.Domain.Abstract
{
    public interface IClassHourRepository
    {
        IQueryable<ClassHour> ClassHours { get; }

        void SaveClassHour(ClassHour classHour);

        void DeleteClasshour(ClassHour classHour);
    }
}
