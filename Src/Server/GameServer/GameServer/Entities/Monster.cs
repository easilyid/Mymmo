using GameServer.Core;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Battle;
using GameServer.AI;
using GameServer.Battle;
using GameServer.Models;

namespace GameServer.Entities
{
    public class Monster : Creature
    {
        AIAgent AI;
        public Vector3Int moveTarget; 
        Vector3 movePosition;
        public Monster(int tid, int level, Vector3Int pos, Vector3Int dir) : base(CharacterType.Monster, tid, level, pos, dir)
        {
            this.AI = new AIAgent(this);
        }

        public override void OnEnterMap(Map map)
        {
            //this.AI.Init();
        }

        public override void Update()
        {
            base.Update();
            this.UpdateMovement();
            this.AI.Update();
        }
        public Skill FindSkill(BattleContext context, SkillType type)
        {
            Skill cancast = null;

            foreach (var skill in this.SkillMger.Skills)
            {
                if ((skill.Define.Type & type) != skill.Define.Type) continue;
                var result = skill.CanCast(context);
                if (result == SkillResult.Casting)
                    return null;//任何技能释放中
                if (result == SkillResult.Ok)
                {
                    cancast = skill;
                }
            }

            return cancast;
        }

        protected override void OnDamage(NDamageInfo damage, Creature source)
        {
            if (AI != null)
            {
                this.AI.OnDamage(damage, source);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="position">要移动的最终位置</param>
        internal void MoveTo(Vector3Int position)
        {
            if (State==CharacterState.Idle)
            {
                State = CharacterState.Move;
            }

            if (this.moveTarget!=position)
            {
                this.moveTarget = position;
                this.movePosition = Position;//这个position通过UpdateMovement计算出来的
                var dist = (this.moveTarget - this.Position);
                this.Direction = dist.normalized;
                this.Speed = this.Define.Speed;

                NEntitySync sync = new NEntitySync();
                sync.Entity = this.EntityData;
                sync.Event = EntityEvent.MoveFwd;
                sync.Id = this.entityId;

                this.Map.UpdateEntity(sync);
            }
        }
        private void UpdateMovement()
        {
            if (State==CharacterState.Move)
            {
                if (this.Distance(this.moveTarget)<50)
                {
                    this.StopMove();
                }
                //计算Position
                if (this.Speed>0)
                {
                    //增加底层支持，实现运算
                    Vector3 dir = this.Direction;
                    this.movePosition += dir * Speed * Time.deltaTime / 100f;
                    this.Position = movePosition;
                }
            }
        }
        internal void StopMove()
        {
            this.State = CharacterState.Idle;
            this.moveTarget=Vector3Int.zero;
            this.Speed = 0;

            NEntitySync sync = new NEntitySync();
            sync.Entity = this.EntityData;
            sync.Event = EntityEvent.Idle;
            sync.Id = this.entityId;

            this.Map.UpdateEntity(sync);
        }

    }
}
