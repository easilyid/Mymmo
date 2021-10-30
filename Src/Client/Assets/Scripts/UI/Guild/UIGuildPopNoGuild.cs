using Managers;
using UnityEngine;

public class UIGuildPopNoGuild : UIWindow
{
    public void ShowCreateGuild()
    {
        UIManager.Instance.Show<UIGuildPopCreate>();
    }

    public void ShowJoinGuild()
    {
        UIManager.Instance.Show<UIGuildList>();
    }
}
