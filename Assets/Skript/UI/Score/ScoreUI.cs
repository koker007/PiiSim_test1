using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour {

    [SerializeField]
    setings seting;
    steam_achievement steam_Achievement;

    [SerializeField]
    Player player;
    [SerializeField]
    PiiController piiController;

    private int scoreOld = 0;

    [SerializeField]
    private Text[] texts;

    private AudioSource source;

    [SerializeField]
    private AudioClip[] clip;

    //Привязать настройки игры
    void get_seting_game()
    {
        //Если настроек нет
        if (seting == null)
        {
            //ищем обьект по тегу
            GameObject main_canvas = GameObject.FindWithTag("setings_game");
            //вытаскиваем настройки
            if (main_canvas != null) seting = main_canvas.GetComponent<setings>();
        }
    }

    void get_steam_achievement() {
        if (steam_Achievement == null) {
             //ищем обьект по тегу
             GameObject steam_manager = GameObject.FindWithTag("steam_manager");
             //вытаскиваем настройки
             if (steam_manager != null) steam_Achievement = steam_manager.GetComponent<steam_achievement>();
        }
    }

    // Use this for initialization
    void Start () {
        source = gameObject.GetComponent<AudioSource>();
        get_seting_game();
        get_steam_achievement();

    }
	
	// Update is called once per frame
	void Update () {

        //Проверяем поменялось ли число
        if (scoreOld != (int)player.ScoreALL) {
            scoreOld = (int)player.ScoreALL;
            //Пытаемся записать результат
            steam_Achievement.TestNewScoreTop(scoreOld);

            //Узнаем восьмое число
            int n8 = scoreOld / 10000000;
            float ostatoc = (scoreOld % 10000000);

            //Узнаем седьмое
            int n7 = (int)(ostatoc / 1000000);
            ostatoc = (ostatoc % 1000000);
            //Узнаем шестое
            int n6 = (int)(ostatoc / 100000);
            ostatoc = (ostatoc % 100000);
            //Узнаем пятое
            int n5 = (int)(ostatoc / 10000);
            ostatoc = (ostatoc % 10000);
            //Узнаем четвертое
            int n4 = (int)(ostatoc / 1000);
            ostatoc = (ostatoc % 1000);
            //Узнаем третье
            int n3 = (int)(ostatoc / 100);
            ostatoc = (ostatoc % 100);
            //Узнаем второе
            int n2 = (int)(ostatoc / 10);
            ostatoc = (ostatoc % 10);
            //Узнаем первое
            int n1 = (int)(ostatoc / 1);

            texts[0].text = System.Convert.ToString(n1);
            texts[1].text = System.Convert.ToString(n2);
            texts[2].text = System.Convert.ToString(n3);
            texts[3].text = System.Convert.ToString(n4);
            texts[4].text = System.Convert.ToString(n5);
            texts[5].text = System.Convert.ToString(n6);
            texts[6].text = System.Convert.ToString(n7);
            texts[7].text = System.Convert.ToString(n8);

            //Воспроизводим звук
            if (source != null && clip != null && clip.Length != 0) {
                if (seting != null && seting.game.new_data_setings_yn)
                    source.volume = seting.game.volume_all * seting.game.volume_sound;

                //source.pitch = Random.Range(0.9f, 1.1f);
                source.pitch = 0.9f + ((100 - player.percent_heath_last_target)/100) * 0.3f; 
                
                source.PlayOneShot(clip[Random.Range(0, clip.Length - 1)]);
            }
        }
	}
}
