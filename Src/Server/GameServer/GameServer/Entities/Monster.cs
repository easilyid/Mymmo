using GameServer.Core;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Battle;
using GameServer.Battle;
using GameServer.Models;

namespace GameServer.Entities
{
    public class Monster : Creature
    { 
        Creature target;
        Map Map;
        public Monster(int tid, int level, Vector3Int pos, Vector3Int dir) : base(CharacterType.Monster, tid, level, pos, dir)
        {

        }

        public void OnEnterMap(Map map)
        {
            this.Map = map;
        }

        public override void Update()
        {
            if (this.State==CharState.InBattle)
            {
                this.UpdateBattle();
            }   
            base.Update();
        }

        private void UpdateBattle()
        {
            if (this.target!=null)
            {
                BattleContext context = new BattleContext(this.Map.Battle)
                {
                    Target = target,
                    Caster = this,
                };
                Skill skill = this.FindSkill(context);

                if (skill!=null)
                {
                    this.CastSkill(context, skill.Define.ID);
                }
            }
        }
        Skill FindSkill(BattleContext context)
        {
            Skill cancast = null;

            foreach (var skill in this.SkillMger.Skills)
            {
                var result = skill.CanCast(context);
                if (result == SkillResult.Casting)
                    return null;
                if (result==SkillResult.Ok)
                {
                    cancast = skill;
                }
            }

            return cancast;
        }

        protected override void OnDamage(NDamageInfo damage, Creature source)
        {
            if (target==null)
            {
                this.target = source;
            }
        }
    }
}
