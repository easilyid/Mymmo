using Common.Data;
using GameServer.Entities;
using GameServer.Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using Common;
using Common.Battle;
using Common.Utils;
using GameServer.Core;

namespace GameServer.Battle
{
    public class Skill
    {
        public NSkillInfo Info;
        public Creature Owner;
        public SkillDefine Define;


        public float cd = 0;

        public SkillStatus Status;


        private float castingTime = 0;
        private float skillTime = 0;
        private int Hit = 0;
        private BattleContext Context;

        private NSkillHitInfo HitInfo;
        public float CD
        {
            get { return cd; }
        }

        public bool Instant
        {
            get
            {
                if (Define.CastTime > 0) return false;
                if (Define.Bullet) return false;
                if (Define. Duration> 0) return false;
                if (Define.HitTimes !=null&&Define.HitTimes.Count>0) return false;

                return true;
            }
        }

        public Skill(NSkillInfo info, Creature owner)
        {
            this.Info = info;
            this.Owner = owner;
            this.Define = DataManager.Instance.Skills[(int)this.Owner.Define.Class][this.Info.Id];
        }

        public SkillResult CanCast(BattleContext context)
        {
            if (this.Status != SkillStatus.None)
            {
                return SkillResult.Casting;
            }
            if (Define.CastTarget == TargetType.Target)
            {
                if (context.Target == null || context.Target == Owner)
                    return SkillResult.InvalidTarget;
                int distance = Owner.Distance(context.Target);
                if (distance > Define.CastRange)
                {
                    return SkillResult.OutOFRANGE;
                }
            }

            if (Define.CastTarget == TargetType.Position)
            {
                if (context.CastSkill.Position == null)
                {
                    return SkillResult.InvalidTarget;
                }

                if (Owner.Distance(context.Position) > Define.CastRange)
                {
                    return SkillResult.OutOFRANGE;
                }
            }

            if (Owner.Attributes.MP < Define.MPCost)
            {
                return SkillResult.OutOfMp;
            }

            if (cd > 0)
            {
                return SkillResult.CoolDown;
            }

            return SkillResult.Ok;
        }

        public SkillResult Cast(BattleContext context)
        {
            SkillResult result = this.CanCast(context);
            if (result == SkillResult.Ok)
            {
                this.skillTime = 0;
                this.castingTime = 0;
                this.cd = Define.CD;
                this.Context = context;
                this.Hit = 0;
                if (this.Instant)
                {
                    this.DoHit();
                }
                else
                {
                    if (Define.CastTime>0)
                    {
                        Status = SkillStatus.Casting;
                    }
                    else
                    {
                        Status = SkillStatus.Running;
                    }
                }
            }

            Log.InfoFormat("Skill[{0}].Cast Result:[{1}] Status:{2}", Define.Name, result, this.Status);
            return result;
        }

        void InitHitInfo()
        {
            this.HitInfo = new NSkillHitInfo();
            this.HitInfo.casterId = this.Context.Caster.entityId;
            this.HitInfo.skillId = this.Info.Id;
            this.HitInfo.hitId = this.Hit;
            Context.Battle.AddHitInfo(this.HitInfo);
        }

        private void DoHit()
        {
            InitHitInfo();
            Log.InfoFormat("Skill[{0}],DoHit[{1}]", Define.Name, Hit);
            this.Hit++;

            if (this.Define.Bullet)
            {
                CastBullet();
                return;
            }

            if (this.Define.AOERange>0)
            {
                this.HitRange();
                return;
            }

            if (this.Define.CastTarget==TargetType.Target)
            {
                this.HitTarget(Context.Target);
            }
        }


        internal void Update()
        {
            UpdateCD();
            if (Status == SkillStatus.Casting)
            {
                UpdateCasting();
            }
            else if(Status==SkillStatus.Running)
            {
                UpdateSkill();
            }
        }

        /// <summary>
        /// 持续时间
        /// </summary>
        private void UpdateCasting()
        {
            if (castingTime < Define.CastTime)
            {
                castingTime += Time.deltaTime;
            }
            else
            {
                castingTime = 0;
                Status = SkillStatus.Running;
                Log.InfoFormat("Skill[{0}] .UpdateCasting Finish", Define.Name);

            }
        }

