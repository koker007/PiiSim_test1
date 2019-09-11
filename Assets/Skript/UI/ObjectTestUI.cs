using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectTestUI : MonoBehaviour {

    steam_achievement achievement;

	// Use this for initialization
	void Start () {
        get_steam_achivment();
	}
	
	// Update is called once per frame
	void Update () {
        gameObject.GetComponent<Text>().text = System.Convert.ToString(achievement.Destroy_objects);
    }

    void get_steam_achivment()
    {
        if (achievement == null)
        {
            //ищем обьект по тегу
            GameObject steam_manager = GameObject.FindWithTag("steam_manager");
            //вытаскиваем настройки
            if (steam_manager != null) achievement = steam_manager.GetComponent<steam_achievement>();
        }
    }
}
