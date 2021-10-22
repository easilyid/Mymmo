using System.Collections.Generic;
using Managers;
using Models;
using SkillBridge.Message;
using UnityEngine;
using UnityEngine.UI;

public class UICharEquip : UIWindow
{
    public Text Money;
    public GameObject ItemPrefab;
    public GameObject ItemEquippedPrefab;
    public Transform ItemListRoot;
    public List<Transform> Slots;


    private void Start()
    {
        RefreshUI();
        EquipManager.Instance.OnEquipChanged += RefreshUI;
    }

    private void OnDestroy()
    {
        EquipManager.Instance.OnEquipChanged -= RefreshUI;
    }


    private void RefreshUI()
    {
        ClearAllEquipList();
        InitAllEquipItems();
        ClearEquippedList();
        InitEquippedItems();
        Money.text = User.Instance.CurrentCharacter.Gold.ToString();
    }

    private Queue<Item> items = new Queue<Item>();

    /// <summary>
    /// 初始化所有装备列表
    /// </summary>
    private void InitAllEquipItems()
    {
        foreach (var kv in ItemManager.Instance.Items)
        {
            if (kv.Value.Define.Type==ItemType.Equip && kv.Value.Define.LimitClass == User.Instance.CurrentCharacter.Class)
            {
                if (EquipManager.Instance.Contains(kv.Key))
                {
                    continue;
                }

                var go = Instantiate(ItemPrefab,ItemListRoot);
                var ui = go.GetComponent<UIEquipItem>();
                items.Enqueue(ui.item);
                ui.SetEquipItem(kv.Key, kv.Value, this, false);
            }
        }
    }


    private void ClearAllEquipList()
    {
        foreach (var item in ItemListRoot.GetComponentsInChildren<UIEquipItem>())
        {
            Destroy(item.gameObject);
        }
    }


    private void InitEquippedItems()
    {
        for (int i = 0; i < (int)EquipSlot.SlotMax; i++)
        {
            var item = EquipManager.Instance.Equips[i];
            if (item!=null)
            {
                var go = Instantiate(ItemPrefab, Slots[i]);
                var ui = go.GetComponent<UIEquipItem>();
                for (int j = 0; j < ui.transform.childCount; j++)
                {
                    ui.transform.GetChild(j).gameObject.SetActive(false);
                }
                ui.transform.GetChild(0).gameObject.SetActive(true);
                ui.SetEquipItem(i,item,this,true);
            }
        }
    }

    private void ClearEquippedList()
    {
        foreach (var item in Slots)
        {
            if (item.childCount>0)
            {
                Destroy(item.GetChild(0).gameObject);
            }
        }
    }

    public void AllEquip()
    {
        for (var i = 0; i < items.Count; i++)
        {
            DoEquip(items.Dequeue());
        }
    }
    public void DoEquip(Item item)
    {
        EquipManager.Instance.EquipItem(item);
    }

    public void UnEquip(Item item)
    {
        EquipManager.Instance.UnEquipItem(item);
    }
}