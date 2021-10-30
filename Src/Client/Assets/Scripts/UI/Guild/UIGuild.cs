using System;
using Managers;
using Services;
using SkillBridge.Message;
using UnityEngine;

public class UIGuild : UIWindow
{
    public UIGuildInfo UiInfo;
    public ListView ListMain;
    public Transform ItemRoot;
    public UIGuildMemberItem SelectedItem;
    public GameObject ItemPrefab;
    /// <summary>
    /// 管理者
    /// </summary>
    public GameObject PanelAdmin;
    /// <summary>
    /// 会长
    /// </summary>
    public GameObject PanelLeader;

    private void Start()
    {
        GuildService.Instance.OnGuildUpdate += UpdateUI;
        ListMain.OnItemSelected += OnGuildMemberSelected;
        UpdateUI();
    }

    private void OnDestroy()
    {
        GuildService.Instance.OnGuildUpdate -= UpdateUI;
    }
    private void UpdateUI()
    {
        UiInfo.Info = GuildManager.Instance.GuildInfo;
        ClearList();
        InitItems();


        this.PanelAdmin.SetActive(GuildManager.Instance.MyMemberInfo.Title > GuildTitle.None);
        this.PanelLeader.SetActive(GuildManager.Instance.MyMemberInfo.Title == GuildTitle.President);

    }


    private void OnGuildMemberSelected(ListView.ListViewItem item)
    {
        SelectedItem = item as UIGuildMemberItem;
    }


    private void InitItems()
    {
        foreach (var item in GuildManager.Instance.GuildInfo.Members)
        {
            var go = Instantiate(ItemPrefab, ListMain.transform);
            var ui = go.GetComponent<UIGuildMemberItem>();
            ui.SetGuildMemberInfo(item);
            ListMain.AddItem(ui);
        }
    }

    private void ClearList()
    {
        ListMain.RemoveAll();
    }

    public void OnClickAppliesList()
    {
        UIManager.Instance.Show<UIGuildApplyList>();
    }

    public void OnClickLeave()
    {
        if (SelectedItem.Info == null)
        {
            MessageBox.Show("请选择要离开的公会");
            return;
        }

        MessageBox.Show($"要离开[{SelectedItem.Info.Info.Guild}]公会吗？", "离开公会", MessageBoxType.Confirm, "确定", "取消")
            .OnYes = () =>
        {
            GuildService.Instance.SendGuildLeaveRequest();
        };

    }
    public void OnClickChat()
    {
        MessageBox.Show("暂未开发");
    }

    public void OnClickKickOut()
    {
        if (SelectedItem.Info == null)
        {
            MessageBox.Show("请选择要踢出的成员");
        }

        if (SelectedItem.Info.Title == GuildTitle.President)
        {
            MessageBox.Show("你不能踢出会长");
        }

        MessageBox.Show(String.Format("要踢[{0}]出公会么？", this.SelectedItem.Info.Info.Name), "踢出公会", MessageBoxType.Confirm,
            "确定", "取消").OnYes = (
            () =>
            {
                GuildService.Instance.SendAdminCommand(GuildAdminCommand.Kickout, this.SelectedItem.Info.Info.Id);
            });

    }

    public void OnClickPromote()
    {
        if (SelectedItem == null)
        {
            MessageBox.Show("选择要晋升的成员");
            return;
        }

        if (SelectedItem.Info.Title != GuildTitle.None)
        {
            MessageBox.Show("对方已经身份尊贵");
            return;
        }

        MessageBox.Show(String.Format("要晋升[{0}]为公会副会长么？", this.SelectedItem.Info.Info.Name), "晋升", MessageBoxType.Confirm, "确定", "取消").OnYes = (
            () =>
            {
                GuildService.Instance.SendAdminCommand(GuildAdminCommand.Promote, this.SelectedItem.Info.Info.Id);
            });

    }

    public void OnClickDepose()
    {
        if (SelectedItem == null)
        {
            MessageBox.Show("选择要罢免的成员");
            return;
        }

        if (SelectedItem.Info.Title != GuildTitle.None)
        {
            MessageBox.Show("对方无职可免");
            return;
        }
        if (SelectedItem.Info.Title != GuildTitle.President)
        {
            MessageBox.Show("大胆你敢动会长？");
            return;
        }


        MessageBox.Show(String.Format("要罢免[{0}]的职务么？", this.SelectedItem.Info.Info.Name), "罢免职务", MessageBoxType.Confirm, "确定", "取消").OnYes = (
            () =>
            {
                GuildService.Instance.SendAdminCommand(GuildAdminCommand.Depost, this.SelectedItem.Info.Info.Id);
            });


    }

    public void OnClickTransfer()
    {
        if (SelectedItem.Info == null)
        {
            MessageBox.Show("请选择把会长转让的成员的");
            return;
        }

        MessageBox.Show(String.Format("要转让会长给[{0}]么？", this.SelectedItem.Info.Info.Name), "转让会长", MessageBoxType.Confirm, "确定", "取消").OnYes = (
            () =>
            {
                GuildService.Instance.SendAdminCommand(GuildAdminCommand.Transfer, this.SelectedItem.Info.Info.Id);
            });

    }

    public void OnClickNotice()
    {
        if (SelectedItem.Info.Title==GuildTitle.President)
        {
            MessageBox.Show("你不能修改公会宣言哦");
        }

        MessageBox.Show($"要修改宣言吗？", "宣言", MessageBoxType.Confirm, "确定", "取消")
            .OnYes = () => { UIManager.Instance.Show<UIChangeNewNotice>(); };
    }

    public void OnClickPrivate()
    {

    }

}
