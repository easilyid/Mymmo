using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Battle
{
    class Bullet
    {
        private Skill skill;
        private int hit = 0;
        private float flyTime = 0;
        private float duration = 0;

        public bool Stoped ;

        public Bullet(Skill skill)
        {
            this.skill = skill;
            var target = skill.Target;
            this.hit = skill.Hit;
            int distance = skill.Owner.Distance(target);
            duration = distance / this.skill.Define.BulletSpeed;
        }

        public void Update()
        {
            if(this.Stoped)return;
            this.flyTime += Time.deltaTime;
            if (this.flyTime > duration)
            {
                this.skill.DoHitDamages(this.hit);
                this.Stoped = true;
            }
        }
    }
}
