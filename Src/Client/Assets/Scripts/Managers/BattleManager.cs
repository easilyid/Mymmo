using Battle;
using Entities;
using Services;
using SkillBridge.Message;
using UnityEngine;

namespace Managers
{
    public class BattleManager:Singleton<BattleManager>
    {
        public delegate void TargetChangedHandler(Creature target);

        public event TargetChangedHandler OnTargetChanged;

        private Creature currentTarget;

        public Creature CurrentTarget
        {
            get => currentTarget;
            set => SetTarget(value);
        }

        public void SetTarget(Creature target)
        {
            if (this.currentTarget!=target&&this.OnTargetChanged!=null)
            {
                this.OnTargetChanged(target);
            }
            this.currentTarget = target;
            Debug.LogFormat($"BattleManager.SetTarget:[{target.entityId}:{target.Name}]");
        }

        private NVector3 currentPosition;
        public NVector3 CurrentPosition
        {
            get => currentPosition;
            set => this.SetPosition(value);
        }
        private void SetPosition(NVector3 position)
        {
            this.currentPosition = position;
            Debug.LogFormat($"BattleManager.SetPosition:[{position}]");
        }
        public void CastSkill(Skill skill)
        {
            var target = currentTarget?.entityId ?? 0;
            BattleService.Instance.SendSkillCast(skill.Define.ID, skill.Owner.entityId, target, currentPosition);
        }
    }
}
