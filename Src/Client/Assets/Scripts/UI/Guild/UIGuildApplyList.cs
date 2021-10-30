using Managers;
using Services;
using UnityEngine;

class UIGuildApplyList : UIWindow
{
    public GameObject ItemPrefab;
    public ListView ListMain;
    public Transform itemRoot;
    private void Start()
    {
        GuildService.Instance.OnGuildUpdate += UpdateList;
        GuildService.Instance.SendGuildListRequest();
        UpdateList();
    }

    private void OnDestroy()
    {
        GuildService.Instance.OnGuildUpdate -= UpdateList;

    }

    private void UpdateList()
    {
        ClearList();
        InitItems();
    }

    private void InitItems()
    {
        if (!GuildManager.Instance.HasGuild)
        {
            return;
        }
        foreach (var item in GuildManager.Instance.GuildInfo.Applies)
        {
            var go = Instantiate(ItemPrefab, ListMain.transform);
            var ui = go.GetComponent<UIGuildApplyItem>();
            ui.SetItemInfo(item);
            ListMain.AddItem(ui);
        }
    }

    private void ClearList()
    {
        ListMain.RemoveAll();
    }
}

