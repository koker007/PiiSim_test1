using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSound : MonoBehaviour {

    setings seting;

    [SerializeField]
    private AudioClip[] Destroy;
    [SerializeField]
    private AudioClip[] Hit;

    AudioSource source;
    PiiTarget target;

    // Use this for initialization
    void Start () {
        source = gameObject.GetComponent<AudioSource>();
        target = gameObject.GetComponent<PiiTarget>();
        get_seting_game();
    }
	
	// Update is called once per frame
	void Update () {
        TestVolume();
    }

    public void PlaySoundDestroy() {
        //Проверяем есть ли вообще звук уничтожения
        if (Destroy != null && Destroy.Length != 0 && source != null) {
             source.pitch = Random.Range(0.8f, 1.2f);
             source.PlayOneShot(Destroy[Random.Range(0, Destroy.Length)]);
        }
    }

    public bool PlaySoundPii() {
        //проверка есть ли возможность воспроизвести звук?
        if (Hit != null && Hit.Length != 0)
        {
            //проверяем что сурс найден
            if (source == null || target == null )
                return false;

            //Смотрим сколько HP у предмета в процентах

            //Меняем питч от оставшихся хп
            source.pitch = 0.9f + ((100 - target.heath_percent) / 100) * 0.3f;

            //воспросизводим рандомный звук
            source.PlayOneShot(Hit[Random.Range(0, Hit.Length)]);

            return true;
        }
        else {
            return false;
        }
        
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

    void TestVolume()
    {
        if (seting != null && seting.game != null && seting.game.new_data_setings_yn)
        {
            if (source != null)
            {
                source.volume = 0.8f * seting.game.volume_all * seting.game.volume_sound;
            }
            else {
                source = gameObject.AddComponent<AudioSource>();
                source.priority = 128;
                source.spatialBlend = 0.8f;
                source.minDistance = 1;
                source.maxDistance = 500;
            }
        }
    }

}
