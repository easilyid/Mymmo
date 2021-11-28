using System;
using Managers;
using Models;
using Services;
using UnityEngine;

public class UIFriends : UIWindow
{
    private UIFriendItem selectedItem;
    public ListView ListMain;
    public GameObject ItemPrefab;
    public Transform ItemRoot;

    private void Start()
    {
        FriendService.Instance.OnFriendUpdate += RefreshUI;
        this.ListMain.OnItemSelected += OnFriendSelected;
        RefreshUI();
    }

    private void OnDestroy()
    {
        FriendService.Instance.OnFriendUpdate -= RefreshUI;
    }


    public void OnFriendSelected(ListView.ListViewItem item)
    {
        selectedItem = item as UIFriendItem;
    }

    private void RefreshUI()
    {
        ClearFriendList();
        InitFriendItems();
    }

    /// <summary>
    /// 初始化所有好友列表
    /// </summary>
    private void InitFriendItems()
    {
        foreach (var item in FriendManager.Instance.allFriends)
        {
            var go = Instantiate(ItemPrefab, ListMain.transform);
            var ui = go.GetComponent<UIFriendItem>();
            ui.SetFriendInfo(item);
            ListMain.AddItem(ui);

        }
    }

    private void ClearFriendList()
    {
        ListMain.RemoveAll();
    }

    public void OnClickFriendAdd()
    {
        InputBox.Show("输入要添加的好友名称或ID", "添加好友").OnSubmit += OnFriendAddSubmit;
    }


    private bool OnFriendAddSubmit(string input, out string tips)
    {
        tips = "";
        int friendId = 0;
        string friendName = "";
        if (!int.TryParse(input, out friendId))
        {
            friendName = input;
        }

        if (friendId == User.Instance.CurrentCharacterInfo.Id || friendName == User.Instance.CurrentCharacterInfo.Name)
        {
            tips = "不能够添加自己哦！";
            return false;
        }

        FriendService.Instance.SendFriendAddRequest(friendId, friendName);
        return true;

    }

    public void OnClickFriendChat()
    {
        MessageBox.Show("暂未开放");
    }

    public void OnClickFriendRemove()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("请选择要删除的好友");
            return;
        }

        MessageBox.Show(String.Format("确定删除好友[{0}]吗？",
                selectedItem.Info.friendInfo.Name), "删除好友", MessageBoxType.Confirm, "删除", "取消").OnYes = (() =>
            {
                FriendService.Instance.SendFriendRemoveRequest(selectedItem.Info.Id, selectedItem.Info.friendInfo.Id);
            });

    }

    public void OnClickFriendTeamInvite()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("请选择要邀请的好友");
            return;
        }

        if (selectedItem.Info.Status == 0)
        {
            MessageBox.Show("请选择在线的好友");
            return;
        }

        MessageBox.Show(string.Format("确定要邀请好友[{0}]加入队伍吗?", selectedItem.Info.friendInfo.Name), "邀请好友组队",
                MessageBoxType.Confirm, "邀请", "取消").OnYes =
            () =>
            {
                TeamService.Instance.SendTeamInviteRequest(this.selectedItem.Info.friendInfo.Id,
                    this.selectedItem.Info.friendInfo.Name);
            };
    }

    public void OnClickChallenge()
    {
        if (selectedItem==null)
        {
            MessageBox.Show("请选择要挑战的好友");
            return;
        }
        if (selectedItem.Info.Status == 0)
        {
            MessageBox.Show("请选择在线的好友");
            return;
        }
        MessageBox.Show(string.Format("确定要与好友[{0}]进行竞技场挑战吗?", selectedItem.Info.friendInfo.Name), "竞技场挑战",
                MessageBoxType.Confirm, "挑战", "取消").OnYes =
            () =>
            {
                ArenaService.Instance.SendArenaChallengeRequest(this.selectedItem.Info.friendInfo.Id,
                    this.selectedItem.Info.friendInfo.Name);
            };


    }

}


