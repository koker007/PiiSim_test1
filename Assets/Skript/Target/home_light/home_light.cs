using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class home_light : MonoBehaviour {

    [SerializeField]
    PiiTarget rozetka;
    steam_achievement steam_Achievement;

	// Use this for initialization
	void Start () {
        set_steam_achievement();
    }
	
	// Update is called once per frame
	void Update () {
        test_heat_rozetka();	
	}

    void set_steam_achievement() {
        if (steam_Achievement == null) {
            //ищем обьект по тегу
            GameObject steam_manager = GameObject.FindWithTag("steam_manager");
            if (steam_manager != null) steam_Achievement = steam_manager.GetComponent<steam_achievement>();
        }
    }

    void test_heat_rozetka() {
        if (rozetka != null && rozetka.heath <= 0) {
            if (steam_Achievement != null)
                steam_Achievement.in_the_dark();
            gameObject.active = false;
        }
    }
}
