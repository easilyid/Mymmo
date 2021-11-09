using System.Collections;
using System.Collections.Generic;
using Models;
using UnityEngine;

public class UISkillSlots : MonoBehaviour
{
    public UISkillSlot[] slots;

    private void Start()
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
        var Skills = DataManager.Instance.Skills[(int)User.Instance.CurrentCharacterInfo.Class];
        int skillIdx = 0;
        foreach (var kv in Skills)
        {
            slots[skillIdx].SetSkill(kv.Value);
            skillIdx++;
        }
    }
}
