using System.Linq;
using Patterson.Domain.Entities;

namespace Patterson.Domain.Abstract
{
    public interface ISMTPRepository
    {
        IQueryable<SMTP> Smtp { get; }

        int SaveSMTP(SMTP smtp);
    }
}
