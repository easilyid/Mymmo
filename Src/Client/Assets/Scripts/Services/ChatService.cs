using System;
using Network;
using SkillBridge.Message;
using UnityEngine;

public class ChatService : Singleton<ChatService>,IDisposable
{
    public ChatService()
    {
        MessageDistributer.Instance.Subscribe<ChatResponse>(OnChat);
    }


    public void Dispose()
    {
        MessageDistributer.Instance.Unsubscribe<ChatResponse>(OnChat);

    }

    public void Init()
    {

    }

    private void OnChat(object sender, ChatResponse message)
    {
        if (message.Result != Result.Success) return;
        ChatManager.Instance.AddMessages(ChatChannel.Local,message.localMessages);
        ChatManager.Instance.AddMessages(ChatChannel.World,message.worldMessages);
        ChatManager.Instance.AddMessages(ChatChannel.System,message.systemMessages);
        ChatManager.Instance.AddMessages(ChatChannel.Private,message.privateMessages);
        ChatManager.Instance.AddMessages(ChatChannel.Team,message.teamMessages);
        ChatManager.Instance.AddMessages(ChatChannel.Guild,message.guildMessages);
    }

    public void SendChat(ChatChannel chatChannel, string content, int toId, string toName)
    {
        Debug.Log("SendChat");
        var message = new NetMessage
        {
            Request = new NetMessageRequest
            {
                Chat = new ChatRequest
                {
                    Message = new ChatMessage
                    {
                        Channel = chatChannel,
                        ToId = toId,
                        ToName = toName,
                        Message = content
                    }
                }
            }
        };
        NetClient.Instance.SendMessage(message);
    }

}
