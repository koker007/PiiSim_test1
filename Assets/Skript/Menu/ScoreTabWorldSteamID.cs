using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using System;

public class ScoreTabWorldSteamID : MonoBehaviour {
    
    private float time_re_update = 0;
    private LeaderboardEntry_t leaderboardEntry;

    [SerializeField]
    Text positionInBoardText;
    [SerializeField]
    RawImage avatarImage;
    Texture2D TextureAvatar;
    [SerializeField]
    Text Nickname;
    [SerializeField]
    Text Score;

    [SerializeField]
    Color Base_color;
    [SerializeField]
    Color Friend_color;
    bool plusCoof_yn = true;

    //Множитель для фонового цвета
    float colorCoofTime = 1;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        TestColorCoof();

    }

    public void SetPlayer(LeaderboardEntry_t player) {
        leaderboardEntry = player;

        if (positionInBoardText != null) {
            positionInBoardText.text = System.Convert.ToString(leaderboardEntry.m_nGlobalRank);
        }
        if (Nickname != null) {
            Nickname.text = SteamFriends.GetFriendPersonaName(leaderboardEntry.m_steamIDUser);
        }
        if (Score != null) {
            Score.text = System.Convert.ToString(leaderboardEntry.m_nScore);
        }

        if (avatarImage != null) {
            uint width = 0;
            uint height = 0;
            int avatarInt = SteamFriends.GetMediumFriendAvatar(leaderboardEntry.m_steamIDUser);


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

                avatarImage.texture = TextureAvatar;
                avatarImage.rectTransform.localScale = new Vector2(1, -1);
            }
            else
            {
                Debug.Log("Get Steam AvatarINT Error");
            }
        }
    }

    public void TestColorCoof() {
        if (leaderboardEntry.m_steamIDUser == SteamUser.GetSteamID()) {
            float coofMax = 0.5f;
            float coofMin = 0.3f;
            float coofSpeed = 0.5f;

            //меняем коофицент
            if (plusCoof_yn)
            {
                colorCoofTime += coofSpeed * Time.deltaTime;
                if (colorCoofTime > coofMax)
                {
                    colorCoofTime = coofMax;
                    plusCoof_yn = false;
                }
            }
            else {
                colorCoofTime -= coofSpeed * Time.deltaTime;
                if (colorCoofTime < coofMin)
                {
                    colorCoofTime = coofMin;
                    plusCoof_yn = true;
                }
            }

            //Меняем цвет с базового
            Color colorNew = Base_color;
            colorNew.a = colorCoofTime;
            //colorNew.r = colorNew.r * colorCoofTime;
            //colorNew.g = colorNew.g * colorCoofTime;
            //colorNew.b = colorNew.b * colorCoofTime;

            gameObject.GetComponent<Image>().color = colorNew;

        }
    }
}
