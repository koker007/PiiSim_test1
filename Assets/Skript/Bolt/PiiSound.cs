using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiiSound : MonoBehaviour {

    setings seting;

    [SerializeField]
    private Player player;

    [SerializeField]
    private AudioSource source1;
    [SerializeField]
    private AudioSource source2;


    [SerializeField]
    private AudioClip[] piiSounds;


    public float volume = 0.7f;

    private int index_sound1_old = 999;
    private int index_sound2_old = 999;

    float oldPiiTime = 0;
    private int OldID = 0;

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

    // Update is called once per frame
    void Update () {
        testTimeLastPii();
        get_seting_game();
    }

    //Проверка на то что пападения были недавно
    void testTimeLastPii() {
        oldPiiTime += Time.deltaTime;
        if (oldPiiTime > 1)
        {
            source1.mute = true;
            source2.mute = true;
        }
        else {
            source1.mute = false;
            source2.mute = false;
        }
    }

    //Расчет того какой звук проигрывать изходя из напора
    void testPresure(float presure) {
        if (presure <= 0)
        {
            source1.mute = true;
            source2.mute = true;
        }
        //Если напор есть
        else {
            source1.mute = false;
            source2.mute = false;

            //полный напор для одного звука
            float onePresure = player.maxPresure / piiSounds.Length;

            //надо запомнить какой индекс у первого звука и какой у второго
            int index_sound1_now = (int)(presure / onePresure);
            int index_sound2_now = index_sound1_now + 1;

            //проверка на выход за пределы
            if (index_sound1_now > piiSounds.Length - 1)
                index_sound1_now = piiSounds.Length - 1;
            if (index_sound2_now > piiSounds.Length - 1)
                index_sound2_now = piiSounds.Length - 1;

            //Теперь мы знаем какие звуки надо воспроизводить в данный момент, проверяем отличаются ли они от предыдущих
            if (index_sound1_now != index_sound1_old) {
                source1.clip = piiSounds[index_sound1_now];
                source1.Play();
                index_sound1_old = index_sound1_now;
            }
            if (index_sound2_now != index_sound2_old) {
                source2.clip = piiSounds[index_sound2_now];
                source2.Play();
                index_sound2_old = index_sound2_now;
            }

            //Звуки изменены

            //Теперь надо изменить громкость
            if (index_sound1_now == index_sound2_now)
            {
                source1.volume = 0 * volume * seting.game.volume_all * seting.game.volume_sound;
                source2.volume = 1 * volume * seting.game.volume_all * seting.game.volume_sound;
            }
            else {
                //Находим левый предел
                float minPresuse = onePresure * index_sound1_now;
                //Находим дробную часть напора
                float ostatokPresure = presure - minPresuse;
                //Вычисляем процент от одного целого напора
                float Volume1 = ostatokPresure / onePresure;
                //Процент для второго звука
                float Volume2 = 1 - Volume1;

                source1.volume = Volume2 * volume * seting.game.volume_all * seting.game.volume_sound;
                source2.volume = Volume1 * volume * seting.game.volume_all * seting.game.volume_sound;
            }

        }
    }

    public void set_new_pii(PiiPrefab piiPrefab) {
        //Получили новый префаб мочи
        oldPiiTime = 0;

        //Проверяем ID
        if (OldID < piiPrefab.ID) {
            OldID = piiPrefab.ID;

            //Перемещаем звук в положение префаба
            Vector3 position = piiPrefab.transform.position;
            gameObject.transform.position = position;

            //Проверяем какой напор у префаба
            testPresure(piiPrefab.Presure);
        }

    }
}
