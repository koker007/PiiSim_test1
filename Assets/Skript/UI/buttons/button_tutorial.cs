using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class button_tutorial : MonoBehaviour {

    [SerializeField]
    menu_ctrl menu_Ctrl;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void TutorialButtonPress() {
        if (!menu_Ctrl.tutorial.active)
        {
            menu_Ctrl.OpenTutorial();
        }
        else menu_Ctrl.closeTutorial();
    }
}
