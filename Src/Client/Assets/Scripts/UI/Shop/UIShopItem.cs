using Common.Data;
using Managers;
using SkillBridge.Message;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIShopItem:MonoBehaviour ,ISelectHandler
{
    public Text Title;
    private UIShop shop;
    public int ShopItemID { get; set; }
    private ShopItemDefine ShopItem;
    private ItemDefine item;
    public Image Icon;
    public Text Count;
    public Text Price;
    private bool selected;
    public Image Background;
    public Sprite SelectedBg;
    public Sprite NormalBg;
    public Text LimitClass;
    public bool Selected
    {
        get => selected;
        set
        {
            selected = value;
            Background.overrideSprite = selected ? SelectedBg : NormalBg;
        }
    }

    public void SetShopItem(int id, ShopItemDefine shopItem, UIShop owner)
    {
        this.shop = owner;
        this.ShopItemID = id;
        this.ShopItem = shopItem;
        this.item = DataManager.Instance.Items[this.ShopItem.ItemID];

        this.Title.text = this.item.Name;
        this.Count.text = "X" + ShopItem.Count;
        this.Price.text = ShopItem.Price.ToString();
        
        this.Icon.overrideSprite = Resloader.Load<Sprite>(item.Icon);
        if (this.item.Type == ItemType.Equip)
            this.LimitClass.text = this.item.LimitClass.ToString();
    }

    public void OnSelect(BaseEventData eventData)
    {
        this.Selected = true;
        this.shop.SelectShopItem(this);
    }
}