using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Patterson.Domain.Entities;

namespace Patterson.Domain.Abstract
{
    public interface ITeamRepository
    {
        IQueryable<Team> Teams { get; }
        IQueryable<TeamRosterEntry> TeamRosterEntries { get; }
        IQueryable<User> Users { get; }
        User GetUser(int id);
        Team GetTeam(int id);
        int AddTeam(string nIdentifier, string nTitle, string nDescription, int nManagerID, int nCreatedByID, int nModifiedByID, string nCreatedByName, string ManagerName);
        void DropTeamRoster(TeamRosterEntry teamMember);
        void AddTeamRoster(int uid, int tid);
        TeamRosterEntry TestTeamRoster(TeamRosterEntry teamMember);
        IQueryable<TeamRosterEntry> GetTeamMembers(int tid);
        List<User> GetTeamUsers(List<TeamRosterEntry> TeamMembers);
        int EditTeam(Team team);
        void SaveChanges();

    }
}
