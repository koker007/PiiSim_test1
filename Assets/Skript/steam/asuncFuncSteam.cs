using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class asuncFuncSteam : MonoBehaviour {

    float timeout = 0;

    //Таблица результатов
    public bool getterLeaderboard = false;
    //private string leaderboardName = "testing_scoreTop2";
    //public string leaderboardName = "CoofTop2019";
    public string leaderboardName = "";
    void iniLeaderBoardName() {
        System.DateTime dateTimeNow = System.DateTime.Now;
        int yearNow = dateTimeNow.Year;
        if (yearNow > 2019) {
            leaderboardName = System.Convert.ToString(dateTimeNow.Year);
        }
    }

    private CallResult<LeaderboardFindResult_t> m_SteamLeaderboard_t;
    public LeaderboardFindResult_t SteamLeaderboard;
    private void IniSteamLeaderboard(LeaderboardFindResult_t pCallback, bool bIOFailure)
    {
        if (pCallback.m_hSteamLeaderboard.m_SteamLeaderboard == 0 || bIOFailure)
        {
            Debug.Log("SteamLeaderboard_t Error");
        }
        else
        {
            Debug.Log("SteamLeaderboard_t OK");
            SteamLeaderboard = pCallback;
            getterLeaderboard = true;
        }
    }

    //3 лидера
    public bool getterLeaderboardDownloadTOP = false;
    public LeaderboardScoresDownloaded_t SteamleaderboardScoresDownloaded_TOP;
    private CallResult<LeaderboardScoresDownloaded_t> m_LeaderboardScoresDownloaded_TOP;
    private void IniSteamLeaderboardScoresDownloadedTOP(LeaderboardScoresDownloaded_t pCallback, bool bIOFailure)
    {
        if (pCallback.m_hSteamLeaderboard.m_SteamLeaderboard == 0 || bIOFailure)
        {
            Debug.Log("SteamLeaderboard_t Error");
        }
        else
        {
            Debug.Log("SteamLeaderboard_t OK");
            SteamleaderboardScoresDownloaded_TOP = pCallback;
            getterLeaderboardDownloadTOP = true;
        }
    }

    //Подготовленная таблица результатов
    public bool getterLeaderboardDownload = false; //Изменяя этот параметр таблица будет пересчитана
    public int LeaderboardSmeshenie = -1; // Смещение таблицы, после изменения необходимо пересчитать
    public LeaderboardScoresDownloaded_t SteamLeaderboardScoresDownloaded;
    private CallResult<LeaderboardScoresDownloaded_t> m_LeaderboardScoresDownloaded_t;
    private void IniSteamLeaderboardScoresDownloaded(LeaderboardScoresDownloaded_t pCallback, bool bIOFailure)
    {
        if (pCallback.m_hSteamLeaderboard.m_SteamLeaderboard == 0 || bIOFailure)
        {
            Debug.Log("SteamLeaderboard_t Error");
        }
        else
        {
            Debug.Log("SteamLeaderboard_t OK");
            SteamLeaderboardScoresDownloaded = pCallback;
            getterLeaderboardDownload = true;
        }
    }

    //таблица друзей
    public bool getterLeaderboardDownloadFriends = false;
    public bool TestFirst = false;
    public LeaderboardScoresDownloaded_t SteamleaderboardScoresDownloaded_Friends;
    private CallResult<LeaderboardScoresDownloaded_t> m_LeaderboardScoresDownloaded_Friends;
    private void IniSteamLeaderboardScoresDownloadedFriends(LeaderboardScoresDownloaded_t pCallback, bool bIOFailure)
    {
        if (pCallback.m_hSteamLeaderboard.m_SteamLeaderboard == 0 || bIOFailure)
        {
            Debug.Log("SteamLeaderboard_t Friends Error");
        }
        else
        {
            Debug.Log("SteamLeaderboard_t Friends OK");
            SteamleaderboardScoresDownloaded_Friends = pCallback;
            getterLeaderboardDownloadFriends = true;
        }
    }

    //Загружены ли очки в таблицу лидеров - загрузка очков
    public bool uploadScore = false;
    public bool reWriteTop = false;
    public int countScore = 0;
    public LeaderboardScoreUploaded_t SteamLeaderboardScoreUploaded;
    private CallResult<LeaderboardScoreUploaded_t> m_LeaderboardScoreUploaded_t;
    private void IniLeaderboardScoreUploaded(LeaderboardScoreUploaded_t pCallback, bool bIOFailure)
    {
        if (pCallback.m_bSuccess == 0 || bIOFailure)
        {
            Debug.Log("SteamLeaderboardScoreUploaded Error");
        }
        else
        {
            Debug.Log("SteamLeaderboardScoreUploaded OK");
            SteamLeaderboardScoreUploaded = pCallback;
            uploadScore = true;
            reWriteTop = false;
        }
    }

    private void Update()
    {
        timeout -= Time.deltaTime;
        if (timeout < 0)
            timeout = 0;

        if (timeout <= 0) {
            timeout = 1;
            if (!getterLeaderboard)
            {
                SteamAPICall_t handle = SteamUserStats.FindOrCreateLeaderboard(leaderboardName, ELeaderboardSortMethod.k_ELeaderboardSortMethodDescending, ELeaderboardDisplayType.k_ELeaderboardDisplayTypeNumeric);
                m_SteamLeaderboard_t.Set(handle);
            }
            //Получение топовых игроков
            else if (getterLeaderboard && !getterLeaderboardDownloadTOP)
            {
                Debug.Log("getterLeaderboardDownload_TOP test");
                SteamLeaderboard_t steamLeaderboard_T;
                steamLeaderboard_T.m_SteamLeaderboard = SteamLeaderboard.m_hSteamLeaderboard.m_SteamLeaderboard;
                SteamAPICall_t handle = SteamUserStats.DownloadLeaderboardEntries(steamLeaderboard_T, ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal, 1, 3);
                m_LeaderboardScoresDownloaded_TOP.Set(handle);
            }
            //Получение участка списка лидеров
            else if (getterLeaderboard && !getterLeaderboardDownload)
            {
                Debug.Log("getterLeaderboardDownload test");
                SteamLeaderboard_t steamLeaderboard_T;
                steamLeaderboard_T.m_SteamLeaderboard = SteamLeaderboard.m_hSteamLeaderboard.m_SteamLeaderboard;
                int min = -5 + LeaderboardSmeshenie * 10;
                int max = 4 + LeaderboardSmeshenie * 10;
                SteamAPICall_t handle = SteamUserStats.DownloadLeaderboardEntries(steamLeaderboard_T, ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobalAroundUser, min, max);
                m_LeaderboardScoresDownloaded_t.Set(handle);
            }
            else if (getterLeaderboard && !getterLeaderboardDownloadFriends)
            {
                Debug.Log("getterLeaderboardDownload_Friends test");
                SteamLeaderboard_t steamLeaderboard_T;
                steamLeaderboard_T.m_SteamLeaderboard = SteamLeaderboard.m_hSteamLeaderboard.m_SteamLeaderboard;
                SteamAPICall_t handle = SteamUserStats.DownloadLeaderboardEntries(steamLeaderboard_T, ELeaderboardDataRequest.k_ELeaderboardDataRequestFriends, 1, 3);
                m_LeaderboardScoresDownloaded_Friends.Set(handle);
            }
            else if (!TestFirst && getterLeaderboard && getterLeaderboardDownloadFriends) {
                TestFirst = true;
                LeaderboardEntry_t leaderboardEntry;
                int[] details = new int[5];
                bool ok_load = SteamUserStats.GetDownloadedLeaderboardEntry(SteamleaderboardScoresDownloaded_Friends.m_hSteamLeaderboardEntries, 0, out leaderboardEntry, details, 5);
                if (ok_load)
                {
                    if (leaderboardEntry.m_steamIDUser == SteamUser.GetSteamID())
                    {
                        gameObject.GetComponent<steam_achievement>().the_first();
                    }
                }
            }
            //Загрузка очков в таблицу
            else if (getterLeaderboard && !uploadScore)
            {
                SteamLeaderboard_t steamLeaderboard_T;
                steamLeaderboard_T.m_SteamLeaderboard = SteamLeaderboard.m_hSteamLeaderboard.m_SteamLeaderboard;
                int[] testArray = new int[5];
                SteamAPICall_t handle;
                if (!reWriteTop)
                {
                    handle = SteamUserStats.UploadLeaderboardScore(steamLeaderboard_T, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest, countScore, testArray, 5);
                }
                else
                {
                    handle = SteamUserStats.UploadLeaderboardScore(steamLeaderboard_T, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodForceUpdate, countScore, testArray, 5);
                }
                m_LeaderboardScoreUploaded_t.Set(handle);
            }
        }
    }

    // Use this for initialization
    void Start () {
        iniLeaderBoardName();
        if (SteamManager.Initialized)
        {
            m_SteamLeaderboard_t = CallResult<LeaderboardFindResult_t>.Create(IniSteamLeaderboard);
            m_LeaderboardScoresDownloaded_TOP = CallResult<LeaderboardScoresDownloaded_t>.Create(IniSteamLeaderboardScoresDownloadedTOP);
            m_LeaderboardScoresDownloaded_t = CallResult<LeaderboardScoresDownloaded_t>.Create(IniSteamLeaderboardScoresDownloaded);
            m_LeaderboardScoresDownloaded_Friends = CallResult<LeaderboardScoresDownloaded_t>.Create(IniSteamLeaderboardScoresDownloadedFriends);
            m_LeaderboardScoreUploaded_t = CallResult<LeaderboardScoreUploaded_t>.Create(IniLeaderboardScoreUploaded);
        }
    }

    public void UploadScoreInLeaderboard(int NewScore, bool reWriteTopFunc) {
        uploadScore = false; //говорим что нужно будет загрузить новый результат
        reWriteTop = reWriteTopFunc;
        countScore = NewScore;
    }

    public void ButtonOffsetMinus() {
        LeaderboardSmeshenie--;
        getterLeaderboardDownload = false;

    }
    public void ButtonOffsetPlus() {
        LeaderboardSmeshenie++;
        getterLeaderboardDownload = false;
    }
}
