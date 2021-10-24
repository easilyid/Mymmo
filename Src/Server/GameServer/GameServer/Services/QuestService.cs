using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Network;
using SkillBridge.Message;

namespace GameServer.Services
{
    class QuestService : Singleton<QuestService>
    {
        public QuestService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<QuestAcceptRequest>(OnQuestAccept);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<QuestSubmitRequest>(OnQuestSubmit);
        }

        public void Init() { }

        private void OnQuestAccept(NetConnection<NetSession> sender, QuestAcceptRequest request)
        {
            var character = sender.Session.Character;
            Log.InfoFormat("QuestAcceptRequest::character:{0}:QuestId{1}", character.Id, request.QuestId);
            sender.Session.Response.questAccept = new QuestAcceptResponse();
            var result = character.QuestManager.AcceptQuest(sender, request.QuestId);
            sender.Session.Response.questAccept.Result = result;
            sender.SendResponse();
        }

        private void OnQuestSubmit(NetConnection<NetSession> sender, QuestSubmitRequest request)
        {
            var character = sender.Session.Character;
            Log.InfoFormat("QuestSubmitRequest::character:{0}:QuestId{1}", character.Id, request.QuestId);
            sender.Session.Response.questSubmit = new QuestSubmitResponse();
            var result = character.QuestManager.SubmitQuest(sender, request.QuestId);
            sender.Session.Response.questSubmit.Result = result;
            sender.SendResponse();
        }
    }
}
