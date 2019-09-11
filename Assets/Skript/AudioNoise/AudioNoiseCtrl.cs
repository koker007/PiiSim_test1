using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioNoiseCtrl : MonoBehaviour {

    [SerializeField]
    setings seting;

    //Звуки заднего фона
    //тип звука
    [SerializeField]
    AudioClip[] home;
    [SerializeField]
    AudioClip[] street;
    [SerializeField]
    AudioClip[] armageddon;

    float volume_max = 0.5f;

    //Тип звука
    int typeNoise_now = 0;
    int typeNoise_need = 0;

    //Источник звуков
    AudioSource audioSource;

	// Use this for initialization
	void Start () {
        get_setings();
        //Получения источника
        audioSource = gameObject.GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
        TestVolume();
        TestPlaying();
	}

    //Проверка воспроизведения
    void TestPlaying() {
        if (audioSource != null) {
            //Расчет скорости изменения звука
            float volume_speed = volume_max / 1;

            //Проверка ожидание - реальность
            if (typeNoise_now != typeNoise_need)
            {
                //Если не совпадает проверяем громкость
                if (audioSource.volume != 0)
                {
                    audioSource.volume = audioSource.volume - volume_speed * Time.deltaTime;
                    if (audioSource.volume < 0)
                        audioSource.volume = 0;
                }
                else
                {
                    typeNoise_now = typeNoise_need;
                    audioSource.Stop();
                }


            }
            else if (audioSource.volume != volume_max)
            {
                if (audioSource.volume < volume_max)
                {
                    audioSource.volume = audioSource.volume + volume_speed * Time.deltaTime;
                    if (audioSource.volume > volume_max)
                        audioSource.volume = volume_max;
                }
                else if (audioSource.volume > volume_max) {
                    audioSource.volume = audioSource.volume - volume_speed * Time.deltaTime;
                    if (audioSource.volume < volume_max)
                        audioSource.volume = volume_max;
                }

            }
            //проверка ожидание - реальность, завершена
        }



        //Если источник есть и звук не воспроизводится
        if (audioSource != null && !audioSource.isPlaying) {

            //Воспроизведение нужного клипа
            int numer_sound = 0;
            //Узнаем какой тип звука воспроизводить
            //Дом
            if (typeNoise_now == 0 && home.Length != 0) {
                numer_sound = Random.Range(0, home.Length);
                audioSource.PlayOneShot(home[numer_sound]);
            }
            //Улица
            else if (typeNoise_now == 1 && street.Length != 0) {
                numer_sound = Random.Range(0, street.Length);
                audioSource.PlayOneShot(street[numer_sound]);
            }
            //Армагеддон
            else if (typeNoise_now == 2 && armageddon.Length != 0) {
                numer_sound = Random.Range(0, armageddon.Length);
                audioSource.PlayOneShot(armageddon[numer_sound]);
            }
        }
    }

    void TestVolume() {
        if(seting != null && seting.game != null)
            volume_max = 0.5f * seting.game.volume_all * seting.game.volume_sound;
    }
    void get_setings() {
        //Если настроек нет
        if (seting == null)
        {
            //ищем обьект по тегу
            GameObject obj = GameObject.FindWithTag("setings_game");
            //вытаскиваем настройки
            if (obj != null) seting = obj.GetComponent<setings>();
        }
    }

    public void SetTypeNoise(int num_type) {
        //проверка есть ли вообще такой тип
        if ((num_type == 0 && home.Length != 0) ||
            (num_type == 1 && street.Length != 0) ||
            (num_type == 2 && armageddon.Length != 0)
            ) {
            typeNoise_need = num_type;
        }
    }
}
