using Models;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.UI;
using Entities;
using System;

public class UIMain : MonoSingleton<UIMain>
{
    public Text avatarName;
    public Text avatarLevel;

    public UITeam TeamWindow;

    public UICreatureInfo targetUI;

    public UISkillSlots SkillSlots;
    protected override void OnStart()
    {
        this.UpdateAvatar();
        this.targetUI.gameObject.SetActive(false);
        BattleManager.Instance.OnTargetChanged += OnTargetChanged;
        User.Instance.OnCharacterInit += this.SkillSlots.UpdateSkills;
        this.SkillSlots.UpdateSkills();
    }

    void UpdateAvatar()
    {
        this.avatarName.text = string.Format("{0}[{1}]", User.Instance.CurrentCharacterInfo.Name, User.Instance.CurrentCharacterInfo.Id);
        this.avatarLevel.text = User.Instance.CurrentCharacterInfo.Level.ToString();
    }
	
	// Update is called once per frame
	void Update () {
		
	}


    public void OnUITest()
    {
        UIManager.Instance.Show<UITest>();
    }

    public void OnClickBag()
    {
        UIManager.Instance.Show<UIBag>();
    }

    public void OnClickEquip()
    {
        UIManager.Instance.Show<UICharEquip>();
    }
    public void OnClickQuest()
    {
        UIManager.Instance.Show<UIQuestSystem>();
    }

    public void OnClickFriend()
    {
        UIManager.Instance.Show<UIFriends>();
    }

    public void ShowTeamUI(bool show)
    {
        TeamWindow.ShowTeam(show);
    }

    public void OnClickGuild()
    {
        GuildManager.Instance.ShowGuild();
    }

    public void OnClickRide()
    {
        UIManager.Instance.Show<UIRide>();
    }

    public void OnclickSetting()
    {
        UIManager.Instance.Show<UISetting>();
    }
    public void OnClickSkill()
    {
        UIManager.Instance.Show<UISkill>();
    }

    private void OnTargetChanged(Creature target)
    {
        if (target!=null)
        {
            if (!targetUI.isActiveAndEnabled)targetUI.gameObject.SetActive(true);
            targetUI.Target = target;
        }
        else
        {
            targetUI.gameObject.SetActive(false);
        }
    }


}
