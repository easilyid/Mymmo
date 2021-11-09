using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Data;
using Managers;
using SkillBridge.Message;

namespace Entities
{
    public class Character:Creature
    {
        public Character(NCharacterInfo info) : base(info)
        {
        }

        public override List<EquipDefine> GetEquips()
        {
            return EquipManager.Instance.GetEquipDefines();
        }
    }
}
