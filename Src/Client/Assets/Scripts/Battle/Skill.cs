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


        public Skill(NSkillInfo skillInfo, Creature owner)
        {
            this.Info = skillInfo;
            this.Owner = owner;
            Define = DataManager.Instance.Skills[(int) this.Owner.Define.Class][this.Info.Id];
            cd = 0;
        }

        public SkillResult CanCast(Creature target)
        {
            //技能检查，测试时关闭

            //if (this.Define.CastTarget == TargetType.Target)
            //{
            //    if (target == null || target == this.Owner)
            //    {
            //        return SkillResult.InvalidTarget;
            //    }
            //}
            //if (this.Define.CastTarget == TargetType.Position && BattleManager.Instance.CurrentPosition == null)
            //{
            //    return SkillResult.InvalidTarget;
            //}
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

        public void BeginCast()
        {
            this.IsCasting = true;
            this.castTime = 0;
            this.cd = this.Define.CD;
            //this.skillTime = 0;
            Owner.PlayAnim(this.Define.SkillAnim);
        }

        public void OnUpdate(float delta)
        {
            if (this.IsCasting)
            {
                
            }

            UpdateCD(delta);
        }

        private void UpdateCD(float delta)
        {
            if (this.cd>0)
            {
                this.cd -= delta;
            }

            if (cd<0)
            {
                this.cd = 0;
            }
        }
    }
}
