using Services;
using SkillBridge.Message;
using UnityEngine.UI;

public class UIGuildApplyItem : ListView.ListViewItem
{
    public Text Nickname;
    public Text Class;
    public Text Level;

    public NGuildApplyInfo Info;

    //设置申请信息
    public void SetItemInfo(NGuildApplyInfo item)
    {
        this.Info = item;
        if (this.Nickname != null) this.Nickname.text = this.Info.Name;
        if (this.Class != null) this.Class.text = this.Info.Class.ToString();
        if (this.Level != null) this.Level.text = this.Info.Level.ToString();
    }

    //点击通过按钮
    public void OnAccept()
    {
        MessageBox.Show($"要通过【{this.Info.Name}】的公会申请吗", "审批申请", MessageBoxType.Confirm, "通过", "取消").OnYes = () =>
        {
            //发送加入公会审批
            GuildService.Instance.SendGuildJoinApply(true, this.Info);
        };
    }

    //点击拒绝按钮
    public void OnDecline()
    {
        MessageBox.Show($"要拒绝【{this.Info.Name}】的公会申请吗", "审批申请", MessageBoxType.Confirm, "拒绝", "取消").OnYes = () =>
        {
            GuildService.Instance.SendGuildJoinApply(false, this.Info);
        };
    }


}
