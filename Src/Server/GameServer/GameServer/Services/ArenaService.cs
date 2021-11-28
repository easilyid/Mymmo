using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.Entities;
using GameServer.Managers;
using Network;
using SkillBridge.Message;

namespace GameServer.Services
{
    class ArenaService:Singleton<ArenaService>
    {
        public void Init()
        {
            ArenaManager.Instance.Init();
        }

        public ArenaService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<ArenaChallengeRequest>(this.OnArenaChallengeRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<ArenaChallengeResponse>(this.OnArenaChallengeResponse);

        }

        public void Dispose()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Unsubscribe<ArenaChallengeRequest>(this.OnArenaChallengeRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Unsubscribe<ArenaChallengeResponse>(this.OnArenaChallengeResponse);

        }

        private void OnArenaChallengeRequest(NetConnection<NetSession> sender, ArenaChallengeRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnArenaChallengeRequest: : RedId:{0} RedName:{1} BlueID:{2} BlueName:{3}",
                request.ArenaInfo.Red.EntityId, request.ArenaInfo.Red.Name, request.ArenaInfo.Bule.EntityId,
                request.ArenaInfo.Bule.Name);
            NetConnection<NetSession> blue = null;
            if (request.ArenaInfo.Bule.EntityId>0)
            {
                blue = SessionManager.Instance.GetSession(request.ArenaInfo.Bule.EntityId);
            }

            if (blue==null)
            {
                sender.Session.Response.arenaChallengeRes = new ArenaChallengeResponse();
                sender.Session.Response.arenaChallengeRes.Result = Result.Failed;
                sender.Session.Response.arenaChallengeRes.Errormsg = "好友不存在或者不在线";
                sender.SendResponse();
                return;
            }

            Log.InfoFormat("ForwardArenaChallengeRequest: :RedId:{0} RedName:{1} BlueID:{2} BlueName:{3}",
                request.ArenaInfo.Red.EntityId, request.ArenaInfo.Red.Name, request.ArenaInfo.Bule.EntityId,
                request.ArenaInfo.Bule.Name);
            blue.Session.Response.arenaChallengeReq = request;
            blue.SendResponse();
        }

        private void OnArenaChallengeResponse(NetConnection<NetSession> sender, ArenaChallengeResponse response)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnArenaChallengeResponse: : cahracter:{0} Result:{1}", character.Id,
                response.Result);
            var requester = SessionManager.Instance.GetSession(response.ArenaInfo.Red.EntityId);
            if (requester==null)
            {
                sender.Session.Response.arenaChallengeRes.Result = Result.Failed;
                sender.Session.Response.arenaChallengeRes.Errormsg = "挑战者已下线";
                sender.SendResponse();
                return;
            }

            if (response.Result==Result.Failed)
            {
                requester.Session.Response.arenaChallengeRes = response;
                requester.Session.Response.arenaChallengeRes.Result = Result.Failed;
                requester.SendResponse();
                return;
            }

            //var arena = ArenaManager.Instance.NewArena(response.ArenaInfo, requester, sender);
            //this.SendArenaBegin(arena);
            this.SendArenaBegin(requester,sender);
        }

        //private void SendArenaBegin(Arena arena)
        //{
        //    var arenaBegin = new ArenaBeginResponse();
        //    arenaBegin.Result = Result.Failed;
        //    arenaBegin.Errormsg = "对方不在线";
        //    arenaBegin.ArenaInfo = arena.ArenaInfo;
        //    arena.Red.Session.Response.arenaBegin = arenaBegin;
        //    arena.Red.SendResponse();
        //    arena.Blue.Session.Response.arenaBegin = arenaBegin;
        //    arena.Blue.SendResponse();
        //}
        private void SendArenaBegin(NetConnection<NetSession> Red, NetConnection<NetSession> Blue)
        {
            var arenaBegin = new ArenaBeginResponse();
            arenaBegin.Result = Result.Failed;
            arenaBegin.Errormsg = "对方不在线";
            //arenaBegin.ArenaInfo = arena.ArenaInfo;
            Red.Session.Response.arenaBegin = arenaBegin;
            Red.SendResponse();
            Blue.Session.Response.arenaBegin = arenaBegin;
            Blue.SendResponse();
        }
    }

}

