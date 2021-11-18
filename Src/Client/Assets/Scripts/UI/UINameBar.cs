using Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINameBar : MonoBehaviour
{
    public Text avaverName;
    // Start is called before the first frame update
    public Character Character;
    void Start () {
		if(this.Character != null)
        {
            
        }
	}
	
	// Update is called once per frame
	void Update () {
        this.UpdateInfo();

	}

    void UpdateInfo()
    {
        if (this.Character != null)
        {
            string name = this.Character.Name + " Lv." + this.Character.Info.Level;
            if(name != this.avaverName.text)
            {
                this.avaverName.text = name;
            }
        }
    }
}
