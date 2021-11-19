using GameServer.Battle;
using GameServer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using SkillBridge.Message;

namespace Battle
{
    class Bullet
    {
        private Skill skill;
        private NSkillHitInfo hitInfo;
        private bool TimeMode = true;
        private float duration = 0;
        private float flyTime = 0;
        public Bullet(Skill skill, Creature target, NSkillHitInfo hitInfo)
        {
            this.skill = skill;
            this.hitInfo = hitInfo;
            int diatance = skill.Owner.Distance(target);
            if (TimeMode)
            {
                duration = duration / this.skill.Define.BulletSpeed;
            }

            Log.InfoFormat("Bullet[{0}].CastBullet[{1}] Target:{2} Distance:{3} Time:{4}", skill.Define.Name,
                skill.Define.BulletResource, target.Name, diatance, duration);
        }

        public void Update()
        {
            if(Stoped)return;
            if (TimeMode)
            {
                this.UpdateTime();
            }
            else
            {
                this.UpdatePos();
            }
        }

        private void UpdatePos()
        {
            /*int distance = skill.Owner.Distance(target);
            if (distance>50)
            {
                pos += speed * Time.deltaTime;
            }
            else
            {
                this.hitInfo.isBullet = true;
                this.skill.DoHit(this.hitInfo);
                this.stoped = true;
            }*/
        }

        private void UpdateTime()
        {
            this.flyTime += Time.deltaTime;
            if (this.flyTime > duration)
            {
                hitInfo.isBullet = true;
                this.skill.DoHit(hitInfo);
                this.Stoped = true;
            }
        }

        public bool Stoped { get; set; }
    }
}
