using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using System;

//Этот скрипт приклепляется к префабу и нужен для показа данных пользователя SteamID в таблице результатов
public class ScoreTabSteamID : MonoBehaviour {

    private CSteamID steamID;
    private float time_re_update = 0;
    public int scoreInt = 0;

    float player_intensy_color = 1;
    bool player_intensy_color_plus = true;

    [SerializeField]
    Image Fon;
    Color Fon_color_normal;

    [SerializeField]
    Color First_color;
    [SerializeField]
    Color Second_color;
    [SerializeField]
    Color Third_color;
    [SerializeField]
    Color Normal_color;

    [SerializeField]
    Text FriendPosition;

    [SerializeField]
    RawImage PlayerAvatar;
    Texture2D TextureAvatar;
    [SerializeField]
    Text Nickname;
    [SerializeField]
    Text Score;
    public bool score_geting_ok = true;

    [SerializeField]
    steam_achievement steam_Achievement;
    


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        iniSteamAchievement();
        testColor();
        //test_update_data();
    }

    public void inicializationSteamID(CSteamID iD) {
        steamID = iD;

        setNickname();
        setAvatar();
        //setScore();
    }

    void iniSteamAchievement() {
        if (steam_Achievement == null) {
            steam_Achievement = GameObject.FindGameObjectWithTag("steam_manager").GetComponent<steam_achievement>();
        }
    }

    void testColor() {

        //Смена базового цвета в зависимости от места
        if (Convert.ToInt32(FriendPosition.text) == 1) {
            Fon_color_normal = First_color;
            Fon.color = First_color;
        }
        else if (Convert.ToInt32(FriendPosition.text) == 2) {
            Fon_color_normal = Second_color;
            Fon.color = Second_color;
        }
        else if (Convert.ToInt32(FriendPosition.text) == 3) {
            Fon_color_normal = Third_color;
            Fon.color = Third_color;
        }
        else
        {
            Fon_color_normal = Normal_color;
            Fon.color = Normal_color;
        }

        //Если это мой ник
        if (Nickname.text == SteamFriends.GetPersonaName() && Fon != null) {

            //Если надо увеличить интенсивность
            if (player_intensy_color_plus)
            {
                player_intensy_color += Time.deltaTime * 6f;
            }
            else {
                player_intensy_color -= Time.deltaTime * 6f;
            }

            //Проверка порога
            if (player_intensy_color > 2.5f) {
                player_intensy_color = 2.5f;
                player_intensy_color_plus = false;
            }
            if (player_intensy_color < 0.25f) {
                player_intensy_color = 0.25f;
                player_intensy_color_plus = true;
            }

            //если цвета по умолчанию нету
            if (Fon_color_normal.r == 0 && Fon_color_normal.g == 0 && Fon_color_normal.b == 0 && Fon_color_normal.a == 0) {
                Fon_color_normal = Fon.color;
            }
            Color Fon_color_new = Fon_color_normal;

            //Изменяем цвет
            Fon_color_new.r = Fon_color_new.r * player_intensy_color;
            Fon_color_new.g = Fon_color_new.g * player_intensy_color;
            Fon_color_new.b = Fon_color_new.b * player_intensy_color;
            //Fon_color_new.a = Fon_color_new.a * player_intensy_color;

            if (Fon_color_new.r > 1)
                Fon_color_new.r = 1;
            if (Fon_color_new.g > 1)
                Fon_color_new.g = 1;
            if (Fon_color_new.b > 1)
                Fon_color_new.b = 1;
            if (Fon_color_new.a > 1)
                Fon_color_new.a = 1;

            Fon.color = Fon_color_new;

        }
    }

    void setAvatar() {
        if (steamID != null && PlayerAvatar != null)
        {
            uint width = 0;
            uint height = 0;
            int avatarInt = SteamFriends.GetLargeFriendAvatar(steamID);


            if (avatarInt > 0)
            {
                SteamUtils.GetImageSize(avatarInt, out width, out height);
            }

            if (width > 0 && height > 0)
            {
                byte[] avatarStream = new byte[4 * (int)width * (int)height];
                SteamUtils.GetImageRGBA(avatarInt, avatarStream, 4 * (int)width * (int)height);

                TextureAvatar = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false);
                TextureAvatar.LoadRawTextureData(avatarStream);
                TextureAvatar.Apply();

                PlayerAvatar.texture = TextureAvatar;
                PlayerAvatar.rectTransform.localScale = new Vector2(1, -1);
            }
            else
            {
                Debug.Log("Get Steam AvatarINT Error");
            }
        }
    }
    void setNickname() {
        if (steamID != null && Nickname != null) {
            string nametext = SteamFriends.GetFriendPersonaName(steamID);

            if (nametext != null)
            {
                Nickname.text = nametext;
            }
            else {
                Debug.Log("Error download nickname");
            }
        }
    }
    public void setScore(int score) {
        if (steamID != null) {
            //Загружаем статистику этого пользователя
            SteamAPICall_t user = SteamUserStats.RequestUserStats(steamID);
            if (user != null) {
                int score_result;
                //score_geting_ok = SteamUserStats.GetUserStat(steamID, "scoreTop", out score_result);
                if (true)
                {
                    //Если этот id не мой то просто берем и ставим
                    if (steamID != SteamUser.GetSteamID())
                    {
                        Score.text = Convert.ToString(score);
                        scoreInt = score;
                    }
                    //Иначе проверяем то что получили с тем что сейчас в игре
                    else {
                        GameplayParametrs gameParam = GameObject.FindGameObjectWithTag("setings_game").GetComponent<GameplayParametrs>();
                        if (gameParam != null && gameParam.CoofST > score)
                        {
                            Score.text = Convert.ToString((int)gameParam.CoofST);
                            scoreInt = (int)gameParam.CoofST;
                        }
                        else {
                            Score.text = Convert.ToString(score);
                            scoreInt = score;
                        }
                    }
                }
                else {
                    Score.text = "No game";
                }
            }
        }
    }
    

    public void setNum(int num) {
        FriendPosition.text = Convert.ToString(num);

        if (steam_Achievement != null && num == 1 && Nickname.text == SteamFriends.GetPersonaName()) {
            steam_Achievement.the_first();
        }
    }
    public int getScore() {
        return scoreInt;
    }

    void test_update_data() {
        time_re_update += Time.deltaTime;
        if (time_re_update >= 5) {
            time_re_update = 0;

            //setScore();
        }
    }
}
