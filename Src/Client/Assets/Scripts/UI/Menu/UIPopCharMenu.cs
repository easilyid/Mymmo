using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPopCharMenu : UIWindow,IDeselectHandler
{
    public int TargetId;
    public string TargetName;


    public void OnDeselect(BaseEventData eventData)
    {
        if (eventData is PointerEventData ed && ed.hovered.Contains(gameObject))
        {
            return;
        }
        Close(WindowResult.None);
    }

    private void OnEnable()
    {
        GetComponent<Selectable>().Select();
        Root.transform.position = Input.mousePosition + new Vector3(80, 0, 0);
    }

    public void OnChat()
    {
        ChatManager.Instance.StartPrivateChat(TargetId, TargetName);
        Close(WindowResult.No);
    }

    public void OnAddFriend()
    {
        Close(WindowResult.No);
    }

    public void OnInviteTeam()
    {
        Close(WindowResult.No);
    }

}
