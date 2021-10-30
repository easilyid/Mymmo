using SkillBridge.Message;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildInfo : MonoBehaviour
{
    public Text GuildName;
    public Text GuildID;
    public Text Leader;
    public Text Notice;
    public Text MemberNumber;

    private NGuildInfo info;

    public NGuildInfo Info
    {
        get => this.info;
        set
        {
            this.info = value;
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        if (info==null)
        {
            GuildName.text = "无";
            GuildID.text = "ID：0";
            Leader.text = "会长：无";
            Notice.text = "";
            MemberNumber.text = $"成员数量：0";
        }
        else
        {
            GuildName.text = Info.GuildName;
            GuildID.text = "ID：" + Info.Id;
            Leader.text = "会长：" + Info.leaderName;
            Notice.text = Info.Notice;
            MemberNumber.text = $"成员数量：{Info.memberCount}";
        }
    }
}
