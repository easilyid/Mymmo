using System.Collections.Generic;
using System.Text;
using Models;
using SkillBridge.Message;
using UnityEngine;
using UnityEngine.Events;

public class ChatManager : Singleton<ChatManager>
{
    public enum  LocalChannel
    {
        All =0,
        Local = 1,
        World = 2,
        Team = 3,
        Guild = 4,
        Private = 5
    }


    public void Init()
    {
        foreach (var messages in Messages)
        {
            messages.Clear();
        }
    }

    public List<ChatMessage>[] Messages = new[]
    {
        new List<ChatMessage>(),
        new List<ChatMessage>(),
        new List<ChatMessage>(),
        new List<ChatMessage>(),
        new List<ChatMessage>(),
        new List<ChatMessage>()
    };


    private readonly ChatChannel[] channelFilter = new[]
    {
        ChatChannel.Local | ChatChannel.World | ChatChannel.Team | ChatChannel.Private | ChatChannel.System,
        ChatChannel.Local,
        ChatChannel.World,
        ChatChannel.Team,
        ChatChannel.Guild,
        ChatChannel.Private
    };

    public LocalChannel DisplayChannel;
    public UnityAction OnChat;
    public int PrivateID;
    public string PrivateName { get; set; }

    public void StartPrivateChat(int targetId, string targetName)
    {
        this.PrivateID = targetId;
        this.PrivateName = targetName;
        this.sendChannel = LocalChannel.Private;
        this.OnChat?.Invoke();
    }

    
    public void SendChat(string content, int toId=0, string toName="")
    {
        ChatService.Instance.SendChat(this.SendChannel, content, toId, toName);
    }

    public LocalChannel sendChannel;

    public ChatChannel SendChannel
    {
        get
        {
            switch (sendChannel)
            {
                //把本地频道和服务器频道做映射，转化成协议中服务器使用的频道
                case LocalChannel.Local: return ChatChannel.Local;
                case LocalChannel.World: return ChatChannel.World;
                case LocalChannel.Team: return ChatChannel.Team;
                case LocalChannel.Guild: return ChatChannel.Guild;
                case LocalChannel.Private: return ChatChannel.Private;
            }

            return ChatChannel.Local;
        }
    }

    
    public bool SetSendChannel(LocalChannel channel)
    {
        if (channel==LocalChannel.Team)
        {
            if (User.Instance.TeamInfo==null)
            {
                AddSystemMessage("你没有加入任何队伍");
                return false;
            }
        }

        if (channel==LocalChannel.Guild)
        {
            if (User.Instance.CurrentCharacter.Guild==null)
            {
                AddSystemMessage("你没有加入任何公会");
                return false;
            }
        }

        this.sendChannel = channel;
        Debug.LogFormat($"设置频道：{sendChannel}");
        return true;
    }


    public string GetCurrentMessages()
    {
        var sb = new StringBuilder();
        foreach (var message in Messages[(int)DisplayChannel])
        {
            sb.AppendLine(FormatMessage(message));
        }
        return sb.ToString();
    }

    private string FormatMessage(ChatMessage message)
    {
        switch (message.Channel)
        {
            //如果当前消息是本地，不设置颜色，直接在前面加上[本地]显示玩家及消息本身
            case ChatChannel.Local:
                return $"[本地]{FormatFromPlayer(message)}{message.Message}";
            //如果是消息是世界
            case ChatChannel.World:
                //把当前消息改变颜色
                return $"<color=cyan>[世界]{FormatFromPlayer(message)}{message.Message}</color>";
            //系统消息则不显示名称，直接显示[系统]+消息
            case ChatChannel.System:
                return $"<color=yellow>[系统]{message.Message}</color>";
            //私聊
            case ChatChannel.Private:
                return $"<color=magenta>[私聊]{FormatFromPlayer(message)}{message.Message}</color>";
            //组队
            case ChatChannel.Team:
                return $"<color=green>[队伍]{FormatFromPlayer(message)}{message.Message}</color>";
            //公会
            case ChatChannel.Guild:
                return $"<color=blue>[公会]{FormatFromPlayer(message)}{message.Message}</color>";
        }

        return "";
    }

    private string FormatFromPlayer(ChatMessage message)
    {
        if (message.FromId == User.Instance.CurrentCharacter.Id)
        {
            //则直接显示【我】
            return "<a name=\"\" class=\"player\">[我]</a>";
        }
        else
        {
            //则设置玩家名字，下方格式方便UIChat的OnClickChatLink方法使用
            return string.Format("<a name=\"c:{0}:{1}\" class=\"player\">{1}</a>", message.FromId, message.FromName);
        }
    }


    internal void AddMessages(ChatChannel channel, List<ChatMessage> messages)
    {
        for (var ch = 0; ch < 6; ch++)
        {
            if ((this.channelFilter[ch]&channel)==channel)
            {
                Messages[ch].AddRange(messages);
            }
        }
        OnChat?.Invoke();
    }

    private void AddSystemMessage(string msg,string from = "")
    {
        this.Messages[(int) LocalChannel.All].Add(new ChatMessage
        {
            Channel = ChatChannel.System,
            Message = msg,
            FromName = from
        });
       OnChat?.Invoke();
    }


    
}


