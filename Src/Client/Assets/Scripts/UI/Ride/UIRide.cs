using Managers;
using Models;
using SkillBridge.Message;
using UnityEngine;
using UnityEngine.UI;

public class UIRide : UIWindow
{
    public ListView listMain;
    public Text Description;
    public GameObject ItemPrefab;
    private UIRideItem selectedItem;

    private void Start()
    {
        RefreshUI();
        listMain.OnItemSelected += OnItemSelected;
    }

   

    private void RefreshUI()
    {
        ClearItems();
        InitItems();
    }

    private void InitItems()
    {
        foreach (var kv in ItemManager.Instance.Items)
        {
            if (kv.Value.Define.Type == ItemType.Ride 
                &&(kv.Value.Define.LimitClass==CharacterClass.None || kv.Value.Define.LimitClass == User.Instance.CurrentCharacter.Class))
            {
                var go = Instantiate(ItemPrefab,listMain.transform);
                var ui = go.GetComponent<UIRideItem>();
                ui.SetEquipItem(kv.Value,this,false);
                listMain.AddItem(ui);
            }
        }
    }


    private void ClearItems()
    {
        listMain.RemoveAll();
    }


    private void OnItemSelected(ListView.ListViewItem item)
    {
        selectedItem = item as UIRideItem;
        if (selectedItem != null) this.Description.text = selectedItem.Item.Define.Description;
    }

    public void DoRide()
    {
        if (this.selectedItem == null)
        {
            MessageBox.Show("请选择要召唤的坐骑", "提示");
            return;
        }
        User.Instance.Ride(selectedItem.Item.Id);
    }

}