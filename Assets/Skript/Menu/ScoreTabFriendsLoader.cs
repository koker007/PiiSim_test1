using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class ScoreTabFriendsLoader : MonoBehaviour {

    [SerializeField]
    GameObject PrefabScoreTabFriend;

    bool need_re_update = false;

    class zvenoStatTab {
        //информация
        public GameObject ScoreFriend;
        //Следующее звено
        public zvenoStatTab Next;

        public void AddZveno(zvenoStatTab addZveno, GameObject score) {
            zvenoStatTab now = this;

            bool end_stack_yn = false;
            while (!end_stack_yn)
            {
                //Проверяем есть ли следующее звено
                if (now.Next != null)
                {
                    now = now.Next;
                }
                //Если звена нет то создаем новое
                else
                {
                    now.Next = addZveno;
                    now.Next.ScoreFriend = score;
                    end_stack_yn = true;
                }
            }
        }
    }

    zvenoStatTab ZvenoStart;


	// Use this for initialization
	void Start () {
        calculateTab();
    }
	
	// Update is called once per frame
	void Update () {
        test_recalc();
        SotringScore();
        Vizualize();
    }

    void calculateTab() {
        //Узнаем общее количество друзей
        int friends_max = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);
        if (friends_max > 0) {
            //получаем массив друзей
            CSteamID[] friends_ID = new CSteamID[friends_max];
            for (int num_now = 0; num_now < friends_max; num_now++) {
                friends_ID[num_now] = SteamFriends.GetFriendByIndex(num_now, EFriendFlags.k_EFriendFlagImmediate);
            }
            
            //Перебираем и добавляем пользователя в список
            for (int num_now = 0; num_now < friends_ID.Length; num_now++) {
                AddZvenoSteamID(friends_ID[num_now]);
            }
            AddZvenoSteamID(SteamUser.GetSteamID());
            
        }
        if (friends_max == 0) {
            need_re_update = true;
        }
    }


    //Добавить звено
    void AddZvenoSteamID(CSteamID SteamID)
    {
        bool ok = true;
        //Cоздаем звено
        zvenoStatTab NewZveno = new zvenoStatTab();
        GameObject ScoreResult = Instantiate(PrefabScoreTabFriend, gameObject.transform);

        //Заполняем
        ScoreTabSteamID ScoreSteamID = ScoreResult.GetComponent<ScoreTabSteamID>();
        if (ScoreSteamID != null)
        {
            ScoreSteamID.inicializationSteamID(SteamID);
            //Получаем очки


        }
        else {
            ok = false;
        }

        //Если ошибок нет
        if (ok) {
            if (ZvenoStart == null)
            {
                ZvenoStart = NewZveno;
                ZvenoStart.ScoreFriend = ScoreResult;
            }
            else {
                ZvenoStart.AddZveno(NewZveno, ScoreResult);
            }
        }
        //Если данные с ошибкой то удаляем их
        else{
            Destroy(ScoreResult);
            need_re_update = true;
        }
    }

    //Отсортировать данные
    void SotringScore() {

        bool end = false;

        //Проверяем пока что-то не найдем или цикл не кончится
        zvenoStatTab zvenoNow = ZvenoStart;
        if (zvenoNow != null) {
            while (!end)
            {
                //Проверяем есть ли следующее звено
                if (zvenoNow.Next != null)
                {
                    //Проверяем значение звена на правильность
                    if (!zvenoNow.Next.ScoreFriend.GetComponent<ScoreTabSteamID>().score_geting_ok)
                    {
                        //Запоминаем звено необходиоме удалить
                        zvenoStatTab needDelite = zvenoNow.Next;

                        //Исправляем разрыв
                        zvenoNow.Next = needDelite.Next;

                        //Удаляем звено
                        GameObject.Destroy(needDelite.ScoreFriend);
                        //Говорим что закончили
                        end = true;
                    }
                    else {
                        //Если очки есть сравниваем настоящее со следующим
                        if (zvenoNow.ScoreFriend.GetComponent<ScoreTabSteamID>().scoreInt < zvenoNow.Next.ScoreFriend.GetComponent<ScoreTabSteamID>().scoreInt ||
                            !zvenoNow.ScoreFriend.GetComponent<ScoreTabSteamID>().score_geting_ok)
                        {
                            //Нужно поменять их местами
                            //Запоминаем настоящее
                            GameObject OBJNOW = zvenoNow.ScoreFriend;
                            GameObject OBJNEXT = zvenoNow.Next.ScoreFriend;

                            zvenoNow.ScoreFriend = OBJNEXT;
                            zvenoNow.Next.ScoreFriend = OBJNOW;

                            end = true;
                        }
                        //Если все ок то переходим к следующему звену
                        else {
                            zvenoNow = zvenoNow.Next;
                        }
                    }
                }
                else {
                    end = true;
                }
            }
        }
        
    }
    //Визуализировать
    void Vizualize() {
        if (ZvenoStart != null) {
            //Узнаем обычный размер
            float size = PrefabScoreTabFriend.GetComponent<RectTransform>().rect.height;
            float positon_now = 1f;

            zvenoStatTab ZvenoNow = ZvenoStart;

            int num_pos = 1;
            bool end = false;
            while (!end) {
                //Меняем текст позиции
                ZvenoNow.ScoreFriend.GetComponent<ScoreTabSteamID>().setNum(num_pos);

                //Меняем позицию этого результата
                Vector3 positionNew = ZvenoNow.ScoreFriend.GetComponent<RectTransform>().position;
                Vector2 pivotNew = ZvenoNow.ScoreFriend.GetComponent<RectTransform>().pivot;

                pivotNew.y = positon_now;

                ZvenoNow.ScoreFriend.GetComponent<RectTransform>().pivot = pivotNew;
                ZvenoNow.ScoreFriend.GetComponent<RectTransform>().position = positionNew;

                //Проверяем есть ли следующее звено
                if (ZvenoNow.Next != null)
                {
                    ZvenoNow = ZvenoNow.Next;
                }
                //Если звена нет то создаем новое и записываем текст
                else
                {
                    end = true;
                }

                positon_now++;
                num_pos++;
            }

            //изменяем размер чтобы появился ползунок
            RectTransform rectMain = GetComponent<RectTransform>();
            rectMain.sizeDelta = new Vector2(rectMain.sizeDelta.x, size * num_pos);
        }
    }

    void test_recalc() {
        if (need_re_update) {
            Debug.Log("Recalcing Tab");
            need_re_update = false;
            ZvenoStart = null;
            calculateTab();
        }
    }
}
