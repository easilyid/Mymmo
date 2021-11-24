using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Battle;
using Common.Battle;
using Common.Data;
using Managers;
using Models;
using SkillBridge.Message;
using UnityEngine;

namespace Entities
{
    public class Creature : Entity
    {
        public NCharacterInfo Info;

        public CharacterDefine Define;

        public Attributes Attributes;

        public SkillManager SkillMgr;
        public BuffManager BuffMger;
        public EffectManager EffectMar;


        public Action<Buff> OnBuffAdd;
        public Action<Buff> OnBuffRemove;

        public int Id { get { return this.Info.Id; } }

        public string Name { get { if (this.Info.Type == CharacterType.Player) return this.Info.Name;else return this.Define.Name; } }

        public bool IsPlayer { get { return this.Info.Type == CharacterType.Player; } }


        public bool IsCurrentPlayer
        {
            get
            {
                if (!IsPlayer)
                {
                    return false;
                }

                return Info.Id == User.Instance.CurrentCharacterInfo.Id;
            }
        }
        private bool battleState;

        public bool BattleState
        {
            get => battleState;
            set
            {
                if (battleState!=value)
                {
                    battleState = value;
                    SetStandby(value);
                }
            }
        }

        internal int Distance(Creature target)
        {
            return (int) Vector3Int.Distance(this.position, target.position);
        }

        public Skill CastingSkill = null;
        public Creature(NCharacterInfo info) : base(info.Entity)
        {
            this.Info = info;
            this.Define = DataManager.Instance.Characters[info.ConfigId];
            this.Attributes = new Attributes();
            this.Attributes.Init(this.Define, this.Info.Level, this.GetEquips(), this.Info.attrDynamic);
            this.SkillMgr = new SkillManager(this);
            this.BuffMger = new BuffManager(this);
            this.EffectMar = new EffectManager(this);

        }

        public void UpdateInfo(NCharacterInfo info)
        {
            this.SetEntityData(info.Entity);
            this.Info = info;
            this.Attributes.Init(this.Define, this.Info.Level, this.GetEquips(), this.Info.attrDynamic);
            this.SkillMgr.UpdateSkills();

        }

        public virtual List<EquipDefine> GetEquips()
        {
            return null;
        }

        public void MoveForward()
        {
            Debug.LogFormat("MoveForward");
            this.speed = this.Define.Speed;
        }

        internal void FaceTo(Vector3Int position)
        {
            this.SetDirection(GameObjectTool.WorldToLogic(GameObjectTool.LogicToWorld(position-this.position).normalized));
            this.UpdateEntityData();
            if (this.Controller != null)
                this.Controller.UpdateDirection();
        }

        public void MoveBack()
        {
            Debug.LogFormat("MoveBack");
            this.speed = -this.Define.Speed;
        }

        public void Stop()
        {
            Debug.LogFormat("Stop");
            this.speed = 0;
        }

        public void SetDirection(Vector3Int direction)
        {
            Debug.LogFormat("SetDirection:{0}", direction);
            this.direction = direction;
        }

        public void SetPosition(Vector3Int position)
        {
            Debug.LogFormat("SetPosition:{0}", position);
            this.position = position;
        }
        public void CastSkill(int skillId, Creature target, NVector3 pos)
        {
            this.SetStandby(true);
            var skill = this.SkillMgr.GetSkill(skillId);
            skill.BeginCast(target,pos);
        }

        public void SetStandby(bool standby)
        {
            Controller?.SetStandby(standby);
        }
        public void PlayAnim(string name)
        {
            Controller?.PlayAnim(name);
        }

        public override void OnUpdate(float delta)
        {
            base.OnUpdate(delta);
            this.SkillMgr.OnUpdate(delta);
            this.BuffMger.OnUpdate(delta);
        }

        public void DoDamage(NDamageInfo damage)
        {
            Debug.LogFormat("Domage:{0} DMG:{1} CRIT:{2}",this.Name,damage.Damage, damage.Crit);
            this.Attributes.HP -= damage.Damage;
            this.PlayAnim("Hurt");
        }

        public void DoSkillHit(NSkillHitInfo hit)
        {
            Debug.LogFormat("DoSkillHit: Caster:{0} Skill:{1} Hit:{2} IsBullet:{3}", hit.casterId,hit.skillId,hit.hitId,hit.isBullet);

            var skill = this.SkillMgr.GetSkill(hit.skillId);
            skill.DoHit(hit);
        }

        internal void DoBuffAction(NBuffInfo buff)
        {
            switch (buff.Action)
            {
                case BuffAction.Add:
                    this.AddBuff(buff.buffId, buff.buffType, buff.casterId);
                    break;
                case BuffAction.Remove:
                    this.RemoveBuff(buff.buffId);
                    break;
                case BuffAction.Hit:
                    this.DoDamage(buff.Damage);
                    break;
                default:
                    break;
            }
        }
        private void AddBuff(int buffId, int buffType, int casterId)
        {
           var buff = this.BuffMger.AddBuff(buffId, buffType, casterId);
           if (buff != null && this.OnBuffAdd != null)
           {
               this.OnBuffAdd(buff);
           }

        }
        public void RemoveBuff(int buffId)
        {
            var buff = this.BuffMger.RemoveBuff(buffId);
            if (buff != null && this.OnBuffRemove != null)
            {
                this.OnBuffRemove(buff);
            }

        }
        internal void AddBuffEffect(BuffEffect effect)
        {
            this.EffectMar.AddEffect(effect);
        }
        internal void RemoveBuffEffect(BuffEffect effect)
        {
            this.EffectMar.RemoveEffect(effect);
        }

        public void PlayEffect(EffectType type, string name, Entity target, float duration)
        {
            if (string.IsNullOrEmpty(name))return;
            if (this.Controller != null)
                this.Controller.PlayEffect(type, name, target, duration);
        }
    }
}
