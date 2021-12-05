using Common.Data;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStory : UIWindow
{
    public Text title;
    public Text descript;

    StoryDefine story;

    public void SetStory(StoryDefine story)
    {
        this.story = story;
        this.title.text = story.Name;
        this.descript.text = story.Description;
    }

    public void OnCkickStart()
    {
        if (!StoryManager.Instance.StartStory(this.story.ID))
        {
        }
    }
}
