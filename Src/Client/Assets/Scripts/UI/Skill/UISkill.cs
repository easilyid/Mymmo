using Common.Battle;
using Models;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISkill : UIWindow
{
    public Text descript;
    public GameObject ItemPrefab;
    public ListView listMain;
    public UISkillItem selectedItem;

    private void Start()
    {
        RefreshUI();
        this.listMain.OnItemSelected += OnItemSelected;
    }

    private void OnDestroy()
    {

    }

    private void OnItemSelected(ListView.ListViewItem item)
    {
        selectedItem = item as UISkillItem;
        this.descript.text = this.selectedItem.item.Define.Description;
    }

    private void RefreshUI()
    {
        ClearItems();
        InitItems();
    }
    /// <summary>
    /// 初始化所有技能列表
    /// </summary>
    private void InitItems()
    {
        var Skills = User.Instance.CurrentCharacter.SkillMgr.Skills;
        foreach (var skill in Skills)
        {
            if (skill.Define.Type == SkillType.Skill)
            {
                var go = Instantiate(ItemPrefab, listMain.transform);
                var ui = go.GetComponent<UISkillItem>();
                ui.SetItem(skill,this,false);
                listMain.AddItem(ui);
            }
        }
    }

    private void ClearItems()
    {
        this.listMain.RemoveAll();
    }

}
