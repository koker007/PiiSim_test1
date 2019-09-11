using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class steam_achievement : MonoBehaviour {

    [SerializeField]
    public GameplayParametrs gameplayParametrs;
    //bool notDemo = false;
    public void iniGameParam() {
        //инициализация переложена на компонент гейм параметр
        /*
        if (gameplayParametrs == null) {
            GameObject gameParamObj = GameObject.FindGameObjectWithTag("setings_game");

            if (gameParamObj != null) {
                gameplayParametrs = gameParamObj.GetComponent<GameplayParametrs>();
            }
        }
        if (!notDemo && gameplayParametrs != null && !gameplayParametrs.ThisDemoVersion) {
            notDemo = true;
        }
        */
    }

    [SerializeField]
    bool need_clear_all_yn = true;

    bool inicialized_yn = false;

    //Таймиер для отсправки данных на сервер
    float timer_stats_to_out = 0;
    //статистические переменные

    string name_ScoreTop = "scoreTop";
    public int ScoreTop = 0;
    int ScoreTopOldSend = 0;
    string name_Destroy_objects = "Destroy_objects";
    public int Destroy_objects = 0;
    string name_Destroy_police = "Destroy_police";
    public int Destroy_police = 0;
    string name_Destroy_house = "Destroy_house";
    public int Destroy_house = 0;
    string name_Destroy_tankShell = "tank_shells";
    public int Destroy_tank_shell = 0;

    //Имена аачивок
    string name_pii_beginer = "Pii_beginer";
    string name_pii_amazing = "Pii_amazing";
    string name_Pii_intelligent = "Pii_intelligent";
    string name_Pii_intense = "Pii_intense";
    string name_Pii_aggressive = "Pii_aggressive";

    string name_Complete_lvl_1 = "Complete_lvl_1";
    string name_Complete_lvl_2 = "Complete_lvl_2";
    string name_Complete_lvl_3 = "Complete_lvl_3";
    string name_Complete_lvl_4 = "Complete_lvl_4";
    string name_Complete_lvl_5 = "Complete_lvl_5";

    string name_in_the_dark = "In_the_dark";
    string name_compromising_material = "compromising_material";
    string name_the_first = "the_first";
    string name_destroy_fighter = "destroy_fighter";
    string name_fly_car = "fly_car";
    string name_scoolbus = "destroy_schollbus";
    string name_zero_score = "zero_score";
    string name_60secLVL1 = "sec60LVL1";
    string name_doggyStyle = "doggyStyle";

    bool to_server_ok = true;

    // Use this for initialization
    void Start () {
        //iniGameParam();
        inicialize();
        GetALL();

        if (need_clear_all_yn)
            ClearAll();
    }
	
	// Update is called once per frame
	void Update () {
        SetAllStats();
        test_time();
    }

    void inicialize() {
        //Получить
        bool geting_current_acivment = false;
        geting_current_acivment = SteamUserStats.RequestCurrentStats();
        bool geting_stats = false;
        SteamAPICall_t user = SteamUserStats.RequestUserStats(SteamUser.GetSteamID());
        if (geting_current_acivment && user != null) {
            inicialized_yn = true;
        }
    }

    //float timeLastSend = 60;
    void SetAllStats() {
        //timeLastSend += Time.deltaTime;

        if (!to_server_ok 
            //&& timeLastSend > 60
            ) {
            to_server_ok = SteamUserStats.StoreStats();
            timer_stats_to_out = 0;
            if (!to_server_ok)
            {
                Debug.Log("SteamUserStats.StoreStats() return false");
            }
            else {
                //timeLastSend = 0;
            }
        }

        timer_stats_to_out += Time.deltaTime;
        if (inicialized_yn && timer_stats_to_out > 300) {
            timer_stats_to_out = 0;
            to_server_ok = false;
            ScoreTopOldSend = ScoreTop;
        }
    }



    //Очистить все достижения
    public void ClearAll() {
        if (inicialized_yn && SteamFriends.GetPersonaName() == "Koker 007") {
            //SteamUserStats.SetStat(name_Destroy_objects, 0);
            //SteamUserStats.SetStat(name_Destroy_police, 0);
            //SteamUserStats.SetStat(name_Destroy_house, 0);
            //SteamUserStats.ClearAchievement(name_pii_beginer);
            //SteamUserStats.ClearAchievement(name_pii_amazing);
            //SteamUserStats.ClearAchievement(name_Pii_intelligent);
            //SteamUserStats.ClearAchievement(name_Pii_intense);
            //SteamUserStats.ClearAchievement(name_Pii_aggressive);

            //SteamUserStats.StoreStats();
            SteamUserStats.ResetAllStats(true);
        }
    }

    //Взять данные из стима
    void GetALL() {
        if (inicialized_yn) {
            bool ok = false;
            ok = SteamUserStats.GetStat(name_Destroy_objects, out Destroy_objects);
            ok = SteamUserStats.GetStat(name_Destroy_police, out Destroy_police);
            ok = SteamUserStats.GetStat(name_Destroy_house, out Destroy_house);
            ok = SteamUserStats.GetStat(name_ScoreTop, out ScoreTop);
            ok = SteamUserStats.GetStat(name_Destroy_tankShell, out Destroy_tank_shell);

        }
    }



    public void plus_1_destroy_object() {
        if (gameplayParametrs != null && !gameplayParametrs.ThisDemoVersion)
        {
            if (time != 0)
            {
                time = 0;
                obj = 0;
            }
            else
            {
                obj++;
            }

            //если обьектов за короткое время стало слишком много. значит что-то тут не чисто
            if (obj > 100)
            {
                obj = obj;
            }

            Destroy_objects++;
            SteamUserStats.SetStat(name_Destroy_objects, Destroy_objects);
            if (Destroy_objects == 100)
            {
                //to_server_ok = false;
            }
            else if (Destroy_objects == 500)
            {
                //to_server_ok = false;
            }
            else if (Destroy_objects == 2000)
            {
                to_server_ok = false;
            }
            else if (Destroy_objects == 5000)
            {
                to_server_ok = false;
            }
            else if (Destroy_objects == 10000)
            {
                to_server_ok = false;
            }
        }
    }
    public void plus_1_destroy_police() {
        if (gameplayParametrs != null && !gameplayParametrs.ThisDemoVersion)
        {

            Destroy_police++;
            SteamUserStats.SetStat(name_Destroy_police, Destroy_police);
            if (Destroy_police == 100)
            {
                to_server_ok = false;
            }
            else if (Destroy_police == 10)
            {
                to_server_ok = false;
            }
            else if (Destroy_police == 100)
            {
                to_server_ok = false;
            }
            else if (Destroy_police == 1000)
            {
                to_server_ok = false;
            }
        }
    }
    public void plus_1_tank_shell() {
        if (gameplayParametrs != null && !gameplayParametrs.ThisDemoVersion)
        {
            Destroy_tank_shell++;
            SteamUserStats.SetStat(name_Destroy_tankShell, Destroy_tank_shell);
            

            if (Destroy_tank_shell == 10)
            {
                to_server_ok = false;
            }
        }
    }
    public void TestNewScoreTop(int NewScoreTop) {
        if (ScoreTop < NewScoreTop) {
            ScoreTop = NewScoreTop;
            SteamUserStats.SetStat(name_ScoreTop, ScoreTop);

            //Если счет резко отличается то нужно отправить немедленно или если игра закончилать
            if (ScoreTopOldSend * 10 < ScoreTop || (gameplayParametrs != null && gameplayParametrs.GameOver))
            {
                //to_server_ok = false;
            }
        }
    }

    public void lvl_1_complite()
    {
        if (gameplayParametrs != null && !gameplayParametrs.ThisDemoVersion)
        {
            bool resultYN = false;
            bool unloskYN = false;
            resultYN = SteamUserStats.GetAchievement(name_Complete_lvl_1, out unloskYN);
            if (resultYN && !unloskYN)
            {
                SteamUserStats.SetAchievement(name_Complete_lvl_1);
                to_server_ok = false;
            }
        }
    }
    public void lvl_2_complite()
    {
        if (gameplayParametrs != null && !gameplayParametrs.ThisDemoVersion)
        {
            bool resultYN = false;
            bool unloskYN = false;
            resultYN = SteamUserStats.GetAchievement(name_Complete_lvl_2, out unloskYN);
            if (resultYN && !unloskYN)
            {
                SteamUserStats.SetAchievement(name_Complete_lvl_2);
                to_server_ok = false;
            }
        }
    }
    public void lvl_3_complite()
    {
        if (gameplayParametrs != null && !gameplayParametrs.ThisDemoVersion)
        {
            bool resultYN = false;
            bool unloskYN = false;
            resultYN = SteamUserStats.GetAchievement(name_Complete_lvl_3, out unloskYN);
            if (resultYN && !unloskYN)
            {
                SteamUserStats.SetAchievement(name_Complete_lvl_3);
                to_server_ok = false;
            }
        }
    }
    public void lvl_4_complite() {
        if (gameplayParametrs != null && !gameplayParametrs.ThisDemoVersion)
        {
            bool resultYN = false;
            bool unloskYN = false;
            resultYN = SteamUserStats.GetAchievement(name_Complete_lvl_4, out unloskYN);
            if (resultYN && !unloskYN)
            {
                SteamUserStats.SetAchievement(name_Complete_lvl_4);
                to_server_ok = false;
            }
        }
    }
    public void lvl_5_complite()
    {
        if (gameplayParametrs != null && !gameplayParametrs.ThisDemoVersion)
        {
            bool resultYN = false;
            bool unloskYN = false;
            resultYN = SteamUserStats.GetAchievement(name_Complete_lvl_5, out unloskYN);
            if (resultYN && !unloskYN)
            {
                SteamUserStats.SetAchievement(name_Complete_lvl_5);
                to_server_ok = false;
            }
        }
    }

    public void in_the_dark() {
        if (gameplayParametrs != null && !gameplayParametrs.ThisDemoVersion)
        {
            if (gameplayParametrs != null && gameplayParametrs.get_lvl_now() <= 2)
            {
                bool resultYN = false;
                bool unloskYN = false;
                resultYN = SteamUserStats.GetAchievement(name_in_the_dark, out unloskYN);
                if (resultYN && !unloskYN)
                {
                    SteamUserStats.SetAchievement(name_in_the_dark);
                    to_server_ok = false;
                }
            }
        }
    }
    public void compromising_material()
    {
        if (gameplayParametrs != null && !gameplayParametrs.ThisDemoVersion)
        {
            if (gameplayParametrs != null && gameplayParametrs.get_lvl_now() <= 2)
            {
                bool resultYN = false;
                bool unloskYN = false;
                resultYN = SteamUserStats.GetAchievement(name_compromising_material, out unloskYN);
                if (resultYN && !unloskYN)
                {
                    SteamUserStats.SetAchievement(name_compromising_material);
                    to_server_ok = false;
                }
            }
        }
    }
    public void the_first() {
        if (gameplayParametrs != null && !gameplayParametrs.ThisDemoVersion)
        {
            bool resultYN = false;
            bool unloskYN = false;
            resultYN = SteamUserStats.GetAchievement(name_the_first, out unloskYN);
            if (resultYN && !unloskYN)
            {
                SteamUserStats.SetAchievement(name_the_first);
                to_server_ok = false;
            }
        }
    }
    public void destroy_fighter() {
        if (gameplayParametrs != null && !gameplayParametrs.ThisDemoVersion)
        {
            bool resultYN = false;
            bool unloskYN = false;
            resultYN = SteamUserStats.GetAchievement(name_destroy_fighter, out unloskYN);
            if (resultYN && !unloskYN)
            {
                SteamUserStats.SetAchievement(name_destroy_fighter);
                to_server_ok = false;
            }
        }
    }
    public void fly_car() {
        if (gameplayParametrs != null && !gameplayParametrs.ThisDemoVersion)
        {
            bool resultYN = false;
            bool unloskYN = false;
            resultYN = SteamUserStats.GetAchievement(name_fly_car, out unloskYN);
            if (resultYN && !unloskYN)
            {
                SteamUserStats.SetAchievement(name_fly_car);
                to_server_ok = false;
            }
        }
    }
    public void scoolbus() {
        if (gameplayParametrs != null && !gameplayParametrs.ThisDemoVersion)
        {
            bool resultYN = false;
            bool unloskYN = false;
            resultYN = SteamUserStats.GetAchievement(name_scoolbus, out unloskYN);
            if (resultYN && !unloskYN)
            {
                SteamUserStats.SetAchievement(name_scoolbus);
                to_server_ok = false;
            }
        }
    }
    public void zero_score() {
        if (gameplayParametrs != null && !gameplayParametrs.ThisDemoVersion)
        {
            bool resultYN = false;
            bool unloskYN = false;
            resultYN = SteamUserStats.GetAchievement(name_zero_score, out unloskYN);
            if (resultYN && !unloskYN)
            {
                SteamUserStats.SetAchievement(name_zero_score);
                to_server_ok = false;
            }
        }
    }
    public void sec60LVL1()
    {
        if (gameplayParametrs != null && !gameplayParametrs.ThisDemoVersion)
        {
            bool resultYN = false;
            bool unloskYN = false;
            resultYN = SteamUserStats.GetAchievement(name_60secLVL1, out unloskYN);
            if (resultYN && !unloskYN)
            {
                SteamUserStats.SetAchievement(name_60secLVL1);
                Debug.Log("LVL1 IN 60 SEC");
                to_server_ok = false;
            }
        }
    }
    public void doggyStyle() {
        if (gameplayParametrs != null && !gameplayParametrs.ThisDemoVersion)
        {
            bool resultYN = false;
            bool unloskYN = false;
            resultYN = SteamUserStats.GetAchievement(name_doggyStyle, out unloskYN);
            if (resultYN && !unloskYN)
            {
                SteamUserStats.SetAchievement(name_doggyStyle);
                Debug.Log("Doggy Style");
                to_server_ok = false;
            }
        }
    }

    float time = 0;
    int obj = 0;
    void test_time() {
        time = time + Time.deltaTime;
    }
}
