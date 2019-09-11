using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamSoundFull : MonoBehaviour {

    setings seting;

    [SerializeField]
    AudioClip[] clip;

    AudioSource source;

	// Use this for initialization
	void Start () {
        source = gameObject.GetComponent<AudioSource>();
        get_seting_game();
	}
	
	// Update is called once per frame
	void Update () {
		
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

    public void PlaySound() {
        if (source != null && clip != null && clip.Length != 0) {
            if (seting != null)
                source.volume = seting.game.volume_all * seting.game.volume_sound;

            source.pitch = Random.Range(0.9f, 1.1f);
            source.PlayOneShot(clip[Random.Range(0, clip.Length - 1)]);

        }
    }
}
