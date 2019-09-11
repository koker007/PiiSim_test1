using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using System;
using UnityEngine.UI;

public class ScoreTabWorldLoader : MonoBehaviour {

    [SerializeField]
    Text NameLeaderBoard;
    bool nameGet_ok = false;
    void iniNameText() {
        if (!nameGet_ok && NameLeaderBoard != null && asuncSteam != null) {
            nameGet_ok = true;
            if (asuncSteam.leaderboardName != "CoofTop2019")
            {
                NameLeaderBoard.text += " " + asuncSteam.leaderboardName;
            }
            else {
                NameLeaderBoard.text += " " + 2019;
            }
        }
    }

    [SerializeField]
    GameObject PrefabScoreTabWorld;

    asuncFuncSteam asuncSteam;
    void iniAsuncSteam() {
        if (asuncSteam == null) {
            asuncSteam = GameObject.FindGameObjectWithTag("steam_manager").GetComponent<asuncFuncSteam>();
        }
    }

    bool NeedReDrawInfo = true;
    [SerializeField]
    Text Amount;
    [SerializeField]
    Text Offset;

    string LeaderboardName = null;
    int LeaderboardMaxCount = 0;

    //нужно ли отрисовать заного таблицу
    bool NeedReDraw = true;
    int SumUserLoaded = 0;

    //нужно ли перерисовать заного топ?
    bool NeewReDrawTop = true;
    [SerializeField]
    ScoreTabWorldSteamID Pos1;
    [SerializeField]
    ScoreTabWorldSteamID Pos2;
    [SerializeField]
    ScoreTabWorldSteamID Pos3;

    public float TimeUpdate = 0;

    void iniLiderboard() {

    }

    // Use this for initialization
    void Start () {
        iniAsuncSteam();
    }
	
	// Update is called once per frame
	void Update () {
        TestTimeReDraw();

        TestReDrawLeader();
        TestReDrawTop();
        TestReTabInfo();

        iniNameText();
    }

    void TestReDrawLeader() {
        //Нужно ли отрисовать? и получена ли таблица?
        if (NeedReDraw && asuncSteam != null && asuncSteam.getterLeaderboardDownload) {
            NeedReDraw = false;
            //Запрос начинает выполнение

            //Сперва очищаем табицу от предыдущего результата
            SumUserLoaded = 0;
            ScoreTabWorldSteamID[] oldList = gameObject.GetComponentsInChildren<ScoreTabWorldSteamID>();
            if (oldList != null && oldList.Length > 0) {
                for (int num_now = 0; num_now < oldList.Length; num_now++) {
                    if (oldList[num_now] != null) {
                        Destroy(oldList[num_now].gameObject);
                    }
                }
            }

            //Таблица очищена грузим новую
            if (PrefabScoreTabWorld != null) {
                //Перебираем все результаты
                for (int num_now = 0; num_now < asuncSteam.SteamLeaderboardScoresDownloaded.m_cEntryCount; num_now++) {
                    //Вытаскиваем позицию игрока
                    LeaderboardEntry_t leaderboardEntry;
                    int[] details = new int[5];
                    bool ok_load = SteamUserStats.GetDownloadedLeaderboardEntry(asuncSteam.SteamLeaderboardScoresDownloaded.m_hSteamLeaderboardEntries, num_now, out leaderboardEntry, details, 5);
                    if (ok_load) {
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

                        ScoreTabWorldSteamID scoreTabWorldSteamID = playerResultObj.GetComponent<ScoreTabWorldSteamID>();
                        scoreTabWorldSteamID.SetPlayer(leaderboardEntry);
                    }
                }
            }
            
        }
        
    }
    void TestReDrawTop() {
        if (NeewReDrawTop && asuncSteam != null && asuncSteam.getterLeaderboardDownloadTOP) {
            NeewReDrawTop = false;

            bool pl1_ok = false;
            bool pl2_ok = false;
            bool pl3_ok = false;

            for (int num_now = 0; num_now < asuncSteam.SteamleaderboardScoresDownloaded_TOP.m_cEntryCount; num_now++) {
                //Вытаскиваем позицию игрока
                LeaderboardEntry_t leaderboardEntry;
                int[] details = new int[5];
                bool ok_load = SteamUserStats.GetDownloadedLeaderboardEntry(asuncSteam.SteamleaderboardScoresDownloaded_TOP.m_hSteamLeaderboardEntries, num_now, out leaderboardEntry, details, 5);

                if (ok_load) {
                    if (leaderboardEntry.m_nGlobalRank == 1 && Pos1 != null)
                    {
                        Pos1.gameObject.SetActive(true);
                        Pos1.SetPlayer(leaderboardEntry);
                        pl1_ok = true;
                    }

                    if (leaderboardEntry.m_nGlobalRank == 2 && Pos2 != null)
                    {
                        Pos2.gameObject.SetActive(true);
                        Pos2.SetPlayer(leaderboardEntry);
                        pl2_ok = true;
                    }

                    if (leaderboardEntry.m_nGlobalRank == 3 && Pos3 != null)
                    {
                        Pos3.gameObject.SetActive(true);
                        Pos3.SetPlayer(leaderboardEntry);
                        pl3_ok = true;
                    }

                }
            }

            if (!pl1_ok && Pos1 != null)
                Pos1.gameObject.SetActive(false);
            if (!pl2_ok && Pos2 != null)
                Pos2.gameObject.SetActive(false);
            if (!pl3_ok && Pos3 != null)
                Pos3.gameObject.SetActive(false);
        }
    }
    void TestReTabInfo() {
        if (NeedReDrawInfo && asuncSteam != null && asuncSteam.getterLeaderboardDownload) {
            NeedReDrawInfo = false;

            if (Amount != null) {
                Amount.text = System.Convert.ToString(asuncSteam.SteamLeaderboardScoreUploaded.m_nGlobalRankNew);
            }
        }
        if (Offset != null && asuncSteam != null)
        {
            Offset.text = System.Convert.ToString(asuncSteam.LeaderboardSmeshenie);
        }
    }

    void TestTimeReDraw() {
        TimeUpdate += Time.deltaTime;

        if (TimeUpdate > 10 && asuncSteam != null)
        {
            TimeUpdate = 0;

            asuncSteam.getterLeaderboardDownload = false;
            asuncSteam.getterLeaderboardDownloadTOP = false;

            NeedReDraw = true;
            NeewReDrawTop = true;
            NeedReDrawInfo = true;
        }
    }

    public void OffsetPlus() {
        if (asuncSteam != null) {
            asuncSteam.ButtonOffsetPlus();
            NeedReDraw = true;
        }
    }
    public void OffsetMinus()
    {
        if (asuncSteam != null)
        {
            asuncSteam.ButtonOffsetMinus();
            NeedReDraw = true;
        }
    }
}
