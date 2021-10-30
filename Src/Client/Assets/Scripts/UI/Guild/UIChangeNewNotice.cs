using Services;
using UnityEngine.UI;

class UIChangeNewNotice : UIWindow
{
    public InputField InputNotice;
    
    private void Start()
    {
        GuildService.Instance.OnGuildChangeNoticeResult = OnGuildNoticeChanged;
    }

    private void OnDestroy()
    {
        GuildService.Instance.OnGuildChangeNoticeResult = null;
    }

    private void OnGuildNoticeChanged(bool result)
    {
        if (result)
        {
            Close(WindowResult.Yes);
        }
    }

    public override void OnYesClick()
    {
        
        if (string.IsNullOrEmpty(InputNotice.text))
        {
            MessageBox.Show("请输入公会宣言", "错误", MessageBoxType.Error);
            return;
        }
        if (InputNotice.text.Length < 3 || InputNotice.text.Length > 50)
        {
            MessageBox.Show("公会宣言3-50个字符", "错误", MessageBoxType.Error);
            return;
        }

        GuildService.Instance.SendGuildChangeNotice(InputNotice.text);
    }
}

