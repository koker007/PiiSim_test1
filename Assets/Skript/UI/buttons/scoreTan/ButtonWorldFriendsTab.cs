using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonWorldFriendsTab : MonoBehaviour {

    [SerializeField]
    GameObject WordlTab;
    [SerializeField]
    GameObject FriendsTab;

    public void ButtonClick() {
        if (FriendsTab != null && WordlTab != null) {
            if (FriendsTab.active && !WordlTab.active)
            {
                WordlTab.SetActive(true);
                FriendsTab.SetActive(false);
            }
            else if(!FriendsTab.active && WordlTab.active) {
                FriendsTab.SetActive(true);
                WordlTab.SetActive(false);
            }
        }
    }
}
