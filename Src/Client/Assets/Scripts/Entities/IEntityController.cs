using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkillBridge.Message;
using UnityEngine;

namespace Entities
{
    public interface IEntityController
    {
        void PlayAnim(string name);
        void SetStandby(bool standby);
        void UpdateDirection(); 
        Transform GetTransform();
        void PlayEffect(EffectType type, string name, Creature target, float duration);
        void PlayEffect(EffectType type, string name, NVector3 position, float duration);

    }
}
