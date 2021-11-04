using Models;
using UnityEngine;
using UnityEngine.UI;

public class UIRideItem:ListView.ListViewItem
{
    public Image Icon;
    public Image Background;
    public Text Title;
    public Text Level;
    public Sprite NormalBg;
    public Sprite SelectBg;
    public Item Item;

    public override void OnSelected(bool selected)
    {
        this.Background.overrideSprite = selected ? SelectBg : NormalBg;
    }

    public void SetEquipItem(Item item, UIRide owner, bool equipped)
    {
        this.Item = item;
        if (this.Title!=null)
        {
            this.Title.text = this.Item.Define.Name;
        }
        if (this.Level != null)
        {
            this.Level.text = this.Item.Define.Level.ToString();
        }
        if (this.Icon != null)
        {
            this.Icon.overrideSprite = Resloader.Load<Sprite>(this.Item.Define.Icon);
        }
    }
}