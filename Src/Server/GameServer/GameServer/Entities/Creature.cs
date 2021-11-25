using Common.Data;
using GameServer.Core;
using GameServer.Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Battle;
using GameServer.Battle;

namespace GameServer.Entities
{
    public class Creature : Entity
    {

        public int Id { get; set; }
        public NCharacterInfo Info;
        public CharacterDefine Define;
        public string Name => this.Info.Name;

        public Attributes Attributes;
        public SkillManager SkillMger;
        public BuffManager BuffMger;
        public EffectManager EffectMar;

        public bool IsDeath = false;

        public CharState State;

        public Creature(CharacterType type, int configId, int level, Vector3Int pos, Vector3Int dir) :
           base(pos, dir)
        {
            this.Define = DataManager.Instance.Characters[configId];

            this.Info = new NCharacterInfo();
            this.Info.Type = type;
            this.Info.Level = level;
            this.Info.ConfigId = configId;
            this.Info.Entity = this.EntityData;
            this.Info.EntityId = this.entityId;
            this.Info.Name = this.Define.Name;
            this.InitSkills();
            this.InitBuffs();

            this.Attributes = new Attributes();
            this.Attributes.Init(this.Define,this.Info.Level,this.GetEquips(),this.Info.attrDynamic);
            this.Info.attrDynamic = this.Attributes.DynamicAttr;
        }


        internal int Distance(Creature target)
        {
            return (int) Vector3Int.Distance(Position, target.Position);
        }
        internal int Distance(Vector3Int position)
        {
            return (int)Vector3Int.Distance(Position, position);
        }


        private void InitSkills()
        {
            SkillMger = new SkillManager(this);
            this.Info.Skills.AddRange(this.SkillMger.Infos);
        }

        private void InitBuffs()
        {
            BuffMger = new BuffManager(this);
            EffectMar = new EffectManager(this);
        }

        public virtual List<EquipDefine> GetEquips()
        {
            return null;
        }

        internal void CastSkill(BattleContext context, int skillId)
        {
            Skill skill = this.SkillMger.GetSkill(skillId);
            context.Result = skill.Cast(context);
            if (context.Result == SkillResult.Ok)
            {
                this.State = CharState.InBattle;
            }

            if (context.CastSkill==null)
            {
                if (context.Result==SkillResult.Ok)
                {
                    context.CastSkill = new NSkillCastInfo()
                    {
                        casterId = this.entityId,
                        targetId = context.Target.entityId,
                        skillId = skill.Define.ID,
                        Position = new NVector3(),
                        Result = context.Result
                    };
                    context.Battle.AddCastSkillInfo(context.CastSkill);
                }
            }
            else
            {
                context.CastSkill.Result = context.Result;
                context.Battle.AddCastSkillInfo(context.CastSkill);
            }
        }

        public void DoDamage(NDamageInfo damage, Creature source)
        {
            this.State = CharState.InBattle;
            this.Attributes.HP -= damage.Damage;
            if (this.Attributes.HP<0)
            {
                this.IsDeath = true;

                damage.WillDead = true;
            }

            this.OnDamage(damage, source);
        }

        public override void Update()
        {
            this.SkillMger.Update();
            this.BuffMger.Update();

        }
        internal void AddBuff(BattleContext context, BuffDefine buffDefine)
        {
            this.BuffMger.AddBuff(context, buffDefine);
        }

        protected virtual void OnDamage(NDamageInfo damage, Creature source)
        {

        }


    }
}
