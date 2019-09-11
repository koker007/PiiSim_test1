using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarStatic : MonoBehaviour {

    setings seting;

    [SerializeField]
    public float SpawnSdvig = 0;

    [SerializeField]
    ParticleSystem LightFaceLeft;

    [SerializeField]
    ParticleSystem LightFaceRight;

    [SerializeField]
    ParticleSystem LightBotmLeft;

    [SerializeField]
    ParticleSystem LightBotmRigth;

    [SerializeField]
    AudioClip[] Signal;
    int MySignal = 0;

    [SerializeField]
    AudioSource Siren;

    [SerializeField]
    int Veroatnost_signals = 90;
    bool Signal_yn = false;

	// Use this for initialization
	void Start () {
        MySignal = Random.Range(0, Signal.Length);
        if (Random.Range(0, 100) < Veroatnost_signals) {
            Signal_yn = true;
        }
        get_seting_game();
        set_siren();

    }
	
	// Update is called once per frame
	void Update () {
        test_volume();

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
    void set_siren() {
        //Создание
        if (Siren == null) {
            Siren = gameObject.AddComponent<AudioSource>();
        }
        //настройка
        if (Siren != null) {
            Siren.spatialBlend = 0.9f;
            Siren.priority = 90;
            Siren.rolloffMode = AudioRolloffMode.Logarithmic;
            Siren.minDistance = 10;
            Siren.maxDistance = 1500;
        }
    }

    void test_volume() {
        if (seting != null && seting.game != null)
            Siren.volume = seting.game.volume_all * seting.game.volume_sound;
    }

    //Запускает сигнал
    public void StartSignal() {

        //Проверяем есть ли звук и если сейчас не работает
        if (Signal_yn && Signal.Length != 0 && Siren != null && !Siren.isPlaying) {
            Siren.PlayOneShot(Signal[MySignal]);

            //Теперь запускаем частицы
            if (LightFaceLeft != null) {
                ParticleSystem.MainModule main = LightFaceLeft.main;
                main.loop = false;
                main.duration = Signal[MySignal].length;

                LightFaceLeft.Play();
            }
            if (LightFaceRight != null) {
                ParticleSystem.MainModule main = LightFaceRight.main;
                main.loop = false;
                main.duration = Signal[MySignal].length;

                LightFaceLeft.Play();
            }
            if (LightBotmLeft != null) {
                ParticleSystem.MainModule main = LightBotmLeft.main;
                main.loop = false;
                main.duration = Signal[MySignal].length;

                LightFaceLeft.Play();
            }
            if (LightBotmRigth != null) {
                ParticleSystem.MainModule main = LightBotmRigth.main;
                main.loop = false;
                main.duration = Signal[MySignal].length;

                LightFaceLeft.Play();
            }
        }


    }
    //Проверяет условия для остановки сигнала
    public void OffSignal() {
        if (Siren != null && Siren.isPlaying) {
            Siren.Stop();
            
            if (LightFaceLeft != null)
            {
                LightFaceLeft.Stop();
            }
            if (LightFaceRight != null)
            {
                LightFaceLeft.Stop();
            }
            if (LightBotmLeft != null)
            {
                LightFaceLeft.Stop();
            }
            if (LightBotmRigth != null)
            {
                LightFaceLeft.Stop();
            }
        }
    }
}
