using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class police_car : MonoBehaviour {
    Automobile automobile;
    setings seting;
    Player player;
    void iniPlayer() {
        if (player == null) {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }
    }

    public bool police_active_now = false;
    bool police_active_old = false;

    [SerializeField]
    ParticleSystem siren_blue;
    [SerializeField]
    ParticleSystem siren_red;

    AudioSource SirenSource;
    public bool siren_short_need_play = false;


    public bool need_sound_yn = false;
    [SerializeField]
    AudioClip Siren_long;
    [SerializeField]
    AudioClip Siren_short;
    float pause_long_siren = 0;
    public float pause_play_siren = 0; //Пауза перед запуском сирены

    //Пауза до запуска следующей фары сирены
    float pause_next_siren_play = 0;

	// Use this for initialization
	void Start () {
        automobile = gameObject.GetComponent<Automobile>();
        //Создаем источник под сирену
        CreateSirenSource();
        Ini_siren();
        iniPlayer();
    }
	
	// Update is called once per frame
	void Update () {
        TestSiren_visual();
        TestSiren_sound();
        get_seting_game();
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

    void TestSiren_visual() {
        //Проверка жизни и выключение если умерла
        PiiTarget piiTarget = gameObject.GetComponent<PiiTarget>();
        if (piiTarget != null && piiTarget.heath <= 0) {
            police_active_now = false;
        }
        
        //Если нужно выключить
        if (!police_active_now && police_active_old) {
            police_active_old = false;
            if (siren_blue != null) {
                siren_blue.Stop();
            }
            if (siren_red != null) {
                siren_red.Stop();
            }
        }

        //Если первый запуск
        if (police_active_now && !police_active_old) {
            police_active_old = true;
            //Запускаем первый фонарь сирены
            if (siren_blue != null) {
                siren_blue.Play();
            }
            //Ставим таймер запуска второй сирены
            pause_next_siren_play = 0.5f;
        }
        //Уменьшаем врямя и запускаем другую сирену если время вышло
        pause_next_siren_play -= Time.deltaTime;
        if (police_active_now && pause_next_siren_play <= 0 && siren_red != null && !siren_red.isPlaying) {
            siren_red.Play();
        }
        
        
        
    }

    float gameoverTime = 0;
    float gameoverTimeStop = 10;
    void TestSiren_sound() {

        if (player != null && SirenSource != null)
        {
            float playerDist = Vector3.Distance(gameObject.transform.position, player.transform.position);
            float MaxSirenDist = 1500;
            if (playerDist > MaxSirenDist)
                playerDist = MaxSirenDist;

            int cootnoshenie = (int)((playerDist / MaxSirenDist) * 200);

            SirenSource.priority = 50 + cootnoshenie;
        }

        //Если сирена включена но звука нет запускаем
        //Проверка жизни и выключение если умерла
        PiiTarget piiTarget = gameObject.GetComponent<PiiTarget>();

        pause_play_siren -= Time.deltaTime;
        if (pause_play_siren < 0)
            pause_play_siren = 0;

        if (need_sound_yn && police_active_now && piiTarget.heath > 0 && pause_play_siren <= 0) {
            if (seting != null)
                SirenSource.volume = 0.5f * seting.game.volume_all * seting.game.volume_sound;

            //Если нужно длинную
            if (!siren_short_need_play && (SirenSource.clip != Siren_long || SirenSource.clip == null || !SirenSource.isPlaying))
            {
                //Уменьшаем таймер задержки запуска
                pause_long_siren -= Time.deltaTime;

                if (pause_long_siren < 0)
                {
                    SirenSource.clip = Siren_long;
                    SirenSource.loop = true;
                    SirenSource.Play();
                }
            }
            else if (siren_short_need_play && (SirenSource.clip != Siren_short || SirenSource.clip == null || !SirenSource.isPlaying)) {
                SirenSource.clip = Siren_short;
                SirenSource.loop = true;
                SirenSource.Play();

                pause_long_siren = 1f;
            }
        }

        GameplayParametrs gameplayParametrs = GameObject.FindGameObjectWithTag("setings_game").GetComponent<GameplayParametrs>();
        if (gameplayParametrs != null) {
            if (gameplayParametrs.GameOver) {
                gameoverTime += Time.deltaTime;
            }

            if (gameplayParametrs.pause || gameoverTime > gameoverTimeStop)
            {
                SirenSource.Pause();
            }
            else {
                SirenSource.UnPause();
            }
        }

        //Выключить при смерти или если звук не нужен
        if ((piiTarget != null && piiTarget.heath <= 0 && SirenSource.isPlaying) || !need_sound_yn || pause_play_siren > 0) {
            SirenSource.Stop();
        }

    }

    void CreateSirenSource() {
        if (SirenSource == null) {
            SirenSource = gameObject.AddComponent<AudioSource>();
            SirenSource.priority = 226;
            SirenSource.volume = 0.0f;
            SirenSource.spatialBlend = 0.9f;

            SirenSource.minDistance = 20;
            SirenSource.maxDistance = 500;
        }
    }
    void Ini_siren() {
        gameoverTimeStop = Random.Range(3.0f, 15.0f);
        if (siren_blue != null) {
            siren_blue.Stop();
        }
        if (siren_red != null) {
            siren_red.Stop();
        }
    }
}
