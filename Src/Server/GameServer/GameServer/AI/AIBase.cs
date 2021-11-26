using Common.Battle;
using GameServer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Battle;
using SkillBridge.Message;

namespace GameServer.AI
{
    //真正的AI行为
    public class AIBase
    {
        public Monster Owner;
        Creature target;
        Skill normalSkill;

        public AIBase(Monster owner)
        {
            this.Owner = owner;
            normalSkill = this.Owner.SkillMger.NormalSkill;
        }

        internal void Update()
        {
            if (this.Owner.BattleState == BattleState.InBattle)
            {
                this.UpdateBattle();
            }
        }

        private void UpdateBattle()
        {
            if (this.target == null)
            {
                this.Owner.BattleState = BattleState.Idle;
                return;
            }
            if(!TryCastSkill())
            {
                if (!tryCastNormal())
                {
                    FollowTarget();
                }
            }
        }
        private bool TryCastSkill()
        {
            if (target != null)
            {
                BattleContext context = new BattleContext(this.Owner.Map.Battle)
                {
                    Target = target,
                    Caster = this.Owner,
                };
                Skill skill = this.Owner.FindSkill(context, SkillType.Skill);
                if (skill!=null)
                {
                    this.Owner.CastSkill(context,skill.Define.ID);
                    return true;
                }
            }
            return false;
        }

        private void FollowTarget()
        {
            int distance = this.Owner.Distance(this.target);
            if (distance > normalSkill.Define.CastRange - 50)
            {
                this.Owner.MoveTo(this.target.Position);
            }
            else
                this.Owner.StopMove();
        }

        private bool tryCastNormal()
        {
            if (target != null)
            {
                BattleContext context = new BattleContext(this.Owner.Map.Battle)
                {
                    Target = target,
                    Caster = this.Owner,
                };
                var result = normalSkill.CanCast(context);
                if (result == SkillResult.Ok)
                {
                    this.Owner.CastSkill(context, normalSkill.Define.ID);
                }
                if (result == SkillResult.OutOFRANGE)
                {
                    return false;
                }
            }
            return true;
        }


        internal void OnDamage(NDamageInfo damage, Creature source)
        {
            this.target = source;
        }
    }
}
