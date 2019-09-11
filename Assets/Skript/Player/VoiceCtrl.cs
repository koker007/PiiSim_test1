using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceCtrl : MonoBehaviour {

    [SerializeField]
    setings seting;

    public float voice_volume = 1;


    AudioClip last_clip;
    bool second_try = false;

    AudioSource voice;

    void stop_and_play(AudioClip clip) {

        //Если сейчас проигрывается какой-то звук останавливаем
        if (voice.isPlaying) {
            voice.Stop();
        }
        voice.PlayOneShot(clip);

    }

    public bool get_and_play_clip(AudioClip new_clip) {

        //Это новый звук
        if (last_clip != new_clip)
        {
            second_try = false;
            //Воспроизводим
            stop_and_play(new_clip);
            last_clip = new_clip;

            return true;
        }
        else {
            //Если раньше уже был отказ
            if (second_try)
            {
                second_try = false;
                //Воспроизводим
                stop_and_play(new_clip);
                last_clip = new_clip;

                return true;
            }
            //Если ранее отказа не было, отказываем
            else {
                second_try = true;
                return false;
            }
        }


    }

    public bool is_playing_now() {
        return gameObject.GetComponent<AudioSource>().isPlaying;
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

    // Use this for initialization
    void Start () {
        voice = gameObject.GetComponent<AudioSource>();
        get_seting_game();
    }
	
	// Update is called once per frame
	void Update () {
        test_volume();
    }

    void test_volume() {
        if (seting != null && seting.game.new_data_setings_yn)
            voice.volume = voice_volume * seting.game.volume_all * seting.game.volume_sound;
    }
}
