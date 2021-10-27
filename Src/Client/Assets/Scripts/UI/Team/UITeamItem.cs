using SkillBridge.Message;
using UnityEngine;
using UnityEngine.UI;

public class UITeamItem : ListView.ListViewItem
{
    public Image Background;
    public Image ClassIcon;
    public Image LeaderIcon;
    public Text NickName;
    public int Index;
    public NCharacterInfo Info;

    public override void OnSelected(bool selected)
    {
        Background.enabled = selected ? true : false;
    }

    private void Start()
    {
        Background.enabled = false;
    }

    public void SetMemberInfo(int idx, NCharacterInfo item, bool isLeader)
    {
        this.Index = idx;
        this.Info = item;
        if (NickName!=null)
        {
            NickName.text = Info.Level.ToString().PadRight(4) + Info.Name;
        }
        if (ClassIcon != null)
        {
            ClassIcon.overrideSprite = SpriteManager.Instance.ClassIcons[(int) Info.Class - 1];
        }
        if (LeaderIcon != null)
        {
            LeaderIcon.gameObject.SetActive(isLeader);
        }
    }

}
