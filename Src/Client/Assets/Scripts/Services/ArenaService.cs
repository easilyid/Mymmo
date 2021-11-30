using Managers;
using Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Services
{
    public class ArenaService : Singleton<ArenaService>,IDisposable
    {
        public void Init()
        {

        }

        public ArenaService()
        {
            MessageDistributer.Instance.Subscribe<ArenaChallengeRequest>(this.OnArenaChallengeRequest);
            MessageDistributer.Instance.Subscribe<ArenaChallengeResponse>(this.OnArenaChallengeResponse);
            MessageDistributer.Instance.Subscribe<ArenaBeginResponse>(this.OnArenaBeginResponse);
            MessageDistributer.Instance.Subscribe<ArenaEndResponse>(this.OnArenaEndResponse);
            MessageDistributer.Instance.Subscribe<ArenaReadyResponse>(this.OnArenaReadyResponse);
            MessageDistributer.Instance.Subscribe<ArenaRoundEndResponse>(this.OnArenaRoundEndResponse);
            MessageDistributer.Instance.Subscribe<ArenaRoundStartResponse>(this.OnArenaRoundStartResponse);

        }


        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<ArenaChallengeRequest>(this.OnArenaChallengeRequest);
            MessageDistributer.Instance.Unsubscribe<ArenaChallengeResponse>(this.OnArenaChallengeResponse);
            MessageDistributer.Instance.Unsubscribe<ArenaBeginResponse>(this.OnArenaBeginResponse);
            MessageDistributer.Instance.Unsubscribe<ArenaEndResponse>(this.OnArenaEndResponse);
            MessageDistributer.Instance.Unsubscribe<ArenaReadyResponse>(this.OnArenaReadyResponse);
            MessageDistributer.Instance.Unsubscribe<ArenaRoundEndResponse>(this.OnArenaRoundEndResponse);
            MessageDistributer.Instance.Unsubscribe<ArenaRoundStartResponse>(this.OnArenaRoundStartResponse);

        }
        /// <summary>
        /// 发送挑战请求
        /// </summary>
        public void SendArenaChallengeRequest(int targetId, string targetName)
        {
            Debug.Log("SendArenaChallengeRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.arenaChallengeReq = new ArenaChallengeRequest();
            message.Request.arenaChallengeReq.ArenaInfo = new ArenaInfo();
            message.Request.arenaChallengeReq.ArenaInfo.Red = new ArenaPlayer()
            {
                EntityId = User.Instance.CurrentCharacterInfo.Id,
                Name = User.Instance.CurrentCharacterInfo.Name
            };
            message.Request.arenaChallengeReq.ArenaInfo.Bule = new ArenaPlayer()
            {
                EntityId = targetId,
                Name = targetName
            };
            NetClient.Instance.SendMessage(message);
        }
        /// <summary>
        /// 发送挑战响应
        /// </summary>
        /// <param name="accept"></param>
        /// <param name="request"></param>
        public void SendArenaChallengeResponse(bool accept, ArenaChallengeRequest request)
        {
            Debug.Log("SendArenaChallengeResponse");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.arenaChallengeRes = new ArenaChallengeResponse();
            message.Request.arenaChallengeRes.Result = accept ? Result.Success : Result.Failed;
            message.Request.arenaChallengeRes.Errormsg = accept ? "" : "对方拒绝了挑战请求";
            message.Request.arenaChallengeRes.ArenaInfo = request.ArenaInfo;
            NetClient.Instance.SendMessage(message);
        }
        //邀请对战
        private void OnArenaChallengeRequest(object sender, ArenaChallengeRequest request)
        {
            Debug.Log("OnArenaChallengeRequest");
            var cofirm = MessageBox.Show(String.Format("{0}邀请你竞技场对战", request.ArenaInfo.Red.Name), "竞技场对战",
                MessageBoxType.Confirm, "接受", "拒绝");
            cofirm.OnYes = () =>
            {
                this.SendArenaChallengeResponse(true, request);
            };
            cofirm.OnNo = (() =>
            {
                this.SendArenaChallengeResponse(false, request);
            });
        }
        private void OnArenaChallengeResponse(object sender, ArenaChallengeResponse message)
        {
            Debug.Log("OnArenaChallengeResponse");
            if (message.Result!=Result.Success)
            {
                MessageBox.Show(message.Errormsg, "对方拒绝挑战");
            }
        }
        private void OnArenaEndResponse(object sender, ArenaEndResponse message)
        {
            Debug.Log("OnArenaEndResponse");
            ArenaManager.Instance.ExitArena(message.ArenaInfo);
        }

        private void OnArenaBeginResponse(object sender, ArenaBeginResponse message)
        {
            Debug.Log("OnArenaBeginResponse");
            ArenaManager.Instance.EnterArena(message.ArenaInfo);
        }
        internal void SendArenaReadyRequest(int arenaId)
        {
            Debug.Log("SendArenaReadyRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.arenaReady = new ArenaReadyRequest();
            message.Request.arenaReady.entityId = User.Instance.CurrentCharacter.entityId;
            message.Request.arenaReady.arenaId = arenaId;
            NetClient.Instance.SendMessage(message);
        }
        private void OnArenaRoundStartResponse(object sender, ArenaRoundStartResponse message)
        {
            ArenaManager.Instance.OnRoundStart(message.Round,message.ArenaInfo);
        }

        private void OnArenaRoundEndResponse(object sender, ArenaRoundEndResponse message)
        {
            ArenaManager.Instance.OnRoundEnd(message.Round, message.ArenaInfo);
        }

        private void OnArenaReadyResponse(object sender, ArenaReadyResponse message)
        {
            ArenaManager.Instance.OnReady(message.Round, message.ArenaInfo);

        }

    }
}
