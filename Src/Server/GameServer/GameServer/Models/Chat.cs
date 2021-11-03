using GameServer.Entities;
using GameServer.Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Models
{
    class Chat
    {
        private readonly Character owner;
        public int LocalIdx;
        public int WorldIdx;
        public int SystemIdx;
        public int TeamIdx;
        public int GuildIdx;
        public Chat(Character owner)
        {
            this.owner = owner;
        }

        public void PostProcess(NetMessageResponse message)
        {
            if (message.Chat == null)
            {
                message.Chat = new ChatResponse
                {
                    Result = Result.Success
                };
            }

            LocalIdx = ChatManager.Instance.GetLocalMessages(owner.Info.mapId, LocalIdx, message.Chat.localMessages);
            WorldIdx = ChatManager.Instance.GetWorldMessages(WorldIdx, message.Chat.worldMessages);
            SystemIdx = ChatManager.Instance.GetSystemMessages(SystemIdx, message.Chat.systemMessages);
            if (owner.Team!=null)
            {
                TeamIdx = ChatManager.Instance.GetTeamMessages(owner.Team.Id, this.TeamIdx, message.Chat.teamMessages);
            }
            if (owner.Guild != null)
            {
                GuildIdx = ChatManager.Instance.GetTeamMessages(owner.Guild.Id, this.GuildIdx, message.Chat.guildMessages);
            }
        }
    }
}
