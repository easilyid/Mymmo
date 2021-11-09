using Common.Data;
using GameServer.Core;
using GameServer.Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Battle;

namespace GameServer.Entities
{
    public class Creature : Entity
    {

        public int Id { get; set; }
        public NCharacterInfo Info;
        public CharacterDefine Define;
        public string Name => this.Info.Name;
        public SkillManager SkillMger;

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
            this.InieSkills();
        }

        private void InieSkills()
        {
            SkillMger = new SkillManager(this);
            this.Info.Skills.AddRange(this.SkillMger.Infos);
        }
    }
}
