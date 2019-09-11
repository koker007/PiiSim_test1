using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Скрипт для привязывания конкретной настройки к слайдеру
public class slider_option : MonoBehaviour {

    setings seting;
    Slider slider;
    bool geting = false;
    float value_old = 0;

    [SerializeField]
    bool volume_all = false;
    [SerializeField]
    bool volume_music = false;
    [SerializeField]
    bool volume_sound = false;
    [SerializeField]
    bool speed_controller = false;

	// Use this for initialization
	void Start () {
        get_slider();
        get_setings();
    }
	
	// Update is called once per frame
	void Update () {
        set_start_value();
        test_new_value();
    }

    void get_slider() {
        slider = gameObject.GetComponent<Slider>();
    }
    void get_setings() {
        //Если настроек нет
        if (seting == null)
        {
            //ищем обьект по тегу
            GameObject main_canvas = GameObject.FindWithTag("setings_game");
            //вытаскиваем настройки
            if (main_canvas != null) seting = main_canvas.GetComponent<setings>();
        }
    }

    //вытаскивание значений
    void set_start_value() {
        if (!geting && seting != null && seting.game != null && slider != null) {
            geting = true;

            if (volume_all)
                slider.value = seting.game.volume_all;
            else if (volume_music)
                slider.value = seting.game.volume_music;
            else if (volume_sound)
                slider.value = seting.game.volume_sound;

            else if (speed_controller) {
                slider.value = seting.game.speedController;
            }

        }
    }

    //
    void test_new_value() {
        //Если новое значение
        if (geting && seting != null && slider != null && slider.value != value_old) {
            value_old = slider.value;
            //То заносим его
            if (volume_all)
                seting.game.volume_all = slider.value;
            else if (volume_music)
                seting.game.volume_music = slider.value;
            else if (volume_sound)
                seting.game.volume_sound = slider.value;

            else if (speed_controller)
                seting.game.speedController = slider.value;
        }
    }
}
