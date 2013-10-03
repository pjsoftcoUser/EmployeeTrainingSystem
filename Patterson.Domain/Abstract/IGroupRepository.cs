using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Patterson.Domain.Entities;

namespace Patterson.Domain.Abstract
{
    public interface IGroupRepository
    {
        IQueryable<Team> Teams { get; }
        IQueryable<Skillset> Skillsets { get; }
        IQueryable<Minor> Minors { get; }

        IQueryable<TeamRosterEntry> TeamRosterEntries { get; }
        IQueryable<User> Users { get; }
        
        User GetUser(int id);
        Team GetTeam(int id);
        Skillset GetSkillset(int id);
        Minor GetMinor(int id);

        int AddTeam(string nIdentifier, string nTitle, string nDescription, int nManagerID, int nCreatedByID, int nModifiedByID, string nCreatedByName, string ManagerName);
        int AddMinor(string nIdentifier, string nTitle, string nDescription, int nCreatedBy);
        int AddSkillset(string nIdentifier, string nTitle, string nDescription, int nCreatedBy);
       
        void DropTeamRoster(TeamRosterEntry teamMember);
        void DropSkillsetRoster(SkillsetRosterEntry SkillsetUser);
        void DropMinorRoster(MinorRosterEntry MinorUser);

        void AddTeamRoster(int uid, int tid);
        int AddSkillsetRoster(int uid, int sid);
        void AddMinorRoster(int uid, int mid);

        TeamRosterEntry TestTeamRoster(TeamRosterEntry teamMember);
        SkillsetRosterEntry TestSkillsetRoster(SkillsetRosterEntry SkillsetMember);
        MinorRosterEntry TestMinorRoster(MinorRosterEntry MinorMember);

        IQueryable<TeamRosterEntry> GetTeamMembers(int tid);
        IQueryable<SkillsetRosterEntry> GetSkillsetMembers(int sid);
        IQueryable<MinorRosterEntry> GetMinorMembers(int mid);

        List<User> GetTeamUsers(List<TeamRosterEntry> TeamMembers);
        List<User> GetSkillsetUsers(List<SkillsetRosterEntry> SkillsetMembers);
        List<User> GetMinorUsers(List<MinorRosterEntry> MinorMembers);
        
        int EditTeam(Team team);
        int EditMinor(Minor minor);
        int EditSkillset(Skillset skillset);
        
        void SaveChanges();

    }
}
