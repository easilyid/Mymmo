using GameServer.Entities;
using SkillBridge.Message;
using System.Collections.Generic;
using Common.Battle;
using GameServer.Managers;

namespace GameServer.Battle
{
    public class SkillManager
    {
        private Creature Owner;

        public List<Skill> Skills { get; private set; }
        public List<NSkillInfo> Infos { get; private set; }

        public  Skill NormalSkill { get; private set; }
        public SkillManager(Creature owner)
        {
            this.Owner = owner;
            this.Skills = new List<Skill>();
            this.Infos = new List<NSkillInfo>();
            this.InitSkills();
        }

        private void InitSkills()
        {
            this.Skills.Clear();
            this.Infos.Clear();
            if (!DataManager.Instance.Skills.ContainsKey(this.Owner.Define.TID))
            {
                return;
            }

            foreach (var define in DataManager.Instance.Skills[this.Owner.Define.TID])
            {
                NSkillInfo info = new NSkillInfo();
                info.Id = define.Key;
                if (this.Owner.Info.Level>=define.Value.UnlockLevel)
                {
                    info.Level = 5;
                }
                else
                {
                    info.Level = 1;
                }
                this.Infos.Add(info);
                Skill skill = new Skill(info, this.Owner);
                if (define.Value.Type==SkillType.Normal)
                {
                    NormalSkill = skill;
                }
                this.AddSkill(skill);
            }
        }

        private void AddSkill(Skill skill)
        {
            this.Skills.Add(skill);
        }

        public Skill GetSkill(int skillId)
        {
            for (int i = 0; i < this.Skills.Count; i++)
            {
                if (this.Skills[i].Define.ID == skillId)
                    return this.Skills[i];
            }

            return null;
        }

        public void Update()
        {
            for (int i = 0; i < Skills.Count; i++)
            {
                this.Skills[i].Update();
            }
        }
    }
}
