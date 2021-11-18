using GameServer.Core;
using GameServer.Entities;
using SkillBridge.Message;

namespace GameServer.Battle
{
    public class BattleContext
    {
        public Battle Battle;

        public BattleContext(Battle battle)
        {
            this.Battle = battle;
        }

        public Creature Caster;
        public Creature Target;
        public NSkillCastInfo CastSkill;
        public NDamageInfo Damage;
        public SkillResult Result;
        public Vector3Int Position;
    }
}
