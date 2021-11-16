using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Network;
using SkillBridge.Message;

namespace GameServer.Managers
{
    class BattleManager:Singleton<BattleManager>
    {
        private static long bid = 0;
        public void Init() { }
        public void ProcessBattleMessage(NetConnection<NetSession> sender, SkillCastRequest request)
        {
            Log.InfoFormat($"BattleManager.ProcessBattleMessage: skill:{request.castInfo.skillId} caster:{request.castInfo.casterId} target:{request.castInfo.targetId} pos:{request.castInfo.Position}");
            var character = sender.Session.Character;
            var battle = MapManager.Instance[character.Info.mapId].Battle;
            battle.ProcessBattleMessage(sender, request);
        }
    }
}
