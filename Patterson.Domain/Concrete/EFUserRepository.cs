using System.Linq;
using Patterson.Domain.Abstract;
using Patterson.Domain.Entities;
using System.Data.Entity;

namespace Patterson.Domain.Concrete
{
    public class EFUserRepository : IUserRepository
    {
        private EFDbContext context = new EFDbContext();

        public IQueryable<User> Users
        {
            get { return context.Users; }
        }

        public int SaveUser(User user)
        {
            
            if (user.id == 0)
            {
                context.Users.Add(user);
            }
            else
            {
                context.Entry(user).State = System.Data.EntityState.Modified;
            }
            var result = context.SaveChanges();
            return result;
        }

        public int DeleteUser(User user)
        {
            context.Users.Remove(user);
            var result = context.SaveChanges();
            return result;
        }
    }
}
