using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSoundPlay : MonoBehaviour {

    [SerializeField]
    AudioClip audioClip;
    AudioSource audioSource;
    setings seting;

	// Use this for initialization
	void Start () {
        audioSource = gameObject.GetComponent<AudioSource>();

        //ищем обьект по тегу
        GameObject main_canvas = GameObject.FindWithTag("setings_game");
        //вытаскиваем настройки
        if (main_canvas != null) seting = main_canvas.GetComponent<setings>();

    }
	
	// Update is called once per frame
	void Update () {
        playSound();
    }

    bool playOld = false;
    void playSound() {
        if (!playOld && audioSource != null && audioClip != null && seting != null)
        {
            playOld = true;
            audioSource.volume = seting.game.volume_all * seting.game.volume_sound;
            audioSource.clip = audioClip;
            audioSource.Play();
        }
    }
}
