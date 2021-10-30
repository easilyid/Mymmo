using Common.Utils;
using SkillBridge.Message;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildMemberItem : ListView.ListViewItem
{
    public Text Nickname;
    public Text Class;
    public Text Level;
    public Text Title;
    public Text JoinTime;
    public Text Status;

    public Image Background;
    public Sprite NormalBg;
    public Sprite SelectedBg;
    //设置背景
    public override void OnSelected(bool selected)
    {
        this.Background.overrideSprite = selected ? SelectedBg : NormalBg;
    }

    public NGuildMemberInfo Info;

    public void SetGuildMemberInfo(NGuildMemberInfo item)
    {
        this.Info = item;
        if (this.Nickname != null) this.Nickname.text = this.Info.Info.Name;
        if (this.Class != null) this.Class.text = this.Info.Info.Class.ToString();
        if (this.Level != null) this.Level.text = this.Info.Info.Level.ToString();
        if (this.Title != null) this.Title.text = this.Info.Title.ToString();
        if (this.JoinTime != null) this.JoinTime.text = TimeUtil.GetTime(this.Info.joinTime).ToShortDateString();
        if (this.Status != null) this.Status.text = this.Info.Status == 1 ? "在线" : TimeUtil.GetTime(this.Info.lastTime).ToShortDateString();

    }

}
