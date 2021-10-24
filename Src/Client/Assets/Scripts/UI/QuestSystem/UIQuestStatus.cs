using Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


    class UIQuestStatus : MonoBehaviour
{

    public Image[] statusImages;

    private NpcQuestStatus questStatus;

    public void SetQuestStatus(NpcQuestStatus status)
    {
        this.questStatus = status;

        for (int i = 0; i < 4; i++)
        {
            if (this.statusImages[i] != null)
            {
                this.statusImages[i].gameObject.SetActive(i == (int)status);
            }
        }
    }
}
    
    
