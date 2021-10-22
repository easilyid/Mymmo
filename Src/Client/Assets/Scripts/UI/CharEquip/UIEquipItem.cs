using System;
using System.Collections.Generic;
using Managers;
using Models;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIEquipItem:MonoBehaviour,IPointerClickHandler
{
    public Image Background;
    public Sprite NormalBg;
    public Sprite SelectedBg;
    private bool selected;
    private bool isEquipped;
    private UICharEquip owner;
    private int index;
    public Item item;
    
    public Text Title;
    public Text Level;
    public Text LimitCategory;
    public Image Icon;
    public Text LimitClass;


    public void SetEquipItem(int idx, Item item, UICharEquip owner, bool equipped)
    {
        this.owner = owner;
        this.index = idx;
        this.item = item;
        this.isEquipped = equipped;

        if (this.Title != null)
            this.Title.text = this.item.Define.Name;
        if (this.Level != null)
            this.Level.text = item.Define.Level.ToString();
        if (this.LimitCategory != null)
            this.LimitCategory.text = item.Define.Category;
        if (this.Icon != null)
            this.Icon.overrideSprite = Resloader.Load<Sprite>(this.item.Define.Icon);
        if (this.LimitClass != null)
            this.LimitClass.text = item.Define.LimitClass.ToString();

    }

    public bool Selected
    {
        get => selected;
        set
        {
            selected = value;
            Background.overrideSprite = selected ? SelectedBg : NormalBg;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isEquipped)
        {
            UnEquip();
        }
        else
        {
            if (selected)
            {
                DoEquip();
                Selected = false;
            }
            else
            {
                Selected = true;
            }
        }
    }

    private void UnEquip()
    {
        var msg = MessageBox.Show(string.Format("要取下[{0}]吗?", this.item.Define.Name), "确认", MessageBoxType.Confirm);
        msg.OnYes = () =>
        {
            this.owner.UnEquip(this.item);
        };

    }

    private void DoEquip()
    {
        var msg = MessageBox.Show(string.Format("要装备[{0}]吗?",this.item.Define.Name),"确认",MessageBoxType.Confirm);
        msg.OnYes = () =>
        {
            var oldEquip = EquipManager.Instance.GetEquip(item.EquipInfo.Slot);
            if (oldEquip!=null)
            {
                var newmsg = MessageBox.Show(String.Format("要替换掉[{0}]吗?", oldEquip.Define.Name), "确认",
                    MessageBoxType.Confirm);
                newmsg.OnYes = () =>
                {
                    this.owner.DoEquip(this.item);
                };
            }
            else
            {
                this.owner.DoEquip(this.item);
            }
        };
    }
}