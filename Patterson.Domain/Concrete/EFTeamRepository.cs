using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Patterson.Domain.Abstract;
using Patterson.Domain.Entities;

namespace Patterson.Domain.Concrete
{
    public class EFTeamRepository : ITeamRepository
    {
        private EFDbContext context = new EFDbContext();

        public IQueryable<Team> Teams
        {
            get { return context.Teams; }
        }

        public IQueryable<TeamRosterEntry> TeamRosterEntries
        {
            get { return context.TeamRosterEntries; }
        }

        public IQueryable<User> Users
        {
            get { return context.Users; }
        }

        public User GetUser(int id)
        {
            return context.Users.SingleOrDefault(d => d.id == id);
        }

        public Team GetTeam(int id)
        {
            return context.Teams.SingleOrDefault(d => d.TeamID == id);
        }

        public int AddTeam(string nIdentifier, string nTitle, string nDescription, int nManagerID, int nCreatedByID, int nModifiedByID, string nCreatedByName, string nManagerName)
        {
            var newTeam = new Team
            {
                Identifier = nIdentifier,
                Title = nTitle,
                Description = nDescription,
                ManagerID = nManagerID,
                CreatedByID = nCreatedByID,
                CreatedOn = DateTime.Now,
                ModifiedByID = nModifiedByID,
                ModifiedOn = DateTime.Now,
                CreatedByName = nCreatedByName,
                ManagerName = nManagerName
            };

            context.Teams.Add(newTeam);
            var result = context.SaveChanges();
            return result;
        }

        public void AddTeamRoster(int uid, int tid)
        {
            var newTeamMember = new TeamRosterEntry { UserID = uid, TeamID = tid };

            context.TeamRosterEntries.Add(newTeamMember);
            context.SaveChanges();
        }

        public void DropTeamRoster(TeamRosterEntry teamMember)
        {
            TeamRosterEntry temp = context.TeamRosterEntries.FirstOrDefault(u => (u.UserID == teamMember.UserID) && (u.TeamID == teamMember.TeamID));

            context.TeamRosterEntries.Remove(temp);
            context.SaveChanges();
        }

        //returns null if the person is not on the team.
        public TeamRosterEntry TestTeamRoster(TeamRosterEntry teamMember)
        {
            TeamRosterEntry temp = context.TeamRosterEntries.FirstOrDefault(u => (u.UserID == teamMember.UserID) && (u.TeamID == teamMember.TeamID));
            return temp;
        }

        public IQueryable<TeamRosterEntry> GetTeamMembers(int tid)
        {
            return from TeamRosterEntry in context.TeamRosterEntries
                   where TeamRosterEntry.TeamID == tid
                   select TeamRosterEntry;
        }

        public List<User> GetTeamUsers(List<TeamRosterEntry> TeamMembers)
        {
            List<User> TeamUsers = new List<User>();
            for (int i = 0; i < TeamMembers.Count(); i++)
            {
                TeamUsers.Add(GetUser(TeamMembers[i].UserID));
            }
            return TeamUsers;
        }

        public int EditTeam(Team team)
        {
            context.Entry(team).State = System.Data.EntityState.Modified;
            var result = context.SaveChanges();
            return result;
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }
    }
}
