using Common.Data;
using GameServer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Battle;
using SkillBridge.Message;

namespace GameServer.Battle
{
    class Buff
    {
        public int BuffId;
        private Creature Owner;
        private BuffDefine BuffDefine;
        private BattleContext Context;

        private float time =0;
        private int hit;

        public bool Stoped { get; internal set; }

        public Buff(int buffID, Creature owner, BuffDefine buffDefine, BattleContext context)
        {
            this.BuffId = buffID;
            this.Owner = owner;
            this.BuffDefine = buffDefine;
            this.Context = context;

            this.OnAdd();
        }


        private void OnAdd()
        {
            if (this.BuffDefine.Effect != BuffEffect.None)
            {
                this.Owner.EffectMar.AddEffect(this.BuffDefine.Effect);
            }

            AddAttr();

            NBuffInfo buff = new NBuffInfo()
            {
                buffId = this.BuffId,
                buffType = this.BuffDefine.ID,
                casterId = this.Context.Caster.entityId,
                ownerId = this.Owner.entityId,
                Action = BuffAction.Add
            };
            Context.Battle.AddBuffAction(buff);
        }
        private void OnRemove()
        {
            RemoveAttr();
            Stoped = true;
            if (this.BuffDefine.Effect != BuffEffect.None)
            {
                this.Owner.EffectMar.RemoveEffect(this.BuffDefine.Effect);
            }

            NBuffInfo buff = new NBuffInfo()
            {
                buffId = this.BuffId,
                buffType = this.BuffDefine.ID,
                casterId = this.Context.Caster.entityId,
                ownerId = this.Owner.entityId,
                Action = BuffAction.Remove
            };
            Context.Battle.AddBuffAction(buff);

        }
        private void AddAttr()
        {
            if (this.BuffDefine.DEFRatio != 0)
            {
                this.Owner.Attributes.Buff.DEF += this.Owner.Attributes.Basic.DEF * this.BuffDefine.DEFRatio;
                this.Owner.Attributes.InitFinalAttributer();
            }
        }

        private void RemoveAttr()
        {
            if (this.BuffDefine.DEFRatio != 0)
            {
                this.Owner.Attributes.Buff.DEF -= this.Owner.Attributes.Basic.DEF * this.BuffDefine.DEFRatio;
                this.Owner.Attributes.InitFinalAttributer();
            }
        }

        internal void Update()
        {
            if(Stoped)return;
            this.time += Time.deltaTime;

            if (this.BuffDefine.Interval>0)
            {
                //带有时间间隔
                if (this.time > this.BuffDefine.Interval * (this.hit + 1))
                {
                    this.DoBuffDamage();
                }
            }

            if (time > this.BuffDefine.Duration)
            {
                this.OnRemove();
            }
        }

        private void DoBuffDamage()
        {
            this.hit++;
            NDamageInfo damage = this.CalcBuffDamage(Context.Caster);
            Log.InfoFormat("Buff[{0}].DoBuffDamage[{1}] Damage:{2} Crit:{3}", this.BuffDefine.Name, this.Owner.Name,
                damage.Damage, damage.Crit);
            this.Owner.DoDamage(damage);

            NBuffInfo buff = new NBuffInfo()
            {
                buffId = this.BuffId,
                buffType = this.BuffDefine.ID,
                casterId = this.Context.Caster.entityId,
                ownerId = this.Owner.entityId,
                Action = BuffAction.Hit,
                Damage = damage
            };
            Context.Battle.AddBuffAction(buff);
        }

        private NDamageInfo CalcBuffDamage(Creature caster)
        {
            float ad = this.BuffDefine.AD + caster.Attributes.AD * this.BuffDefine.ADFactor;
            float ap = this.BuffDefine.AP + caster.Attributes.AP * this.BuffDefine.APFactor;

            float addmg = ad * (1 - this.Owner.Attributes.DEF / (this.Owner.Attributes.DEF + 100));
            float apdmg = ap * (1 - this.Owner.Attributes.MDEF / (this.Owner.Attributes.MDEF + 100));


            float final = addmg + apdmg;

            NDamageInfo damage = new NDamageInfo();
            damage.entityId = this.Owner.entityId;
            damage.Damage = Math.Max(1, (int) final);
            return damage;
        }
    }
}
