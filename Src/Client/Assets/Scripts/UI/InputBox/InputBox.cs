using UnityEngine;
using static UnityEngine.Object;

public class InputBox
{
    private static Object cacheObject;

    public static UIInputBox Show(string message, string title="",string btnOk="",string btnCancel="",string emptyTips="")
    {
        if (cacheObject ==null)
        {
            cacheObject = Resloader.Load<Object>("UI/UIInputBox");
        }

        var go = (GameObject)Instantiate(cacheObject);
        var inputBox = go.GetComponent<UIInputBox>();
        inputBox.Init(title, message, btnOk, btnCancel, emptyTips);
        return inputBox;
    }
}
