using System.Collections.Generic;
using Common.Battle;
using Common.Data;
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

    public Text hp;
    public Slider hpBar;

    public Text mp;
    public Slider mpBar;

    public Text[] attrs;

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
        Money.text = User.Instance.CurrentCharacterInfo.Gold.ToString();

        InitAttributes();
    }

    private Queue<Item> items = new Queue<Item>();

    /// <summary>
    /// 初始化所有装备列表
    /// </summary>
    private void InitAllEquipItems()
    {
        foreach (var kv in ItemManager.Instance.Items)
        {
            if (kv.Value.Define.Type==ItemType.Equip && kv.Value.Define.LimitClass == User.Instance.CurrentCharacterInfo.Class)
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

    private void InitAttributes()
    {
        var charattr = User.Instance.CurrentCharacter.Attributes;
        this.hp.text = string.Format("{0}/{1}", charattr.HP, charattr.MaxHP);
        this.mp.text = string.Format("{0}/{1}", charattr.MP, charattr.MaxMP);
        this.hpBar.maxValue = charattr.MaxHP;
        this.hpBar.value = charattr.HP;
        this.mpBar.maxValue = charattr.MaxMP;
        this.mpBar.value = charattr.MP;

        for (int i = (int)AttributeType.STR; i < (int)AttributeType.MAX; i++)
        {
            if (i == (int)AttributeType.CRI)
            {
                this.attrs[i - 2].text = string.Format("{0:f2}%", charattr.Final.Data[i] * 100);
            }
            else
            {
                this.attrs[i - 2].text = ((int)charattr.Final.Data[i]).ToString();
            }
        }
    }

}