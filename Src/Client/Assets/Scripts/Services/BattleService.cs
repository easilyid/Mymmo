using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Managers;
using Network;
using SkillBridge.Message;
using UnityEngine;

namespace Services
{
    public class BattleService : Singleton<BattleService>, IDisposable
    {
        public void Init()
        {

        }

        public BattleService()
        {
            MessageDistributer.Instance.Subscribe<SkillCastResponse>(OnSkillCast);
            MessageDistributer.Instance.Subscribe<SkillHitResponse>(OnSkillHit);
        }


        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<SkillCastResponse>(OnSkillCast);
            MessageDistributer.Instance.Unsubscribe<SkillHitResponse>(OnSkillHit);
        }

        public void SendSkillCast(int skillId, int casterId, int targetId, NVector3 position)
        {
            if (position == null)
            {
                position = new NVector3();
            }
            Debug.LogFormat($"SendSkillCast: skill: {skillId} caster:{casterId} target:{targetId} pos:{position.ToString()}");
            var message = new NetMessage
            {
                Request = new NetMessageRequest
                {
                    skillCast = new SkillCastRequest
                    {
                        castInfo = new NSkillCastInfo
                        {
                            skillId = skillId,
                            casterId = casterId,
                            targetId = targetId,
                            Position = position
                        }
                    }
                }
            };
            NetClient.Instance.SendMessage(message);
        }

        private void OnSkillCast(object sender, SkillCastResponse message)
        {
            Debug.LogFormat("OnSkillCast: skill:{0} caster:{1} target:{2} pos:{3} result:{4}", message.castInfo.skillId,
                message.castInfo.casterId, message.castInfo.targetId, message.castInfo.Position.String(),
                message.Result);
            if (message.Result == Result.Success)
            {
                Creature caster = EntityManager.Instance.GetEntity(message.castInfo.casterId) as Creature;
                if (caster != null)
                {
                    Creature target = EntityManager.Instance.GetEntity(message.castInfo.targetId) as Creature;
                    caster.CastSkill(message.castInfo.skillId, target, message.castInfo.Position,message.Damage);
                }
            }
            else
            {
                ChatManager.Instance.AddSystemMessage(message.Errormsg);
            }
        }


        private void OnSkillHit(object sender, SkillHitResponse message)
        {
            Debug.LogFormat("OnSkillHit: count:{0}",message.Hits.Count);
            if (message.Result==Result.Success)
            {
                foreach (var hit in message.Hits)
                {
                    Creature caster = EntityManager.Instance.GetEntity(hit.casterId) as Creature;
                    if (caster!=null)
                    {
                        caster.DoSkillHit(hit);
                    }
                }
            }
        }

    }
}
