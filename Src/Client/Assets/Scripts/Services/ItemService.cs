using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Managers;
using Models;
using Network;
using SkillBridge.Message;
using UnityEngine;

namespace Services
{
    class ItemService : Singleton<ItemService>, IDisposable
    {
        private bool isEquip;
        private Item pendingEquip;
        public ItemService()
        {
            MessageDistributer.Instance.Subscribe<ItemBuyResponse>(this.OnItemBuy);
            MessageDistributer.Instance.Subscribe<ItemEquipResponse>(OnItemEquip);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<ItemBuyResponse>(this.OnItemBuy);
            MessageDistributer.Instance.Unsubscribe<ItemEquipResponse>(OnItemEquip);
        }
        public void SendBuyItem(int shopId, int shopItemId)
        {
            Debug.Log("SendBuyItem");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.itemBuy = new ItemBuyRequest();
            message.Request.itemBuy.shopId = shopId;
            message.Request.itemBuy.shopItemId = shopItemId;
            NetClient.Instance.SendMessage(message);
        }
        private void OnItemBuy(object sender, ItemBuyResponse message)
        {
            MessageBox.Show("购买结果：" + message.Result + "\n" + message.Errormsg, "购买完成");
        }
        private void OnItemEquip(object sender, ItemEquipResponse response)
        {
            if (response.Result == Result.Success)
            {
                if (pendingEquip != null)
                {
                    if (this.isEquip)
                        EquipManager.Instance.OnEquipItem(pendingEquip);
                    else
                        EquipManager.Instance.OnUnEquipItem(pendingEquip.EquipInfo.Slot);
                    pendingEquip = null;
                }
            }
        }

        public bool SendEquipItem(Item equip, bool isEquip)
        {
            if (pendingEquip != null)
                return false;
            Debug.Log("SendEquipItem");

            pendingEquip = equip;
            this.isEquip = isEquip;

            var message = new NetMessage
            {
                Request = new NetMessageRequest
                {
                    itemEquip = new ItemEquipRequest
                    {
                        Slot = (int)equip.EquipInfo.Slot,
                        itemId = equip.Id,
                        isEquip = isEquip
                    }
                }
            };
            NetClient.Instance.SendMessage(message);
            return true;
        }

    }
}
