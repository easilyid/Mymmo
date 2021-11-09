using Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINameBar : MonoBehaviour
{
    public Text avaverName;
    // Start is called before the first frame update
    public Creature Creature;
    void Start () {
		if(this.Creature!=null)
        {
            
        }
	}
	
	// Update is called once per frame
	void Update () {
        this.UpdateInfo();

	}

    void UpdateInfo()
    {
        if (this.Creature != null)
        {
            string name = this.Creature.Name + " Lv." + this.Creature.Info.Level;
            if(name != this.avaverName.text)
            {
                this.avaverName.text = name;
            }
        }
    }
}
