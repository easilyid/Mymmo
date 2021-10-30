using Services;
using UnityEngine.UI;

public class UIGuildPopCreate : UIWindow
{
    public InputField InputName;
    public InputField InputNotice;

    private void Start()
    {
        GuildService.Instance.OnGuildCreateResult = OnGuildCreated;
    }

    private void OnDestroy()
    {
        GuildService.Instance.OnGuildCreateResult = null;
    }

    public override void OnYesClick()
    {
        if (string.IsNullOrEmpty(InputName.text))
        {
            MessageBox.Show("请输入公会名称", "错误", MessageBoxType.Error);
            return;
        }
        if (InputName.text.Length < 4 || InputName.text.Length > 10)
        {
            MessageBox.Show("公会名称4-10个字符", "错误", MessageBoxType.Error);
            return;
        }
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

        GuildService.Instance.SendGuildCreate(InputName.text, InputNotice.text);
    }

    private void OnGuildCreated(bool result)
    {
        if (result)
        {
            Close(WindowResult.Yes);
        }
    }


}
