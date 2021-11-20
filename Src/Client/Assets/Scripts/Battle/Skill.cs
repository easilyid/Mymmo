using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
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
        public Creature Target;

        public SkillDefine Define;
        private float cd;
        public float CD => cd;
        public bool IsCasting;
        private float castTime;
        private float skillTime;
        private SkillStatus Status;
        public int Hit;


        private Dictionary<int, List<NDamageInfo>> HitMap = new Dictionary<int, List<NDamageInfo>>();

        private List<Bullet> Bullets = new List<Bullet>();
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

        public void BeginCast(Creature target)
        {
            this.IsCasting = true;
            this.castTime = 0;
            this.cd = this.Define.CD;
            this.skillTime = 0;
            this.Hit = 0;
            this.Target = target;
            Owner.PlayAnim(this.Define.SkillAnim);
            this.Bullets.Clear();
            this.HitMap.Clear();

            if (this.Define.CastTime > 0)
            {
                this.Status = SkillStatus.Casting;
            }
            else
            {
                this.Status = SkillStatus.Running;
            }
        }


        public void OnUpdate(float delta)
        {
            UpdateCD(delta);
            if (this.Status == SkillStatus.Casting)
            {
                this.UpdateCasting();
            }
            else if (this.Status == SkillStatus.Running)
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
                    if (!this.Define.Bullet)
                    {
                        this.Status = SkillStatus.None;
                        this.IsCasting = false;
                        Debug.LogFormat("Skill[{0}].UpdateSkill Finish", this.Define.Name);

                    }

                    if (this.Define.Bullet)
                    {
                        bool finish = true;
                        foreach (Bullet bullet in this.Bullets)
                        {
                            bullet.Update();
                            if (!bullet.Stoped) finish = false;
                        }

                        if (finish && this.Hit >= this.Define.HitTimes.Count)
                        {
                            this.Status = SkillStatus.None;
                            this.IsCasting = false;
                            Debug.LogFormat("Skill[{0}] .UpdateSkill Finish", Define.Name);
                        }
                    }
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
            if (this.Define.Bullet)
                this.CastBullet();
            else
            {
                this.DoHitDamages(this.Hit);
            }
            Hit++;
        }

        public void DoHitDamages(int hit)
        {
            List<NDamageInfo> damages;
            if (this.HitMap.TryGetValue(hit, out damages))
            {
                DoHitDamages(damages);
            }
        }
        public void CastBullet()
        {
            Bullet bullet = new Bullet(this);
            Log.InfoFormat("Skill[{0}].CastBullet[{1}] Target:{2}", this.Define.Name, this.Define.BulletResource,
                this.Target.Name);
            this.Bullets.Add(bullet);
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

        //给服务器发回来的消息用的
        internal void DoHit(NSkillHitInfo hit)
        {
            if (hit.isBullet || this.Define.Bullet)
            {
                this.DoHit(hit.hitId, hit.Damages);
            }
        }

        internal void DoHit(int hitId, List<NDamageInfo> damages)
        {
            if (hitId > this.Hit)
            {
                this.HitMap[hitId] = damages;
            }
            else
            {
                DoHitDamages(damages);
            }
        }
        //直接造成伤害
        internal void DoHitDamages(List<NDamageInfo> damages)
        {
            foreach (var dmg in damages)
            {
                Creature target = EntityManager.Instance.GetEntity(dmg.entityId) as Creature;
                if (target == null) continue;
                target.DoDamage(dmg);
            }
        }

    }
}
