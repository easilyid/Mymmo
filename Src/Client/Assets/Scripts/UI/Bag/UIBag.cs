using System.Collections;
using System.Collections.Generic;
using Managers;
using Models;
using UnityEngine;
using UnityEngine.UI;

public class UIBag : UIWindow
{
    private List<Image> slots;
    public Transform[] Pages;
    public GameObject BagItem;
    public Text Money;

    private void Start()
    {
        if (slots == null)
        {
            slots = new List<Image>();
            for (int page = 0; page < Pages.Length; page++)
            {
                slots.AddRange(Pages[page].GetComponentsInChildren<Image>(true));
            }

            this.SetMoney();
            //初始化背包
            StartCoroutine(InitBags());
        }
    }

    private void Update()
    {
        this.SetMoney();
    }

    private IEnumerator InitBags()
    {
        for (int i = 0; i < BagManager.Instance.Items.Length; i++)
        {
            var item = BagManager.Instance.Items[i];
            if (item.ItemId > 0)
            {
                GameObject go = Instantiate(BagItem, slots[i].transform);
                var ui = go.GetComponent<UIIconItem>();
                var def = ItemManager.Instance.Items[item.ItemId].Define;
                ui.SetMainIcon(def.Icon, item.Count.ToString());

            }
        }

        for (int i = BagManager.Instance.Items.Length; i < slots.Count; i++)
        {
            slots[i].color = Color.gray;
        }

        yield return null;
    }

    public void SetMoney()
    {
        this.Money.text = User.Instance.CurrentCharacterInfo.Gold.ToString();
    }

    void Clear()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].transform.childCount>0)
            {
                Destroy(slots[i].transform.GetChild(0).gameObject);
            }
        }
    }

    public void OnReset()
    {
        BagManager.Instance.Reset();
        this.Clear();
        StartCoroutine(InitBags());
    }

}