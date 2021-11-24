using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Entities
{
    public interface IEntityController
    {
        void PlayAnim(string name);
        void SetStandby(bool standby);
        void UpdateDirection();
        Transform GetTransform();
        void PlayEffect(EffectType type, string name, Entity target, float duration);
    }
}
