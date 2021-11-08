using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TabView : MonoBehaviour
{
    public TabButton[] TabButtons;
    public GameObject[] TabPages;
    public int index = -1;
    public UnityAction<int> OnTabSelect;
    IEnumerator Start()
    {
        for (var i = 0; i < TabButtons.Length; i++)
        {
            TabButtons[i].TabView = this;
            TabButtons[i].TabIndex = i;
        }
        yield return new WaitForEndOfFrame();
        SelectTab(0);
    }

    public void SelectTab(int index)
    {
        if (this.index!=index)
        {
            for (var i = 0; i < TabButtons.Length; i++)
            {
                TabButtons[i].Select(i == index);
                if (i<TabPages.Length-1)
                {
                    TabPages[i].SetActive(i==index);
                }
            }
            OnTabSelect?.Invoke(index);
        }
    }
}