        private void UpdateCD()
        {
            if (this.cd > 0)
            {
                this.cd -= Time.deltaTime;
            }

            if (cd < 0)
            {
                this.cd = 0;
            }
        }

        private void UpdateSkill()
        {
            skillTime += Time.deltaTime;
            if (Define.Duration>0)
            {
                if (skillTime>Define.Interval*(Hit+1))
                {
                    DoHit();
                }

                if (skillTime>=Define.Duration)
                {
                    this.Status = SkillStatus.None;
                    Log.InfoFormat("Skill[{0}].UpdateSkill Finish", Define.Name);
                }
            }
            else if (Define.HitTimes!=null&&Define.HitTimes.Count>0)
            {
                if (this.Hit<Define.HitTimes.Count)
                {
                    if (skillTime>Define.HitTimes[Hit])
                    {
                        DoHit();
                    }
                }
                else
                {
                    this.Status = SkillStatus.None;
                    Log.InfoFormat("Skill[{0}] .UpdateSkill Finish", Define.Name);
                }
            }
        }

        private void HitTarget(Creature target)
        {
            if (this.Define.CastTarget==TargetType.Self&&(target!=Context.Caster))return;

            else if(target==Context.Caster)return;

            NDamageInfo damage = this.CalcSkillDamage(Context.Caster, target);
            Log.InfoFormat("Skill[{0}].HitTarget[{1}] Damage:{2} Crit:{3}",this.Define.Name,target.Name,damage.Damage,damage.Crit);
            target.DoDamage(damage);
            this.HitInfo.Damages.Add(damage);
        } 
        private void HitRange()
        {
            Vector3Int pos;
            if (this.Define.CastTarget==TargetType.Target)
            {
                pos = Context.Target.Position;
            }
            else if (this.Define.CastTarget==TargetType.Position)
            {
                pos = Context.Position;
            }
            else
            {
                pos = Owner.Position;
            }

            List<Creature> units = this.Context.Battle.FindUnitsInRange(pos, this.Define.AOERange);

            foreach (var target in units)
            {
                this.HitTarget(target);
            }
        }

        private void CastBullet()
        {
            Log.InfoFormat("Skill[{0}].CastBullet[{1}]", this.Define.Name,this.Define.BulletResource);
        }
        /* 战斗计算公式
        物理伤害=物理攻击或技能原始伤害*（1-物理防御/（物理防御+100））
        魔法伤害=法术攻击或技能原始伤害*（1-魔法防御/（魔法防御+100））
        暴击伤害=固定两倍伤害
        注：伤害值最小值为1.当伤害值小于1的时候取1.
        注：最终伤害值在最终取舍时随机浮动5%。即：最终伤害值*(1-5%）＜最终伤害值输出＜最终伤害值*(1+5%)     
        */

        /// <summary>
        /// 计算技能伤害
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private NDamageInfo CalcSkillDamage(Creature caster, Creature target)
        {
            float ad = this.Define.AD + caster.Attributes.AD * this.Define.ADFactor;
            float ap = this.Define.AP + caster.Attributes.AP * this.Define.APFactor;

            float addmg = ad * (1 - target.Attributes.DEF / (target.Attributes.DEF + 100));
            float apdmg = ap * (1 - target.Attributes.MDEF / (target.Attributes.MDEF + 100));


            float final = addmg + apdmg;
            bool isCrit = IsCrit(caster.Attributes.CRI);
            if (isCrit)
            {
                final = final * 2f;//暴击两倍伤害
            }

            //随机浮动
            final = final * (float) MathUtil.Random.NextDouble() * 0.1f - 0.05f;

            NDamageInfo damage = new NDamageInfo();
            damage.entityId = target.entityId;
            damage.Damage = Math.Max(1, (int) final);
            damage.Crit = isCrit;
            return damage;
        }

        private bool IsCrit(float crit)
        {
            return MathUtil.Random.NextDouble() < crit;
        }

        //private void DoSkillDamage(BattleContext context)
        //{
        //    context.Damage = new NDamageInfo();
        //    context.Damage.entityId = context.Target.entityId;
        //    context.Damage.Damage = 100;
        //    context.Target.DoDamage(context.Damage);
        //}

    }
}
