using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Data;
using Common.Utils;
using GameServer.Entities;
using SkillBridge.Message;

namespace GameServer.Managers
{
    class ChatManager : Singleton<ChatManager>
    {
        public List<ChatMessage> System = new List<ChatMessage>();//系统
        public List<ChatMessage> World = new List<ChatMessage>();//世界
        public Dictionary<int, List<ChatMessage>> Local = new Dictionary<int, List<ChatMessage>>();//本地
        public Dictionary<int, List<ChatMessage>> Team = new Dictionary<int, List<ChatMessage>>();//队伍
        public Dictionary<int, List<ChatMessage>> Guild = new Dictionary<int, List<ChatMessage>>(); //公会


        public void Init() { }

        public void AddMessage(Character from, ChatMessage message)
        {
            message.FromId = from.Id;
            message.FromName = from.Name;
            message.Time = TimeUtil.timestamp;
            switch (message.Channel)
            {
                case ChatChannel.Local:
                    AddLocalMessage(from.Info.mapId, message);
                    break;
                case ChatChannel.World:
                    AddWorldMessage(message);
                    break;
                case ChatChannel.System:
                    AddSystemMessage(message);
                    break;
                case ChatChannel.Team:
                    AddTeamMessage(from.Team.Id, message);
                    break;
                case ChatChannel.Guild:
                    AddGuildMessage(from.Guild.Id, message);
                    break;
            }
        }

        private void AddLocalMessage(int mapId, ChatMessage message)
        {
            if (!Local.TryGetValue(mapId,out List<ChatMessage> messages))
            {
                messages = new List<ChatMessage>();
                Local[mapId] = messages;
            }
            messages.Add(message);
        }

        private void AddWorldMessage(ChatMessage message)
        {
            World.Add(message);
        }

        private void AddSystemMessage(ChatMessage message)
        {
            System.Add(message);
        }

        private void AddTeamMessage(int teamId, ChatMessage message)
        {
            if (!Team.TryGetValue(teamId, out List<ChatMessage> messages))
            {
                messages = new List<ChatMessage>();
                Local[teamId] = messages;
            }
            messages.Add(message);
        }

        private void AddGuildMessage(int guildId, ChatMessage message)
        {
            if (!Guild.TryGetValue(guildId, out List<ChatMessage> messages))
            {
                messages = new List<ChatMessage>();
                Local[guildId] = messages;
            }
            messages.Add(message);
        }

        public int GetLocalMessages(int mapId, int idx, List<ChatMessage> result)
        {
            if (!Local.TryGetValue(mapId,out List<ChatMessage> messages))
            {
                return 0;
            }

            return GetNewMessages(idx, result, messages);
        }
        public int GetWorldMessages(int idx, List<ChatMessage> result)
        {
            return GetNewMessages(idx, result, World);
        }
        public int GetSystemMessages(int idx, List<ChatMessage> result)
        {
            return GetNewMessages(idx, result, World);
        }

        public int GetTeamMessages(int teamId, int idx, List<ChatMessage> result)
        {
            if (!Team.TryGetValue(teamId, out List<ChatMessage> messages))
            {
                return 0;
            }

            return GetNewMessages(idx, result, messages);
        }

        public int GetGuildMessages(int guildId, int idx, List<ChatMessage> result)
        {
            if (!Guild.TryGetValue(guildId, out List<ChatMessage> messages))
            {
                return 0;
            }

            return GetNewMessages(idx, result, messages);
        }

        private int GetNewMessages(int idx, List<ChatMessage> result, List<ChatMessage> messages)
        {
            if (idx == 0)
            {
                if (messages.Count > GameDefine.MaxChatRecoredNums)
                {
                    idx = messages.Count - GameDefine.MaxChatRecoredNums;
                }
            }

            for (; idx < messages.Count; idx++)
            {
                result.Add(messages[idx]);
            }

            return idx;
        }
    }
}
