using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scoreTabButton : MonoBehaviour {

    [SerializeField]
    menu_ctrl menu_Ctrl;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ScoreTabButtonPress() {
        if (!menu_Ctrl.scoreTab.active)
        {
            menu_Ctrl.OpenStaticticsTab();
        }
        else {
            menu_Ctrl.closeScoreTab();
        }
    }
}
