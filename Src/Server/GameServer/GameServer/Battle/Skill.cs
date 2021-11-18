using Common.Data;
using GameServer.Entities;
using GameServer.Managers;
using SkillBridge.Message;
using System;
using Common;
using Common.Battle;

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

        private void DoHit()
        {
            this.Hit++;
            Log.InfoFormat("Skill[{0}],DoHit[{1}]", Define.Name, Hit);
        }

        private void DoSkillDamage(BattleContext context)
        {
            context.Damage = new NDamageInfo();
            context.Damage.entityId = context.Target.entityId;
            context.Damage.Damage = 100;
            context.Target.DoDamage(context.Damage);
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
    }
}
