using UnityEngine;
using UnityEngine.UI;

public class UIIconItem : MonoBehaviour
{
    public Image MainImage;
    public Image SecondImage;
    public Text MainText;

    public void SetMainIcon(string iconName, string text)
    {
        MainImage.overrideSprite = Resources.Load<Sprite>(iconName);
        MainText.text = text;
    }
}
