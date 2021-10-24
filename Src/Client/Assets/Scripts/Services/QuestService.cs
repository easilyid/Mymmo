using Managers;
using Models;
using Network;
using SkillBridge.Message;
using System;
using UnityEngine;

namespace Services
{
    public class QuestService : Singleton<QuestService>, IDisposable
    {
        public QuestService()
        {
            MessageDistributer.Instance.Subscribe<QuestAcceptResponse>(OnQuestAccept);
            MessageDistributer.Instance.Subscribe<QuestSubmitResponse>(OnQuestSubmit);

        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<QuestAcceptResponse>(OnQuestAccept);
            MessageDistributer.Instance.Unsubscribe<QuestSubmitResponse>(OnQuestSubmit);
        }

        private void OnQuestAccept(object sender, QuestAcceptResponse message)
        {
            Debug.LogFormat("OnQuestAccept:{0},ERR:{1}",message.Result,message.Errormsg);
            if (message.Result == Result.Success)
            {
                QuestManager.Instance.OnQuestAccepted(message.Quest);
            }
            else
            {
                MessageBox.Show("任务接受失败", "错误", MessageBoxType.Error);
            }
        }

        private void OnQuestSubmit(object sender, QuestSubmitResponse message)
        {
            Debug.LogFormat("OnQuestAccept:{0},ERR:{1}",message.Result,message.Errormsg);
            if (message.Result == Result.Success)
            {
                QuestManager.Instance.OnQuestSubmitted(message.Quest);
            }
            else
            {
                MessageBox.Show("任务完成失败", "错误", MessageBoxType.Error);
            }
        }

        public bool SendQuestAccept(Quest quest)
        {
            Debug.Log("SendQuestAccept");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.questAccept = new QuestAcceptRequest();
            message.Request.questAccept.QuestId = quest.Define.ID;
            NetClient.Instance.SendMessage(message);
            return true;
        }

        public bool SendQuestSubmit(Quest quest)
        {
            Debug.Log("SendQuestSubmit");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.questSubmit = new QuestSubmitRequest();
            message.Request.questSubmit.QuestId = quest.Define.ID;
            NetClient.Instance.SendMessage(message);
            return true;
        }
    }
}
