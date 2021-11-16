using Common;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Entities;
using GameServer.Managers;

namespace GameServer.Services
{
    public class BattleService : Singleton<BattleService>
    {
        public void Init()
        {

        }

        public BattleService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<SkillCastRequest>(OnSkillCast);

        }

        private void OnSkillCast(NetConnection<NetSession> sender, SkillCastRequest request)
        {
            Log.InfoFormat("OnSkillCast:skill:{0} caster:{1} target:{2} pos:{3} ", request.castInfo.skillId,
                request.castInfo.casterId, request.castInfo.targetId, request.castInfo.Position);

            BattleManager.Instance.ProcessBattleMessage(sender,request);
        }
    }
}
