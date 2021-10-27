using System.Collections;
using Common.Data;
using Managers;
using Models;
using UnityEngine;
using UnityEngine.UI;

public class UIShop : UIWindow
{
    public GameObject ShopItem;
    public Transform[] ItemRoot;
    private ShopDefine shop;
    public Text Title;
    public Text Money;
    private UIShopItem selectedItem;

    private void Start()
    {
        StartCoroutine(InitItems());
    }

    private void Update()
    {
        this.Money.text = User.Instance.CurrentCharacter.Gold.ToString();
        this.InitItems();
    }
    private IEnumerator InitItems()
    {
        int count = 0;
        int page = 0;
        //遍历配置表中的商店ID
        foreach (var kv in DataManager.Instance.ShopItems[shop.ID])
        {
            if (kv.Value.Status > 0)
            {
                var go = Instantiate(ShopItem, ItemRoot[page]);
                var ui = go.GetComponent<UIShopItem>();
                ui.SetShopItem(kv.Key, kv.Value, this);
                count++;
                //商店翻页，道具超过10个翻页
                if (count >= 10)
                {
                    count = 0;
                    page++;
                    ItemRoot[page].gameObject.SetActive(true);
                }
            }
        }

        yield return null;
    }

    public void SetShop(ShopDefine shop)
    {
        this.shop = shop;
        Title.text = shop.Name;
        Money.text = User.Instance.CurrentCharacter.Gold.ToString();
    }

    public void SelectShopItem(UIShopItem uiShopItem)
    {
        if (selectedItem!=null)
        {
            selectedItem.Selected = false;
        }

        selectedItem = uiShopItem;
    }

    public void OnClickBuy()
    {
        if (selectedItem==null)
        {
            MessageBox.Show("请选择要购买的道具", "购买提示");
            return;
        }
        //发送商店ID及选中的道具ID
        if (!ShopManager.Instance.BuyItem(this.shop.ID, this.selectedItem.ShopItemID))
        {


        }
    }
}