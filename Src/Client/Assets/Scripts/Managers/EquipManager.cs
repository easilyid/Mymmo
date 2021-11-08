using Common.Data;
using Models;
using Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Managers
{
    class EquipManager : Singleton<EquipManager>
    {
        public delegate void OnEquipChangeHandler();

        public event OnEquipChangeHandler OnEquipChanged;
        public Item[] Equips = new Item[(int)EquipSlot.SlotMax];
        private byte[] Data;

        public unsafe void Init(byte[] data)
        {
            this.Data = data;
            this.ParseEquipData(data);
        }

        public bool Contains(int equipId)
        {
            for (var i = 0; i < Equips.Length; i++)
            {
                if (Equips[i] != null && Equips[i].Id == equipId)
                {
                    return true;
                }
            }

            return false;
        }

        private unsafe void ParseEquipData(byte[] data)
        {
            fixed (byte* pt = this.Data)
            {
                for (var i = 0; i < Equips.Length; i++)
                {
                    var itemId = *(int*)(pt + i * sizeof(int));
                    if (itemId > 0)
                        Equips[i] = ItemManager.Instance.Items[itemId];
                    else 
                        Equips[i] = null;
                }
            }
        }

        public void OnEquipItem(Item equip)
        {
            if (Equips[(int)equip.EquipInfo.Slot] != null && this.Equips[(int)equip.EquipInfo.Slot].Id == equip.Id)
            {
                return;
            }

            this.Equips[(int)equip.EquipInfo.Slot] = ItemManager.Instance.Items[equip.Id];
            if (OnEquipChanged != null)
                OnEquipChanged();
        }

        public void OnUnEquipItem(EquipSlot slot)
        {
            if (Equips[(int)slot] == null) return;
            Equips[(int)slot] = null;
            OnEquipChanged?.Invoke();
        }


        public unsafe byte[] GetEquipData()
        {
            fixed (byte* pt = Data)
            {
                for (int i = 0; i < (int)EquipSlot.SlotMax; i++)
                {
                    var itemId = (int*)(pt + i * sizeof(int));
                    (*itemId) = Equips[i] == null ? 0 : Equips[i].Id;
                }
            }

            return Data;
        }

        public void EquipItem(Item equip)
        {
            ItemService.Instance.SendEquipItem(equip, true);
        }

        public void UnEquipItem(Item equip)
        {
            ItemService.Instance.SendEquipItem(equip, false);
        }


        public Item GetEquip(EquipSlot slot)
        {
            return Equips[(int)slot];
        }

        public List<EquipDefine> GetEquipDefines()
        {
            List<EquipDefine> result = new List<EquipDefine>();
            for (int i = 0; i < (int)EquipSlot.SlotMax; i++)
            {
                if (Equips[i] != null)
                    result.Add(Equips[i].EquipInfo);
            }
            return result;
        }

    }
}
