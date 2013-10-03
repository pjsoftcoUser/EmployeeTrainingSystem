using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Patterson.Domain.Abstract;
using Patterson.Domain.Entities;

namespace Patterson.Domain.Concrete
{
    public class EFSMTPRepository : ISMTPRepository
    {
        private EFDbContext context = new EFDbContext();

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
