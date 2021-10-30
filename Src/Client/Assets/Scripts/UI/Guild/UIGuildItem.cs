using SkillBridge.Message;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildItem: ListView.ListViewItem
{
    public NGuildInfo Info;

    public Text GuildId;
    public Text GuildName;
    public Text MemberCount;
    public Text LeaderName;

    public Image Background;
    public Sprite NormalBg;
    public Sprite SelectedBg;

    public override void OnSelected(bool selected)
    {
        this.Background.overrideSprite = selected ? SelectedBg : NormalBg;
    }

    public void SetGuildInfo(NGuildInfo item)
    {
        this.Info = item;
        if (this.GuildId != null) this.GuildId.text = this.Info.Id.ToString();
        if (this.GuildName != null) this.GuildName.text = this.Info.GuildName;
        if (this.MemberCount != null) this.MemberCount.text = this.Info.memberCount.ToString();
        if (this.LeaderName != null) this.LeaderName.text = this.Info.leaderName;
    }
}