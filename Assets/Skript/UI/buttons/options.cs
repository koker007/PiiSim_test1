using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class options : MonoBehaviour {

    [SerializeField]
    menu_ctrl menu_Ctrl;


    public void OptionsClick() {
        if (menu_Ctrl.setings != null) {
            if (menu_Ctrl.setings.active)
            {
                menu_Ctrl.closeAll();
            }
            else {
                menu_Ctrl.OpenSetings();
            }
        }
    }
}
