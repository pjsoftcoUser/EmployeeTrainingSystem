using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Patterson.Domain.Abstract;
using Patterson.Domain.Entities;

namespace Patterson.Domain.Concrete
{
    public class EFGroupRepository : IGroupRepository
    {
        private EFDbContext context = new EFDbContext();

        public IQueryable<Team> Teams
        {
            get { return context.Teams; }
        }

        public IQueryable<Skillset> Skillsets
        {
            get { return context.Skillsets; }
        }

        public IQueryable<Minor> Minors
        {
            get { return context.Minors; }
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

        public Skillset GetSkillset(int id)
        {
            return context.Skillsets.SingleOrDefault(d => d.ID == id);
        }

        public Minor GetMinor(int id)
        {
            return context.Minors.SingleOrDefault(d => d.ID == id);
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

        public int AddMinor(string nIdentifier, string nTitle, string nDescription, int nCreatedBy)
        {

            User creator = GetUser(nCreatedBy);
            string CreatorName = creator.userName;

            var newMinor = new Minor
            {
                Identifier = nIdentifier,
                Title = nTitle,
                Description = nDescription,
               
                CreatedBy = nCreatedBy,
                CreatedOn = DateTime.Now,
                CreatedByName = CreatorName
            };

            context.Minors.Add(newMinor);
            var result = context.SaveChanges();
            return result;
        }

        public int AddSkillset(string nIdentifier, string nTitle, string nDescription, int nCreatedBy)
        {
            User creator = GetUser(nCreatedBy);
            string CreatorName = creator.userName;

            var newSkillset = new Skillset
            {
                Identifier = nIdentifier,
                Title = nTitle,
                Description = nDescription,

                CreatedBy = nCreatedBy,
                CreatedOn = DateTime.Now,
                CreatedByName = CreatorName
            };

            context.Skillsets.Add(newSkillset);
            var result = context.SaveChanges();
            return result;
        }

        public void AddTeamRoster(int uid, int tid)
        {
            var newTeamMember = new TeamRosterEntry { UserID = uid, TeamID = tid };

            if (GetUser(uid) != null)//make sure the id number given is a real id number before continuing
            {
                context.TeamRosterEntries.Add(newTeamMember);
                context.SaveChanges();
            }
        }

        public int AddSkillsetRoster(int uid, int sid)
        {
            var returnvalue = -1;
            var newSkillsetUser = new SkillsetRosterEntry { UserID = uid, SkillsetID= sid };

            if (GetUser(uid) != null)//make sure the id number given is a real id number before continuing
            {    
                context.SkillsetRosterEntries.Add(newSkillsetUser);
                returnvalue = context.SaveChanges();
            }
            return returnvalue;
        }

        public void AddMinorRoster(int uid, int mid)
        {
            var newMinorUser = new MinorRosterEntry { UserID = uid, MinorID = mid };

            context.MinorRosterEntries.Add(newMinorUser);
            context.SaveChanges();
        }

        public void DropTeamRoster(TeamRosterEntry teamMember)
        {
            TeamRosterEntry temp = context.TeamRosterEntries.FirstOrDefault(u => (u.UserID == teamMember.UserID) && (u.TeamID == teamMember.TeamID));

            context.TeamRosterEntries.Remove(temp);
            context.SaveChanges();
        }

        public void DropSkillsetRoster(SkillsetRosterEntry SkillsetUser)
        {
            SkillsetRosterEntry temp = context.SkillsetRosterEntries.FirstOrDefault(u => (u.UserID == SkillsetUser.UserID) && (u.SkillsetID == SkillsetUser.SkillsetID));

            context.SkillsetRosterEntries.Remove(temp);
            context.SaveChanges();
        }

        public void DropMinorRoster(MinorRosterEntry MinorUser)
        {
            MinorRosterEntry temp = context.MinorRosterEntries.FirstOrDefault(u => (u.UserID == MinorUser.UserID) && (u.MinorID == MinorUser.MinorID));

            context.MinorRosterEntries.Remove(temp);
            context.SaveChanges();
        }


        //returns null if the person is not on the team, skillset, or minor.
        public TeamRosterEntry TestTeamRoster(TeamRosterEntry teamMember)
        {
            TeamRosterEntry temp = context.TeamRosterEntries.FirstOrDefault(u => (u.UserID == teamMember.UserID) && (u.TeamID == teamMember.TeamID));
            return temp;
        }

        public SkillsetRosterEntry TestSkillsetRoster(SkillsetRosterEntry SkillsetMember)
        {
            SkillsetRosterEntry temp = context.SkillsetRosterEntries.FirstOrDefault(u => (u.UserID == SkillsetMember.UserID) && (u.SkillsetID == SkillsetMember.SkillsetID));
            return temp;
        }

        public MinorRosterEntry TestMinorRoster(MinorRosterEntry MinorMember)
        {
            MinorRosterEntry temp = context.MinorRosterEntries.FirstOrDefault(u => (u.UserID == MinorMember.UserID) && (u.MinorID == MinorMember.MinorID));
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

        public IQueryable<SkillsetRosterEntry> GetSkillsetMembers(int sid)
        {
            return from SkillsetRosterEntry in context.SkillsetRosterEntries
                   where SkillsetRosterEntry.SkillsetID == sid
                   select SkillsetRosterEntry;
        }

        public List<User> GetSkillsetUsers(List<SkillsetRosterEntry> SkillsetMembers)
        {
            List<User> SkillsetUsers = new List<User>();
            for (int i = 0; i < SkillsetMembers.Count(); i++)
            {
                SkillsetUsers.Add(GetUser(SkillsetMembers[i].UserID));
            }
            return SkillsetUsers;
        }

        public IQueryable<MinorRosterEntry> GetMinorMembers(int mid)
        {
            return from MinorRosterEntry in context.MinorRosterEntries
                   where MinorRosterEntry.MinorID == mid
                   select MinorRosterEntry;
        }

        public List<User> GetMinorUsers(List<MinorRosterEntry> MinorMembers)
        {
            List<User> MinorUsers = new List<User>();
            for (int i = 0; i < MinorMembers.Count(); i++)
            {
                MinorUsers.Add(GetUser(MinorMembers[i].UserID));
            }
            return MinorUsers;
        }

        public int EditTeam(Team team)
        {
            context.Entry(team).State = System.Data.EntityState.Modified;
            var result = context.SaveChanges();
            return result;
        }

        public int EditMinor(Minor minor)
        {
            context.Entry(minor).State = System.Data.EntityState.Modified;
            var result = context.SaveChanges();
            return result;
        }

        public int EditSkillset(Skillset skillset)
        {
            context.Entry(skillset).State = System.Data.EntityState.Modified;
            var result = context.SaveChanges();
            return result;
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }
    }
}
