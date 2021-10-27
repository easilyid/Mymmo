using System;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Managers;
using Common.Data;

public class UIQuestSystem : UIWindow
{
    public Text title;
    public GameObject itemPrefab;

    public TabView Tabs;
    public ListView listMain;
    public ListView listBranch;

    public UIQuestInfo questInfo;


    private bool showAvailableList = false;

    private void Start()
    {
        this.listMain.OnItemSelected += this.OnQuestSelected;
        this.listBranch.OnItemSelected += this.OnQuestSelected;
        this.Tabs.OnTabSelect += OnSelectTab;
        RefreshUI();

    }
    void OnSelectTab(int idx)
    {
        showAvailableList = idx == 1;
        RefreshUI();
    }



  
    void RefreshUI()
    {
        ClearAllQuestList();
        InitAllQuestItems();
    }

 

    void InitAllQuestItems()
    {
        foreach (var kv in QuestManager.Instance.AllQuests)
        {
            if (showAvailableList)
            {
                if (kv.Value.Info!=null)
                {
                    continue;
                }
            }
            else
            {
                if (kv.Value.Info==null)
                {
                    continue;
                }
            }
            GameObject go = Instantiate(itemPrefab,kv.Value.Define.Type==QuestType.Main?this.listMain.transform:this.listBranch.transform);
            UIQuestItem ui = go.GetComponent<UIQuestItem>();
            ui.SetQuestInfo(kv.Value);
            if (kv.Value.Define.Type==QuestType.Main)
            {
                this.listMain.AddItem(ui);
            }
            else
            {
                this.listBranch.AddItem(ui);
            }
        }
       
    }
    void ClearAllQuestList()
    {
        this.listMain.RemoveAll();
        this.listBranch.RemoveAll();
    }
    public void OnQuestSelected(ListView.ListViewItem item)//任务面板根据item显示信息
    {
        if (item.Owner==listMain)
        {
            if (listBranch.SelectedItem)
            {
                listBranch.SelectedItem.OnSelected(false);
                RefreshUI();
            }
        }
        if (item.Owner==listBranch)
        {
            if (listMain.SelectedItem)
            {
                listMain.SelectedItem.OnSelected(false);
                RefreshUI();
            }
        }
        UIQuestItem questItem = item as UIQuestItem;
        this.questInfo.SetQuestInfo(questItem.quest);
    }
}
