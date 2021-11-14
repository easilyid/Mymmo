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
        public Skill CastingSkill = null;
        public Creature(NCharacterInfo info) : base(info.Entity)
        {
            this.Info = info;
            this.Define = DataManager.Instance.Characters[info.ConfigId];
            this.Attributes = new Attributes();
            this.Attributes.Init(this.Define, this.Info.Level, this.GetEquips(), this.Info.attrDynamic);
            this.SkillMgr = new SkillManager(this);
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
        public void CastSkill(int skillId, Entity target, NVector3 position)
        {
            this.SetStandby(true);
            var skill = this.SkillMgr.GetSkill(skillId);
            skill.BeginCast();
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
        }
    }
}
