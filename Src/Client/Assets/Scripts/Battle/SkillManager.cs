using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Battle
{
    public class SkillManager
    { 
        Creature Owner;

        public delegate void SkillInfoUpdateHandle();

        public event SkillInfoUpdateHandle OnSkillInfoUpdate;

        public List<Skill> Skills { get; private set; }

        public SkillManager(Creature owner)
        {
            this.Owner = owner;
            this.Skills = new List<Skill>();
            this.InitSkills();
        }

        private void InitSkills()
        {
            this.Skills.Clear();
            foreach (var skillInfo in this.Owner.Info.Skills)
            {
                Skill skill = new Skill(skillInfo, this.Owner);
                this.AddSkill(skill);
            }

            if (OnSkillInfoUpdate!=null)
            {
                OnSkillInfoUpdate();
            }
        }

        public void UpdateSkills()
        {
            foreach (var skillInfo in Owner.Info.Skills)
            {
                Skill skill = this.GetSkill(skillInfo.Id);
                if (skill!=null)
                {
                    skill.Info = skillInfo;
                }
                else
                {
                    this.AddSkill(skill);
                }
            }

            if (OnSkillInfoUpdate!=null)
            {
                OnSkillInfoUpdate();
            }
        }

        public void AddSkill(Skill skill)
        {
            this.Skills.Add(skill);
        }
        public Skill GetSkill(int skillId)
        {
            foreach (var skill in Skills)
            {
                if (skill.Define.ID==skillId)
                {
                    return skill;
                }
            }
            return null;
        }

        public void OnUpdate(float delta)
        {
            for (var i = 0; i < Skills.Count; i++)
            {
                this.Skills[i].OnUpdate(delta);
            }
        }
    }
}
