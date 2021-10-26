using SkillBridge.Message;
using UnityEngine;
using UnityEngine.UI;

public class UIFriendItem:ListView.ListViewItem
{
    public Image Background;
    public Sprite SelectedBg;
    public Sprite NormalBg;
    public NFriendInfo Info;
    public Text NickName;
    public Text Class;
    public Text Level;
    public Text Status;

    public override void OnSelected(bool selected)
    {
        this.Background.overrideSprite = selected ? SelectedBg : NormalBg;
    }

    public void SetFriendInfo(NFriendInfo item)
    {
        this.Info = item;
        if (NickName!=null)
        {
            NickName.text = Info.friendInfo.Name;
        }
        if (Class!=null)
        {
            Class.text = Info.friendInfo.Class.ToString();
        }
        if (Level!=null)
        {
            Level.text = Info.friendInfo.Level.ToString();
        }
        if (Status!=null)
        {
            Status.text = Info.Status == 1 ? "在线" : "离线";
        }
    }
}