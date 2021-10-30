using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Managers;
using Network;
using UnityEngine;
using UnityEngine.Events;
using Models;

namespace Services
{
    class GuildService : Singleton<GuildService>, IDisposable
    {
        public UnityAction OnGuildUpdate;
        public UnityAction<bool> OnGuildCreateResult;

        public UnityAction<List<NGuildInfo>> OnGuildListResult;
        public UnityAction<bool> OnGuildChangeNoticeResult;


        public void Init()
        {

        }

        public GuildService()
        {
            MessageDistributer.Instance.Subscribe<GuildCreateResponse>(this.OnGuildCreate);
            MessageDistributer.Instance.Subscribe<GuildListResponse>(this.OnGuildList);
            MessageDistributer.Instance.Subscribe<GuildJoinRequest>(this.OnGuildJoinRequest);
            MessageDistributer.Instance.Subscribe<GuildJoinResponse>(this.OnGuildJoinResponse);
            MessageDistributer.Instance.Subscribe<GuildResponse>(this.OnGuild);
            MessageDistributer.Instance.Subscribe<GuildLeaveResponse>(this.OnGuildLeave);
            MessageDistributer.Instance.Subscribe<GuildNoticeChangeResponse>(this.OnGuildNoticeChange);
            MessageDistributer.Instance.Subscribe<GuildAdminResponse>(this.OnGuildAdmin);

        }


        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<GuildCreateResponse>(this.OnGuildCreate);
            MessageDistributer.Instance.Unsubscribe<GuildListResponse>(this.OnGuildList);
            MessageDistributer.Instance.Unsubscribe<GuildJoinRequest>(this.OnGuildJoinRequest);
            MessageDistributer.Instance.Unsubscribe<GuildJoinResponse>(this.OnGuildJoinResponse);
            MessageDistributer.Instance.Unsubscribe<GuildResponse>(this.OnGuild);
            MessageDistributer.Instance.Unsubscribe<GuildLeaveResponse>(this.OnGuildLeave);
            MessageDistributer.Instance.Unsubscribe<GuildNoticeChangeResponse>(this.OnGuildNoticeChange);
            MessageDistributer.Instance.Unsubscribe<GuildAdminResponse>(this.OnGuildAdmin);

        }

        #region 创建公会
        public void SendGuildCreate(string guildName, string notice)
        {
            Debug.Log("SendGuildCreate");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildCreate = new GuildCreateRequest();
            message.Request.guildCreate.GuildName = guildName;
            message.Request.guildCreate.GuildNotice = notice;
            NetClient.Instance.SendMessage(message);

        }

        private void OnGuildCreate(object sender, GuildCreateResponse response)
        {
            Debug.LogFormat("ONGuildCreateResponse :{0}", response.Result);
            if (OnGuildCreateResult != null)
            {
                this.OnGuildCreateResult(response.Result == Result.Success);
            }

            if (response.Result == Result.Success)
            {
                GuildManager.Instance.Init(response.guildInfo);
                MessageBox.Show(String.Format("{0} 公会创建成功", response.guildInfo.GuildName), "公会");
            }
            else
            {
                MessageBox.Show(String.Format("{0} 公会创建失败", response.guildInfo.GuildName), "公会");

            }
        }
        #endregion

        #region 加入公会

