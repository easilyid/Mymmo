using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using SkillBridge.Message;
using UnityEngine;
using UnityEngine.UI;

public class UIArena : MonoSingleton<UIArena>
{
    public Text RoundText;
    public Text CountDownText;

    protected override void OnStart()
    {
        RoundText.enabled = false;
        CountDownText.enabled = false;
        ArenaManager.Instance.SendReady();
    }

    public void ShowCountDown()
    {
        StartCoroutine(CountDown(10));
    }

    IEnumerator CountDown(int seconds)
    {
        int total = seconds;
        RoundText.text = "ROUND " + ArenaManager.Instance.Round;
        RoundText.enabled = true;
        CountDownText.enabled = true;
        while (total > 0)
        {
            SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_CountDown);
            CountDownText.text = total.ToString();
            yield return new WaitForSeconds(1f);
            total--;
        }
        CountDownText.text = "READY";
    }

    internal void ShowRoundStart(int round, ArenaInfo arenaInfo)
    {
        CountDownText.text = "FIGHT";
    }

    public void ShowRoundResult(int round, ArenaInfo arenaInfo)
    {
        CountDownText.enabled = true;
        CountDownText.text = "YOU WIN";
    }
}
