using Common;
using GameServer.Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    class TeamService:Singleton<TeamService>
    {
        public TeamService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TeamInviteRequest>(OnTeamInviteRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TeamInviteResponse>(OnTeamInviteResponse);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TeamLeaveRequest>(OnTeamLeave);
        }

        public void Init()
        {
            TeamManager.Instance.Init();
        }

        /// <summary>
        /// 收到组队请求 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        private void OnTeamInviteRequest(NetConnection<NetSession> sender, TeamInviteRequest request)
        {
            var character = sender.Session.Character;
            Log.InfoFormat($"OnTeamInviteRequest: :FromId:{request.FromId} FromName:{request.FromName} ToId{request.ToId} ToName:{request.ToName}");
            //todo:执行一些前置数据校验

            //开始逻辑
            var target = SessionManager.Instance.GetSession(request.ToId);
            if (target==null)
            {
                sender.Session.Response.teamInviteRes = new TeamInviteResponse();
                sender.Session.Response.teamInviteRes.Result = Result.Failed;
                sender.Session.Response.teamInviteRes.Errormsg = "好友不在线";
                sender.SendResponse();
                return;
            }
            if (target.Session.Character.Team!=null)
            {
                sender.Session.Response.teamInviteRes = new TeamInviteResponse();
                sender.Session.Response.teamInviteRes.Result = Result.Failed;
                sender.Session.Response.teamInviteRes.Errormsg = "对方已经有队伍";
                sender.SendResponse();
                return;
            }
            //转发请求
            Log.InfoFormat($"ForwardTeamInviteRequest: :FromId:{request.FromId} FromName:{request.FromName} ToId{request.ToId} ToName:{request.ToName}");
            target.Session.Response.teamInviteReq = request;
            target.SendResponse();
        }

        /// <summary>
        /// 收到组队响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="response"></param>
        private void OnTeamInviteResponse(NetConnection<NetSession> sender, TeamInviteResponse response)
        {
            var character = sender.Session.Character;
            Log.InfoFormat($"OnTeamInviteResponse: :character:{character.Id} Result:{response.Result} FromId:{response.Request.FromId} ToId{response.Request.ToId} ");
            sender.Session.Response.teamInviteRes = response;
            if (response.Result==Result.Success)
            {
                var requester = SessionManager.Instance.GetSession(response.Request.FromId);
                if (requester==null)
                {
                    sender.Session.Response.teamInviteRes.Result = Result.Failed;
                    sender.Session.Response.teamInviteRes.Errormsg = "请求者已下线";
                }
                else
                {
                    TeamManager.Instance.AddTeamMember(requester.Session.Character, character);
                    requester.Session.Response.teamInviteRes = response;
                    requester.SendResponse();
                }
            }
            sender.SendResponse();
        }

        private void OnTeamLeave(NetConnection<NetSession> sender, TeamLeaveRequest request)
        {
            var character = sender.Session.Character;
            Log.InfoFormat("接收离开队伍响应:  角色:{0} 队伍ID{1}:{2}", character.Id, request.TeamId, request.characterId);
            sender.Session.Response.teamLeave = new TeamLeaveResponse
            {
                Result = Result.Success, characterId = request.characterId
            };

            character.Team.Leave(character);
            sender.SendResponse();
        }

    }
}