        public void SendGuildJoinRequest(int guildId)
        {
            Debug.Log("SendGuildJoinRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildJoinReq = new GuildJoinRequest();
            message.Request.guildJoinReq.Apply = new NGuildApplyInfo();
            message.Request.guildJoinReq.Apply.GuildId = guildId;
            NetClient.Instance.SendMessage(message);

        }

        public void SendGuildJoinResponse(bool accept, GuildJoinRequest request)
        {
            Debug.Log("SendGuildJoinResponse");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildJoinRes = new GuildJoinResponse();
            message.Request.guildJoinRes.Result = Result.Success;
            message.Request.guildJoinRes.Apply = request.Apply;
            message.Request.guildJoinRes.Apply.Result = accept ? ApplyResult.Accept : ApplyResult.Reject;
            NetClient.Instance.SendMessage(message);

        }
        private void OnGuildJoinRequest(object sender, GuildJoinRequest request)
        {
            var confirm = MessageBox.Show(String.Format("{0} 申请加入公会", request.Apply.Name), "公会申请",
                MessageBoxType.Confirm, "同意", "拒绝");
            confirm.OnYes = (() =>
            {
                this.SendGuildJoinResponse(true, request);
            });
            confirm.OnNo = (() =>
            {
                this.SendGuildJoinResponse(false, request);
            });
        }
        private void OnGuildJoinResponse(object sender, GuildJoinResponse response)
        {
            Debug.LogFormat("OnGuildJoinResponse:{0}", response.Result);
            if (response.Result == Result.Success)
            {
                MessageBox.Show("加入公会成功", "公会");
            }
            else
            {
                MessageBox.Show("加入公会失败", "公会");
            }
        }
        private void OnGuild(object sender, GuildResponse message)
        {
            Debug.LogFormat("OnGuild: {0} {1} :{2}", message.Result, message.Guilds.Id, message.Guilds.GuildName);
            GuildManager.Instance.Init(message.Guilds);
            if (this.OnGuildUpdate != null)
            {
                this.OnGuildUpdate();
            }
        }

        #endregion

        /// <summary>
        /// 离开公会
        /// </summary>
        public void SendGuildLeaveRequest()
        {
            Debug.Log("SendGuildLeaveRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildLeave = new GuildLeaveRequest();
            NetClient.Instance.SendMessage(message);


        }

        private void OnGuildLeave(object sender, GuildLeaveResponse message)
        {
            if (message.Result == Result.Success)
            {
                GuildManager.Instance.Init(null);
                MessageBox.Show("离开公会成功", "公会");
            }
            else
            {
                MessageBox.Show("离开公会失败","公会",MessageBoxType.Error);
            }

        }

        /// <summary>
        /// 请求公会列表
        /// </summary>
        public void SendGuildListRequest()
        {
            Debug.Log("SendGuildListRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildList = new GuildListRequest();
            NetClient.Instance.SendMessage(message);

        }

        private void OnGuildList(object sender, GuildListResponse response)
        {
            if (OnGuildListResult!=null)
            {
                this.OnGuildListResult(response.Guilds);
            }
        }

        /// <summary>
        /// 修改公会宣言
        /// </summary>
        /// <param name="notice"></param>
        public void SendGuildChangeNotice(string notice)
        {
            Debug.Log("SendGuildChangeNotice");
            var message = new NetMessage
            {
                Request = new NetMessageRequest
                {
                    guildNoticeChange = new GuildNoticeChangeRequest
                    {
                        GuildNotice = notice
                    }
                }
            };
            NetClient.Instance.SendMessage(message);
        }

        private void OnGuildNoticeChange(object sender, GuildNoticeChangeResponse response)
        {
            Debug.LogFormat($"OnGuildNoticeChange: {response.Result} {response.Errormsg}");
            OnGuildChangeNoticeResult?.Invoke(response.Result == Result.Success);
            if (response.Result == Result.Success)
            {
                GuildManager.Instance.Init(response.guildInfo);
            }
            else
            {
                MessageBox.Show($"{response.guildInfo.GuildName} 公会宣言修改失败", "公会");
            }
        }

        #region 管理公会
        /// <summary>
        /// 发送加入公会审批
        /// </summary>
        /// <param name="accept"></param>
        /// <param name="apply"></param>
        public void SendGuildJoinApply(bool accept,NGuildApplyInfo apply)
        {
            Debug.Log("SendGuildJoinApply");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildJoinRes = new GuildJoinResponse();
            message.Request.guildJoinRes.Result = Result.Success;
            message.Request.guildJoinRes.Apply = apply;
            message.Request.guildJoinRes.Apply.Result = accept ? ApplyResult.Accept : ApplyResult.Reject;
            NetClient.Instance.SendMessage(message);
        }

        public void SendAdminCommand(GuildAdminCommand command, int characterId)
        {
            Debug.Log("SendAdminCommand");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildAdmin = new GuildAdminRequest();
            message.Request.guildAdmin.Command = command;
            message.Request.guildAdmin.Target = characterId;
            NetClient.Instance.SendMessage(message);
        }


        private void OnGuildAdmin(object sender, GuildAdminResponse message)
        {
            Debug.LogFormat("GuildAdmin:{0}{1}", message.Command, message.Result);
            if (message.Command.Command == GuildAdminCommand.Depost)
            {

            }
            if (message.Command.Command == GuildAdminCommand.Kickout)
            {
                //获取message中的tragetid是否和玩家一致
                if (User.Instance.CurrentCharacter.Id == message.Command.Target)
                {
                    GuildManager.Instance.Init(null);
                }

            }
            if (message.Command.Command == GuildAdminCommand.Promote)
            {

            }
            if (message.Command.Command == GuildAdminCommand.Transfer)
            {

            }
            MessageBox.Show(string.Format("执行结果:{0}结果:{1}{1}", message.Command, message.Result, message.Errormsg));
        }
    }

    #endregion

}

