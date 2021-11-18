using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Battle;
using Common.Data;
using Entities;
using Managers;
using SkillBridge.Message;
using UnityEngine;

namespace Battle
{
    public class Skill
    {
        public NSkillInfo Info;
        public Creature Owner;
        public SkillDefine Define;
        private float cd;
        public float CD => cd;
        public bool IsCasting;
        private float castTime;
        private float skillTime;
        private SkillStatus Status;

        private Dictionary<int, List<NDamageInfo>> HitMap = new Dictionary<int, List<NDamageInfo>>();

        public Skill(NSkillInfo skillInfo, Creature owner)
        {
            this.Info = skillInfo;
            this.Owner = owner;
            Define = DataManager.Instance.Skills[(int)this.Owner.Define.Class][this.Info.Id];
            cd = 0;
        }

        public SkillResult CanCast(Creature target)
        {
            //技能检查，测试时关闭

            if (this.Define.CastTarget == TargetType.Target)
            {
                if (target == null || target == this.Owner)
                {
                    return SkillResult.InvalidTarget;
                }

                int distance = this.Owner.Distance(target);
                if (distance > this.Define.CastRange)
                {
                    return SkillResult.OutOFRANGE;
                }
            }
            if (this.Define.CastTarget == TargetType.Position && BattleManager.Instance.CurrentPosition == null)
            {
                return SkillResult.InvalidTarget;
            }
            if (this.Owner.Attributes.MP < this.Define.MPCost)
            {
                return SkillResult.OutOfMp;
            }
            if (this.cd > 0)
            {
                return SkillResult.CoolDown;
            }

            return SkillResult.Ok;
        }

        public void BeginCast(NDamageInfo damage)
        {
            this.IsCasting = true;
            this.castTime = 0;
            this.cd = this.Define.CD;
            this.skillTime = 0;
            this.Hit = 0;
            this.Damage = damage;
            Owner.PlayAnim(this.Define.SkillAnim);

            if (this.Define.CastTime>0)
            {
                this.Status = SkillStatus.Casting;
            }
            else
            {
                this.Status = SkillStatus.Running;
            }
        }

        public NDamageInfo Damage;
        public int Hit;

        public void OnUpdate(float delta)
        {
            UpdateCD(delta);
            if (this.Status == SkillStatus.Casting)
            {
                this.UpdateCasting();
            }
            else if(this.Status == SkillStatus.Running)
            {
                this.UpdateSkill();
            }
        }

        private void UpdateSkill()
        {
            skillTime += Time.deltaTime;
            if (Define.Duration > 0)
            {
                if (skillTime > Define.Interval * (Hit + 1))
                {
                    DoHit();
                }

                if (skillTime >= Define.Duration)
                {
                    this.Status = SkillStatus.None;
                    this.IsCasting = false;
                    Debug.LogFormat("Skill[{0}].UpdateSkill Finish", Define.Name);
                }
            }
            else if (Define.HitTimes != null && Define.HitTimes.Count > 0)
            {
                if (this.Hit < Define.HitTimes.Count)
                {
                    if (skillTime > Define.HitTimes[Hit])
                    {
                        DoHit();
                    }
                }
                else
                {
                    this.Status = SkillStatus.None;
                    this.IsCasting = false;
                    Debug.LogFormat("Skill[{0}] .UpdateSkill Finish", Define.Name);
                }
            }
        }

        private void UpdateCasting()
        {
            if (castTime < Define.CastTime)
            {
                castTime += Time.deltaTime;
            }
            else
            {
                castTime = 0;
                Status = SkillStatus.Running;
                Debug.LogFormat("Skill[{0}] .UpdateCasting Finish", Define.Name);

            }
        }

        private void DoHit()
        {
            List<NDamageInfo> damages;
            if (this.HitMap.TryGetValue(this.Hit,out damages))
            {
                DoHitDamages(damages);
            }
            Hit++;
        }

        private void DoHitDamages(List<NDamageInfo> damages)
        {
            foreach (var dmg in damages)
            {
                Creature target=EntityManager.Instance.GetEntity(dmg.entityId)as Creature;
                if (target==null)continue;
                target.DoDamage(dmg);
            }
        }

        private void UpdateCD(float delta)
        {
            if (this.cd > 0)
            {
                this.cd -= delta;
            }

            if (cd < 0)
            {
                this.cd = 0;
            }
        }

        internal void DoHit(int hitId, List<NDamageInfo> damages)
        {
            if (hitId<=this.Hit)
            {
                this.HitMap[hitId] = damages;
            }
            else
            {
                DoHitDamages(damages);
            }
        }
    }
}
