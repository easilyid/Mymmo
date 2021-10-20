using UnityEngine;
using UnityEngine.UI;

public class TabButton : MonoBehaviour
{
    public TabView TabView;
    public int TabIndex = 0;
    private Image tabImage;
    public Sprite activeImage;
    private Sprite normalImage;

    private void Start()
    {
        tabImage = GetComponent<Image>();
        normalImage = tabImage.sprite;

        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void Select(bool select)
    {
        tabImage.overrideSprite = select ? activeImage : normalImage;
    }
    private void OnClick()
    {
        TabView.SelectTab(TabIndex);
    }

}