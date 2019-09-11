using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Attention : MonoBehaviour {

    [SerializeField]
    GameObject ArrowPresure;

    Image attention;
    PiiController bolt;

    [SerializeField]
    AudioClip audioClip;
    AudioSource audioSource;

    setings seting;

    float[] presure_old = new float[10];
    float presure_time_old = 0;

    bool alert = false;

	// Use this for initialization
	void Start () {
        set_attention_image();
        set_bolt();
        set_seting();
        create_audio_source();
    }
	
	// Update is called once per frame
	void Update () {
        test_attention_indicator2();
        test_attention_visualization();
    }

    void set_attention_image() {
        attention = gameObject.GetComponent<Image>();
        
    }
    void set_bolt() {
        Player player = null;
        //Если настроек нет
        if (player == null)
        {
            //ищем обьект по тегу
            GameObject obj = GameObject.FindWithTag("Player");
            //вытаскиваем настройки
            if (obj != null) player = obj.GetComponent<Player>();

            if (player != null) {
                bolt = player.bolt.GetComponent<PiiController>();
            }
        }
    }
    void set_seting() {
        //Если настроек нет
        if (seting == null)
        {
            //ищем обьект по тегу
            GameObject obj = GameObject.FindWithTag("setings_game");
            //вытаскиваем настройки
            if (obj != null) seting = obj.GetComponent<setings>();
        }
    }
    void create_audio_source() {
        if (audioClip != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.volume = seting.game.volume_all * seting.game.volume_sound;
            audioSource.priority = 10;
            audioSource.spatialBlend = 0;

        }
    }

    void test_attention_indicator() {
        //Если картинка и болт есть то работаем
        if (attention != null && bolt != null) {
            //Загоняем новое значение
            set_new_presure(bolt.speed_pii);

            //Считаем среднее
            float presure_time_now = 0;
            for (int num_now = 0; num_now < presure_old.Length; num_now++) {
                presure_time_now = presure_old[num_now];
            }
            presure_time_now = presure_time_now / presure_old.Length;

            //находим сдвиг
            float sdvig = presure_time_now - presure_time_old;
            presure_time_old = presure_time_now;

            float min_porog = presure_time_now / 100;
            //Если раньше было больше чем сейчас и тревога выключена
            if (sdvig < min_porog * -1  && !alert) {
                //Включаем тревогу
                alert = true;
            }

        }
    }
    void test_attention_indicator2() {
        //Debug.Log(ArrowPresure.transform.rotation.z);
        if (ArrowPresure != null && ArrowPresure.transform.localRotation.z >= 0.2f && !alert) {
            alert = true;
        }
    }

    void test_attention_visualization() {
        if (attention != null) {
            Color new_color = attention.color;

            if (new_color.a > 0)
            {
                new_color.a -= Time.deltaTime * 1;
                if (new_color.a < 0)
                {
                    new_color.a = 0;
                }

            }
            else if(alert) {
                alert = false;

                //включаем индикатор
                new_color.a = 1;

                //Включаем звук
                playSound();
            }
            attention.color = new_color;
        }
    }
    void playSound() {
        if (audioSource != null && audioClip != null && seting != null) {
            //меняем громкость
            audioSource.volume = seting.game.volume_all * seting.game.volume_sound;
            audioSource.PlayOneShot(audioClip);
        }
    }

    void set_new_presure(float new_value) {
        for (int num_now = 0; num_now < presure_old.Length; num_now++) {
            if (num_now != presure_old.Length - 1)
            {
                presure_old[num_now] = presure_old[num_now + 1];
            }
            else {
                presure_old[num_now] = new_value;
            }
        }
    }
}
