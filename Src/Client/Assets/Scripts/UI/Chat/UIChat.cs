using Candlelight.UI;
using Managers;
using UnityEngine;
using UnityEngine.UI;

public class UIChat : MonoBehaviour
{
    public TabView ChannelTab;
    public HyperText TextArea;
    public Text ChatTarget;
    public Dropdown ChannelSelect;
    public InputField ChatText;

    private void Start()
    {
        this.ChannelTab.OnTabSelect += OnDisplayChannelSelected;
        ChatManager.Instance.OnChat += RefreshUI;
    }

   

    private void OnDestroy()
    {
        ChatManager.Instance.OnChat -= RefreshUI;
    }

   

    private void Update()
    {
        InputManager.Instance.IsInputMode = ChatText.isFocused;
    }


    private void OnDisplayChannelSelected(int idx)
    {
        ChatManager.Instance.DisplayChannel = (ChatManager.LocalChannel) idx;
        RefreshUI();
    }

    private void RefreshUI()
    {
        this.TextArea.text  = ChatManager.Instance.GetCurrentMessages();
        this.ChannelSelect.value = (int) ChatManager.Instance.SendChannel - 1;
        if (ChatManager.Instance.SendChannel == SkillBridge.Message.ChatChannel.Private)
        {
            ChatTarget.gameObject.SetActive(true);
            if (ChatManager.Instance.PrivateID!=0)
            {
                ChatTarget.text = ChatManager.Instance.PrivateName + ":";
            }
            else
            {
                ChatTarget.text = "<无>:";
            }
        }
        else
        {
            ChatTarget.gameObject.SetActive(false);
        }
    }

    public void OnClickChatLink(HyperText text, HyperText.LinkInfo link)
    {
        if (string.IsNullOrEmpty(link.Name))
        {
            return;
        }
        //约定
        //<a name="c:1001:Name" class="player">Name</a>
        //<a name="i:1001:Name" class="item">Name</a>
        if (link.Name.StartsWith("c:"))
        {
            var strs = link.Name.Split(":".ToCharArray());
            var menu = UIManager.Instance.Show<UIPopCharMenu>();
            menu.TargetId = int.Parse(strs[1]);
            menu.TargetName = strs[2];
        }
    }

    public void OnClickSend()
    {
        OnEndInput(ChatText.text);
    }

    public void OnEndInput(string text)
    {
        if (!string.IsNullOrEmpty(text.Trim()))
        {
            SendChat(text);
        }

        ChatText.text = "";
    }

    private void SendChat(string content)
    {
        ChatManager.Instance.SendChat(content, ChatManager.Instance.PrivateID, ChatManager.Instance.PrivateName);
    }


    public void OnSendChannelChanged(int idx)
    {
        if (ChatManager.Instance.sendChannel == (ChatManager.LocalChannel)(idx+1))
        {
            return;
        }

        if (!ChatManager.Instance.SetSendChannel((ChatManager.LocalChannel)idx+1))
        {
            ChannelSelect.value = (int) ChatManager.Instance.sendChannel - 1;
        }
        else
        {
            RefreshUI();
        }
    }
}
