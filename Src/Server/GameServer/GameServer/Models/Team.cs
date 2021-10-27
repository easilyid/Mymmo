using Common;
using GameServer.Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Models
{
    class Team
    {
        public int Id;
        public int TimeStamp;
        public Character Leader;
        public  List<Character> Members = new List<Character>();

        public Team(Character leader)
        {
            AddMember(leader);
        }

        public void AddMember(Character member)
        {
            if (Members.Count==0)
            {
                Leader = member;
            }
            Members.Add(member);
            member.Team = this;
            TimeStamp = Time.timestamp;
        }

        public void Leave(Character member)
        {
            Log.InfoFormat($"Leave Team: {member.Id}:{member.Info.Name}");
            Members.Remove(member);
            if (member==this.Leader)
            {
                Leader = Members.Count>0 ? Members[0] : null;
            }

            member.Team = null;
            TimeStamp = Time.timestamp;
        }

        public void PostProcess(NetMessageResponse message)
        {
            if (message.teamInfo != null) return;
            message.teamInfo = new TeamInfoResponse
            {
                Result = Result.Success,
                Team = new NTeamInfo
                {
                    Id = this.Id,
                    Leader = Leader.Id,
                }
            };
            foreach (var member in Members)
            {
                message.teamInfo.Team.Members.Add(member.GetBasicInfo());
            }
        }

    }
}
