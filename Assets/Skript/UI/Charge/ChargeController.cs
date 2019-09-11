using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargeController : MonoBehaviour {

    setings seting;

    [SerializeField]
    private PiiController piiController;

    [SerializeField]
    private RawImage rawImage;
    Color color_base;
    private float color_base_r = 0;
    private float color_base_g = 0;
    private float color_base_b = 0;
    private float color_percent = 0;
    private bool charge_plus_yn = false; //прибавляем или убавляем?

    [SerializeField]
    private FaceController faceController;

    private Slider slider;

    private AudioSource source;
    private bool OldFull = false;
    private VoiceCharge voice;

	// Use this for initialization
	void Start () {
        source = gameObject.GetComponent<AudioSource>();
        voice = gameObject.GetComponent<VoiceCharge>();

        slider = gameObject.GetComponent<Slider>();

        if (rawImage != null) {
            color_base = rawImage.color;
            if (color_base  != null) {
                color_base_r = color_base.r;
                color_base_g = color_base.g;
                color_base_b = color_base.b;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
        get_seting_game();
        TestValue();
        ImageScroll();
        TestColorImpulse();

        PlaySound();
        TestFullText();

    }

    void TestValue() {
        if (slider != null) {
            slider.maxValue = piiController.ChargeMax;
            slider.value = piiController.ChargeNow;
        }
    }

    //Тест цвета и его изменение от воздержания
    void TestColorImpulse() {
        if (color_base != null)
        {
            //Если сейчас напор
            if (piiController != null && piiController.ChargeNow >= piiController.ChargeMax)
            {
                //Переключаем на повышение
                if (!charge_plus_yn && color_percent <= 0)
                {
                    charge_plus_yn = true;
                }
                //Если в плюс
                if (charge_plus_yn)
                {
                    color_percent = color_percent + (Time.deltaTime * 100)*2.5f;
                    //Проверка выхода за пределы
                    if (color_percent > 100)
                    {
                        color_percent = 100;
                    }
                }

            }

            if (piiController != null)
            {
                //Переключаем на понижение
                if (charge_plus_yn && color_percent >= 100)
                {
                    charge_plus_yn = false;
                }

                if (!charge_plus_yn || piiController.ChargeNow <= piiController.ChargeMax)
                {
                    color_percent = color_percent - (Time.deltaTime * 100)*2.5f;
                    //Проверка выхода за пределы
                    if (color_percent < 0)
                    {
                        color_percent = 0;
                    }
                }
            }

            //Проценты изменены
            //Теперь узнаем цвет который должен быть

            //доля одного процента для красного зеленого и синего
            float per_red = (1 - color_base_r) / 100;
            float per_gre = (1 - color_base_g) / 100;
            float per_blu = (1 - color_base_b) / 100;

            //Узнаем процентное изменение
            per_red = per_red * color_percent;
            per_gre = per_gre * color_percent;
            per_blu = per_blu * color_percent;

            //Заменяем цвет
            color_base.r = color_base_r + per_red;
            color_base.g = color_base_g + per_gre;
            color_base.b = color_base_b + per_blu;
            rawImage.color = color_base;
        }
    }

    void ImageScroll() {
        rawImage.uvRect = new Rect(new Vector2((float)(piiController.speed_pii * Time.deltaTime * 0.1 + rawImage.uvRect.x), 0), new Vector2(slider.value / slider.maxValue, 1));

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

    void PlaySound() {
        //Если есть сурс и аудио файл
        if (source != null) {
            float onePercent = piiController.ChargeMax / 100;
            float nowPercent = piiController.ChargeNow / onePercent;
            source.pitch = 0.5f + ((0.5f * nowPercent) / 100);

            if (piiController.ChargeNow <= 0) {
                if (OldFull) {
                    OldFull = false;
                    voice.playEnd();
                    if (faceController != null)
                    {
                        faceController.set_stress(-99999);
                        faceController.set_bliss(0.8f);
                    }
                }
            }
            else if (piiController.ChargeMax <= piiController.ChargeNow)
            {
                if (!OldFull) {
                    OldFull = true;
                    gameObject.GetComponentInChildren<SteamSoundFull>().PlaySound();
                }
                source.volume = 0;
            }
            else {
                if (seting != null && seting.game != null)
                    source.volume = (0.01f + (0.1f * nowPercent) / 100) * seting.game.volume_all * seting.game.volume_sound;
                if (OldFull) {
                    voice.playProcess();
                    if (faceController != null)
                        faceController.set_stress(0.1f);
                }
            }
        }
    }

    [SerializeField]
    Text textFull;
    [SerializeField]
    string[] textFullstr;

    bool FirstPressDone = false;
    float textFullXNorm = 0;

    void TestFullText() {
        if (piiController != null && textFull != null) {
            if (piiController.ChargeNow >= piiController.ChargeMax)
            {
                if (textFull.text == " ") {
                    //Если первый обход
                    if (!FirstPressDone)
                    {
                        FirstPressDone = true;
                        textFull.text = "Press Mouse!";
                    }
                    else if (textFullstr != null && textFullstr.Length > 0) {
                        textFull.text = textFullstr[Random.Range(0, textFullstr.Length)];
                    }
                }

                RectTransform rectTransformNew = textFull.gameObject.GetComponent<RectTransform>();
                if (textFullXNorm == 0) {
                    textFullXNorm = rectTransformNew.localPosition.x;
                }

                Vector3 posNew = rectTransformNew.localPosition;
                posNew.x += Time.deltaTime * 10;
                if (posNew.x > textFullXNorm + 10) {
                    posNew.x = textFullXNorm;
                }
                rectTransformNew.localPosition = posNew;

            }
            else {
                textFull.text = " ";
            }
        }
    }
}
