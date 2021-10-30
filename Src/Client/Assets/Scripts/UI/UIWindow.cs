using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIWindow : MonoBehaviour
{
    public delegate void CloseHandler(UIWindow sender,WindowResult result);
    public event CloseHandler OnClose;

    public virtual System.Type Type { get { return this.GetType(); } }

    public GameObject Root;

    public enum WindowResult
    {
        None=0,
        Yes,
        No,
    }

    public void Close(WindowResult result =WindowResult.None)
    {
        //SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Win_Close);
        UIManager.Instance.Close(Type);
        if (OnClose!=null)
        {
            OnClose(this,result);
        }
        OnClose = null;
    }
    public virtual void OnCloseClick()
    {
        this.Close();
    }
    public virtual void OnYesClick()
    {
        Close(WindowResult.Yes);
    }
    public virtual void OnNoClick()
    {
        Close(WindowResult.No);
    }
    void OnMouseDown()
    {
        Debug.LogFormat(name + "Clicked");
    }
}
