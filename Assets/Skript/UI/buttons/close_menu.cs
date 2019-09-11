using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class close_menu : MonoBehaviour {

    [SerializeField]
    GameObject Menu;
    [SerializeField]
    GameplayParametrs gameplayParametrs;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        test_gameover();
	}

    public void close_menu_button() {
        Menu.active = false;
    }

    void test_gameover() {
        if (gameplayParametrs.GameOver)
            gameObject.SetActive(false);
    }
}
