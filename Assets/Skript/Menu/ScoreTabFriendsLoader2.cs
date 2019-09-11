using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
//using System;
using UnityEngine.UI;

public class ScoreTabFriendsLoader2 : MonoBehaviour {

    [SerializeField]
    GameObject PrefabScoreTabWorld;

    asuncFuncSteam asuncSteam;
    void iniAsuncSteam()
    {
        if (asuncSteam == null)
        {
            asuncSteam = GameObject.FindGameObjectWithTag("steam_manager").GetComponent<asuncFuncSteam>();
        }
    }

    bool NeedReDrawFriends = true;
    int SumUserLoaded = 0;

    public float TimeUpdate = 0;

    // Use this for initialization
    void Start () {
        iniAsuncSteam();

    }
	
	// Update is called once per frame
	void Update () {
        TimeReDraw();
        TestReDrawLeader();

        TestVisioScroll();
    }

    void TestReDrawLeader()
    {
        //Нужно ли отрисовать? и получена ли таблица?
        if (NeedReDrawFriends && asuncSteam != null && asuncSteam.getterLeaderboardDownloadFriends)
        {
            NeedReDrawFriends = false;
            //Запрос начинает выполнение

            //Сперва очищаем табицу от предыдущего результата
            SumUserLoaded = 0;
            ScoreTabSteamID[] oldList = gameObject.GetComponentsInChildren<ScoreTabSteamID>();
            if (oldList != null && oldList.Length > 0)
            {
                for (int num_now = 0; num_now < oldList.Length; num_now++)
                {
                    if (oldList[num_now] != null)
                    {
                        Destroy(oldList[num_now].gameObject);
                    }
                }
            }

            //Таблица очищена грузим новую
            if (PrefabScoreTabWorld != null)
            {
                //Перебираем все результаты
                for (int num_now = 0; num_now < asuncSteam.SteamleaderboardScoresDownloaded_Friends.m_cEntryCount; num_now++)
                {
                    //Вытаскиваем позицию игрока
                    LeaderboardEntry_t leaderboardEntry;
                    int[] details = new int[5];
                    bool ok_load = SteamUserStats.GetDownloadedLeaderboardEntry(asuncSteam.SteamleaderboardScoresDownloaded_Friends.m_hSteamLeaderboardEntries, num_now, out leaderboardEntry, details, 5);
                    if (ok_load)
                    {
                        SumUserLoaded++;
                        //Создаем ячейку
                        GameObject playerResultObj = Instantiate(PrefabScoreTabWorld, gameObject.transform);
                        RectTransform rectTransform = playerResultObj.GetComponent<RectTransform>();

                        //Меняем позицию этого результата
                        Vector3 positionNew = rectTransform.position;
                        Vector2 pivotNew = rectTransform.pivot;

                        pivotNew.y = SumUserLoaded;

                        rectTransform.pivot = pivotNew;
                        rectTransform.position = positionNew;

                        
                        ScoreTabSteamID scoreTabWorldSteamID = playerResultObj.GetComponent<ScoreTabSteamID>();
                        scoreTabWorldSteamID.inicializationSteamID(leaderboardEntry.m_steamIDUser);
                        scoreTabWorldSteamID.setScore(leaderboardEntry.m_nScore);
                        scoreTabWorldSteamID.setNum(SumUserLoaded);
                    }
                }
            }

        }

    }

    void TimeReDraw() {
        TimeUpdate -= Time.deltaTime;
        if (TimeUpdate <= 0) {
            TimeUpdate = 10;

            if (asuncSteam != null) {
                asuncSteam.getterLeaderboardDownloadFriends = false;
                asuncSteam.TestFirst = false;

                NeedReDrawFriends = true;
            }
        }
    }

    //тест видимости
    void TestVisioScroll() {
        //узнаем количество пользователей
        ScoreTabSteamID[] scoreTabSteamsIDs = gameObject.GetComponentsInChildren<ScoreTabSteamID>();

        //изменяем размер чтобы появился ползунок
        RectTransform rectMain = GetComponent<RectTransform>();
        rectMain.sizeDelta = new Vector2(rectMain.sizeDelta.x, PrefabScoreTabWorld.GetComponent<RectTransform>().sizeDelta.y * scoreTabSteamsIDs.Length);
    }
}
