using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayParametrs : MonoBehaviour {

    [SerializeField]
    public bool ThisDemoVersion = false;

    [SerializeField]
    private menu_ctrl menu_Ctrl;

    [SerializeField]
    private lvl_text text_center;

    [SerializeField]
    private setings seting;

    [SerializeField]
    private AudioSource source;

    [SerializeField]
    steam_achievement achievement;
    void iniAchivement() {
        if (achievement == null) {
            GameObject achievementObj = GameObject.FindGameObjectWithTag("steam_manager");

            if (achievementObj != null)
            {
                achievement = achievementObj.GetComponent<steam_achievement>();
            }

        }

        if (achievement != null) {
            achievement.gameplayParametrs = gameObject.GetComponent<GameplayParametrs>();
        }
    }

    bool GameOverSend = false;

    private float volume_music_now = 0;
    private float volume_music_need = 1;
    private float music_need = 0;
    private int lvl_now = 0;
    private float volume_music_base = 0.5f;

    [SerializeField]
    private AudioClip[] Music_menu;
    bool start_menu_song_yn = false;

    [SerializeField]
    private AudioClip[] Music_LVL1;

    [SerializeField]
    private AudioClip[] Music_LVL2;

    [SerializeField]
    private AudioClip[] Music_LVL3;

    [SerializeField]
    private AudioClip[] Music_LVL4;

    [SerializeField]
    private AudioClip[] Music_LVL5;

    //сылка на игрока
    [SerializeField]
    private Player player;
    [SerializeField]
    private AudioNoiseCtrl noiseCtrl;

    [SerializeField]
    ScoreTabWorldLoader scoreTab;

    public float TimeGamePlay = 0;
    public float TimeToEnd_max = 0;
    public float TimeToEnd = 0;
    public bool OverTime = false;
    public bool GameOver = false;
    float GameOverTime = 0;
    // Use this for initialization

    public int cheats_smeshenie = 0;
    public float cheats_time = 0;
    void IniCheats() {
        cheats_smeshenie = Random.Range(1, 100);
        player.ScoreCheats = cheats_smeshenie;
        cheats_time = cheats_smeshenie;
    }
    public bool CheatsFound = false;
    void TestCheats() {
        float timeFunc = cheats_time - cheats_smeshenie;
        float ScoreFunc = player.ScoreCheats - cheats_smeshenie;

        //узнаем насколько большая погрешность
        float pogreshnostTime = timeFunc - TimeGamePlay;
        float pogreshnostScore = ScoreFunc - player.ScoreALL;

        bool BadResult = false;
        float ResultFunc = ScoreFunc/timeFunc;
        float pogreshnostResult = ResultFunc - (player.ScoreALL / TimeGamePlay);

        if (pogreshnostTime > 1 || pogreshnostTime < -1 ||  
            pogreshnostScore > 1  || pogreshnostScore < -1 || 
            pogreshnostResult > 1 || pogreshnostResult < -1)
        {
            CheatsFound = true;
        }
    }

	void Start () {
        get_seting_game();
        IniCheats();
        //iniAchivement();
    }
	
	// Update is called once per frame
	void Update () {
        iniAchivement();
        TestTime();
        TestVolume();
        test_gameover();
        TestPause();
        TestTimeGamePlay();
        CalcCoof();
        TestCheats();
    }

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

    void TestTime() {
        //Если игрок сыт
        if (player.PiiNow) {
            if (TimeToEnd > 0 && !OverTime && !GameOver)
            {
                TimeToEnd -= Time.deltaTime;
                if ((TimeToEnd < 0 && lvl_now != 5) ||
                    (lvl_now == 1 && (TimeToEnd_max - TimeToEnd) > 30 && player.ScoreTime < 50) ||
                    (lvl_now == 2 && player.ScoreTime < 300) ||
                    (lvl_now == 3 && player.ScoreTime < 3000) ||
                    (lvl_now == 4 && player.ScoreTime < 5000) ||
                    (lvl_now == 5 && player.ScoreTime < 10000)
                    )
                {
                    text_center.text_next = "Over Time";
                    OverTime = true;
                    TimeToEnd = 0;
                }
                //Если цунами настигло игрока
                else if (TimeToEnd < 0 && lvl_now == 5) {
                    OverTime = true;
                    player.ScoreTime = 0;
                    TimeToEnd = 0;
                }
            }
            /*
            if (TimeToEnd == 0 && Time.timeScale > 0 && player.ScoreALL > 500) {
                Time.timeScale = Time.timeScale - 0.03f;
                if (Time.timeScale < 0.1f) {
                    Time.timeScale = 0.1f;
                    player.PiiNow = false;
                }
            }
            */
            //Уменьшение напора
            if (OverTime) {
                //player.ScoreTime -= (player.ScoreTime/5) * Time.deltaTime;
                //player.ScoreTime -= (player.ScoreTime/0.f - player.ScoreTime/2) * Time.deltaTime * 0.5f;
                PiiController piiController = player.bolt.GetComponent<PiiController>();
                if (!piiController.ChargeOn) {

                    //Скорость уменьшения в овер тайме
                    float speedOverTime = 15;
                    if (lvl_now == 2) speedOverTime = 10;
                    else if (lvl_now == 3) speedOverTime = 15;
                    else if (lvl_now == 4) speedOverTime = 20;
                    else if (lvl_now == 5 || lvl_now == 6) speedOverTime = 40;

                    player.ScoreTime -= Calculations.coofing_num(player.ScoreTime * piiController.speed_pii * 0.05f, 1.5f) * Time.deltaTime * speedOverTime;
                }
                
                //Условие gameover
                if (player.ScoreTime < 1)
                {
                    if (get_lvl_now() < 6)
                    {
                        text_center.text_next = "Game Over";
                    }
                    else {
                        text_center.text_next = "Thanks for Playing!";
                    }
                    GameOver = true;
                    player.PiiNow = false;
                }
            }
        }
    }
    void TestVolume() {
        if (volume_music_now != volume_music_need) {
            if (volume_music_now < volume_music_need) {
                volume_music_now += (float)(0.5 * Time.deltaTime);
                if (volume_music_now > volume_music_need) {
                    volume_music_now = volume_music_need;
                }
                source.volume = volume_music_base * volume_music_now * seting.game.volume_all * seting.game.volume_music;
            }
            if (volume_music_now > volume_music_need) {
                volume_music_now -= (float)(0.5 * Time.deltaTime);
                if (volume_music_now < volume_music_need) {
                    volume_music_now = volume_music_need;
                    //Переключаем запускаем необходимую музыку
                    volume_music_need = 1;

                    if (music_need == 1 && Music_LVL1.Length != 0) {
                        //TimeToEnd = 5 * 60;
                        //TimeToEnd_max = TimeToEnd;
                        source.Stop();
                        source.PlayOneShot(Music_LVL1[Random.Range(0, Music_LVL1.Length)]);
                    }
                    else if (music_need == 2 && Music_LVL2.Length != 0)
                    {
                        //TimeToEnd = (3 * 60) + 30;
                        //TimeToEnd_max = TimeToEnd;
                        source.Stop();
                        source.PlayOneShot(Music_LVL2[Random.Range(0, Music_LVL2.Length)]);
                    }
                    else if (music_need == 3 && Music_LVL3.Length != 0)
                    {
                        //TimeToEnd = 3 * 60;
                        //TimeToEnd_max = TimeToEnd;
                        source.Stop();
                        source.PlayOneShot(Music_LVL3[Random.Range(0, Music_LVL3.Length)]);
                    }
                    else if (music_need == 4 && Music_LVL4.Length != 0)
                    {
                        //TimeToEnd = (2 * 60) + 30;
                        //TimeToEnd_max = TimeToEnd;
                        source.Stop();
                        source.PlayOneShot(Music_LVL4[Random.Range(0, Music_LVL4.Length)]);
                    }
                    else if (music_need == 5 && Music_LVL5.Length != 0)
                    {
                        //TimeToEnd = (2 * 60) + 30;
                        //TimeToEnd_max = TimeToEnd;
                        source.Stop();
                        source.PlayOneShot(Music_LVL5[Random.Range(0, Music_LVL5.Length)]);
                    }
                    //Музыка меню
                    else if (music_need == 1000 && Music_menu.Length != 0) {
                        source.Stop();
                        source.PlayOneShot(Music_menu[Random.Range(0, Music_menu.Length)]);
                    }

                }
            }
        }

        if (seting != null && source != null) {
            source.volume = volume_music_base * volume_music_now * seting.game.volume_all * seting.game.volume_music;
        }
    }

    public void start_menu_song() {
        if (!start_menu_song_yn)
        {
            start_menu_song_yn = true;
            volume_music_need = 0;
            music_need = 1000;
        }
    }

    public void start_lvl_1() {
        if (lvl_now < 1) {
            text_center.text_next = "LVL I";
            lvl_now = 1;
            volume_music_need = 0;
            music_need = 1;

            TimeToEnd = 5 * 60;
            TimeToEnd_max = TimeToEnd;

            //player.ScoreALL = player.ScoreALL * 1.5f;
            //player.ScorePlus(player.ScoreALL * 0.5f);
            noiseCtrl.SetTypeNoise(0);

            OverTime = false;
        }
    }
    public void start_lvl_2() {
        if (lvl_now < 2)
        {
            text_center.text_next = "LVL II";
            lvl_now = 2;
            volume_music_need = 0;
            music_need = 2;

            TimeToEnd = (3 * 60) + 30;
            TimeToEnd_max = TimeToEnd;

            //player.ScoreALL = player.ScoreALL * 1.5f;
            player.ScorePlusCheat(player.ScoreALL * 0.5f);
            noiseCtrl.SetTypeNoise(1);
            player.SetFullCharge();

            OverTime = false;

            achievement.lvl_1_complite();
            if (TimeGamePlay <= 60) {
                achievement.sec60LVL1();
            }
        }
    }
    public void start_lvl_3()
    {
        if (lvl_now < 3 && !ThisDemoVersion) {
            text_center.text_next = "LVL III";
            lvl_now = 3;
            volume_music_need = 0;
            music_need = 3;

            TimeToEnd = 3 * 60;
            TimeToEnd_max = TimeToEnd;

            //player.ScoreALL = player.ScoreALL * 1.5f;
            player.ScorePlusCheat(player.ScoreALL * 0.5f);
            noiseCtrl.SetTypeNoise(1);
            player.SetFullCharge();

            OverTime = false;

            achievement.lvl_2_complite();
        }
        else if (lvl_now < 3 && ThisDemoVersion) {
            text_center.text_next = "This Demo Version";

            lvl_now = 3;
            volume_music_need = 0;
            music_need = 3;

            //TimeToEnd = 3 * 60;
            //TimeToEnd_max = TimeToEnd;

            //player.ScoreALL = player.ScoreALL * 1.5f;
            player.ScorePlusCheat(player.ScoreALL * 0.5f);
            noiseCtrl.SetTypeNoise(1);
            player.SetFullCharge();

            OverTime = false;

            achievement.lvl_2_complite();
        }
    }
    public void start_lvl_4()
    {
        if (lvl_now < 4 && !ThisDemoVersion)
        {
            text_center.text_next = "LVL IV";
            lvl_now = 4;
            volume_music_need = 0;
            music_need = 4;

            TimeToEnd = (2 * 60) + 30;
            TimeToEnd_max = TimeToEnd;

            //player.ScoreALL = player.ScoreALL * 1.5f;
            player.ScorePlusCheat(player.ScoreALL * 0.5f);
            noiseCtrl.SetTypeNoise(2);
            player.SetFullCharge();

            OverTime = false;

            achievement.lvl_3_complite();
        }
    }
    public void start_lvl_5()
    {
        if (lvl_now < 5 && !ThisDemoVersion)
        {
            text_center.text_next = "LVL V";
            lvl_now = 5;
            volume_music_need = 0;
            music_need = 5;

            TimeToEnd = (2 * 60) + 30;
            TimeToEnd_max = TimeToEnd;

            //player.ScoreALL = player.ScoreALL * 1.5f;
            player.ScorePlusCheat(player.ScoreALL * 0.5f);
            player.SetFullCharge();

            OverTime = false;

            achievement.lvl_4_complite();
        }
    }
    public void start_lvl_6() {
        if (lvl_now < 6 && !ThisDemoVersion)
        {
            //text_center.text_next = "LVL V";
            lvl_now = 6;
            //volume_music_need = 0;
            music_need = 6;

            //player.ScoreALL = player.ScoreALL * 1.1f;
            player.ScorePlusCheat(player.ScoreALL * 0.1f);
            //player.SetFullCharge();

            //OverTime = false;
            //GameOver = true;

            achievement.lvl_5_complite();
        }
    }

    public int get_lvl_now() {
        return lvl_now;
    }

    void test_gameover() {
        //Открываем меню только после того как надпись гейм овер высветится полностью
        if (GameOver) {
            if (!GameOverSend && !CheatsFound)
            {
                GameOverSend = true;
                //Запихиваем результат в таблицу
                asuncFuncSteam asuncSteam = GameObject.FindGameObjectWithTag("steam_manager").GetComponent<asuncFuncSteam>();
                bool NeedReWrite = false;
                if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.RightAlt)) {
                    NeedReWrite = true;
                }

                asuncSteam.UploadScoreInLeaderboard((int)CoofST, NeedReWrite);
                if (scoreTab != null)
                    scoreTab.TimeUpdate = 999;
            }

            start_menu_song();
            if (player.ScoreALL <= 0 && GameOverTime <= 0)
            {
                achievement.zero_score();
            }
            GameOverTime += Time.deltaTime;

            menu_Ctrl = gameObject.GetComponent<menu_ctrl>();
            if (GameOverTime > 4 && !menu_Ctrl.menu.active) {
                menu_Ctrl.OpenStaticticsTab();
            }
        }
    }

    public GameObject PauseWindow;
    public bool pause = false;
    bool pausePressButtonOld = false;
    void TestPause() {
        if (menu_Ctrl == null) {
            menu_Ctrl = gameObject.GetComponent<menu_ctrl>();
        }

        if ((!pausePressButtonOld && UnityEngine.Input.GetKey(KeyCode.Pause)) || (pause && menu_Ctrl != null && menu_Ctrl.menu.active))
        {
            pausePressButtonOld = true;

            if (!pause && !GameOver)
            {
                pause = true;
                Time.timeScale = 0;
                if (source != null) {
                    source.Pause();
                }
            }
            else {
                pause = false;
                Time.timeScale = 1;
                if (source != null) {
                    source.UnPause();
                }
            }
        }
        else if(!UnityEngine.Input.GetKey(KeyCode.Pause)) {
            pausePressButtonOld = false;
        }

        if (PauseWindow != null) {
            if (!PauseWindow.active && pause) {
                PauseWindow.SetActive(true);
            }
            else if (PauseWindow.active && !pause) {
                PauseWindow.SetActive(false);
            }
        }
    }

    void TestTimeGamePlay() {
        if (!GameOver && !OverTime && !pause && TimeToEnd != 0) {
            TimeGamePlay += Time.deltaTime;
            cheats_time += Time.deltaTime;
        }
    }
    public float CoofST = 0;
    void CalcCoof() {
        CoofST = player.ScoreALL / TimeGamePlay;
    }
}
