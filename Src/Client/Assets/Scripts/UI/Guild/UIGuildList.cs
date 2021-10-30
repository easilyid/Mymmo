using System.Collections.Generic;
using Services;
using SkillBridge.Message;
using UnityEngine;

public class UIGuildList : UIWindow
{
    public GameObject itemPrefab;
    public ListView ListMain;
    public Transform itemRoot;
    public UIGuildInfo UiInfo;

    private void Start()
    {
        ListMain.OnItemSelected += OnGuildMemberSelected;
        UiInfo.Info = null;
        GuildService.Instance.OnGuildListResult += UpdateGuildList;
        GuildService.Instance.SendGuildListRequest();
    }

    private void OnDestroy()
    {
        GuildService.Instance.OnGuildListResult -= UpdateGuildList;
    }

    private void UpdateGuildList(List<NGuildInfo> guilds)
    {
        ClearList();
        InitItems(guilds);
    }

    private void OnGuildMemberSelected(ListView.ListViewItem item)
    {
        SelectedItem = item as UIGuildItem;
        if (SelectedItem != null) UiInfo.Info = SelectedItem.Info;
    }

    public UIGuildItem SelectedItem;

    /// <summary>
    /// 初始化列表
    /// </summary>
    /// <param name="guilds"></param>
    private void InitItems(List<NGuildInfo> guilds)
    {
        foreach (var item in guilds)
        {
            var go = Instantiate(itemPrefab,ListMain.transform);
            var ui = go.GetComponent<UIGuildItem>();
            ui.SetGuildInfo(item);
            ListMain.AddItem(ui);
        }
    }
    private void ClearList()
    {
        ListMain.RemoveAll();
    }

    public void OnClickJoin()
    {
        if (SelectedItem==null)
        {
            MessageBox.Show("请选择要加入的公会");
            return;
        }

        MessageBox.Show($"确定要加入公会【{SelectedItem.Info.GuildName}】吗？", "申请加入公会", MessageBoxType.Confirm, "申请加入", "取消加入")
                .OnYes =
            () => { GuildService.Instance.SendGuildJoinRequest(SelectedItem.Info.Id); };

    }

} 