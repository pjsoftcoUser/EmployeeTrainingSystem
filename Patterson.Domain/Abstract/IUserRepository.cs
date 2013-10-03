using System.Linq;
using Patterson.Domain.Entities;

namespace Patterson.Domain.Abstract
{
    public interface IUserRepository
    {
        IQueryable<User> Users { get; }

        int SaveUser(User user);

        int DeleteUser(User user);
    }
}
