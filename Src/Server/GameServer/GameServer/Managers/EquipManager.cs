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
        //修复服务端重启，装备没了
        //原因：装备数据没有保存成功，可能是EF6框架的BUG，检测不到二进制数据改变

        public Result EquipItem(NetConnection<NetSession> sender, int slot, int itemId, bool isEquip)
        {
            var character = sender.Session.Character;
            if (!character.ItemManager.Items.ContainsKey(itemId))
            {
                return Result.Failed;
            }

            character.Data.Equips= UpdateEquip(character.Data.Equips, slot, itemId, isEquip);
            DBService.Instance.Save();
            return Result.Success;
        }

        private unsafe byte[] UpdateEquip(byte[] equipData, int slot, int itemId, bool isEquip)
        {
            byte[] EquipData = new byte[28];
            fixed (byte* pt = equipData)
            {
                var slotId = (int*)(pt+slot*sizeof(int));
                if (isEquip)
                {
                    *slotId = itemId;
                }
                else
                {
                    *slotId = 0;
                }
            }
            Array.Copy(equipData,EquipData,28);
            return EquipData;
        }
    }
}
