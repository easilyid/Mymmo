using Common;
using GameServer.Services;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class EquipManager:Singleton<EquipManager>
    {
        public Result EquipItem(NetConnection<NetSession> sender, int slot, int itemId, bool isEquip)
        {
            var character = sender.Session.Character;
            if (!character.ItemManager.Items.ContainsKey(itemId))
            {
                return Result.Failed;
            }

            UpdateEquip(character.Data.Equips, slot, itemId, isEquip);
            DBService.Instance.Save();
            return Result.Success;
        }

        private unsafe void UpdateEquip(byte[] equipData, int slot, int itemId, bool isEquip)
        {
            fixed (byte* pt = equipData)
            {
                var slotId = (int*)(pt+slot*sizeof(int));
                (*slotId) = isEquip ? itemId : 0;
            }
        }
    }
}
