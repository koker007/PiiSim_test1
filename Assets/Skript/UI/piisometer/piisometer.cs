using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class piisometer : MonoBehaviour {

    GameplayParametrs gameplayParametrs;
    Player player;

    [SerializeField]
    GameObject Arrow;
    [SerializeField]
    GameObject signalAlpha;

    public float gradFull = 45;
    public float gradEmpty = -75;

    float alphaNeed = 0;
    bool alphaPlus = false;

	// Use this for initialization
	void Start () {
        iniGameplayParam();
        iniPlayer();
    }
	
	// Update is called once per frame
	void Update () {
        testArrow();
        testSignal();
    }

    void iniGameplayParam() {
        if (gameplayParametrs == null) {
           gameplayParametrs = GameObject.FindGameObjectWithTag("setings_game").GetComponent<GameplayParametrs>();
        }
    }
    void iniPlayer() {
        if (player == null) {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }
    }

    void testArrow() {
        if (gameplayParametrs != null && Arrow != null) {
            //Узнаем процент настоящий
            float percentOne = gameplayParametrs.TimeToEnd_max / 100;
            float percentNow = gameplayParametrs.TimeToEnd / percentOne;
            if (gameplayParametrs.TimeToEnd == 0 && gameplayParametrs.get_lvl_now() != 0)
                percentNow = 0;
            else if(gameplayParametrs.get_lvl_now() == 0) {
                percentNow = 100;
            }

            //Узнали сколько в процентах осталось Узнаем сколько это в градусах
            float gradOnePercent = (gradFull - gradEmpty) / 100;
            float gradNeed = gradOnePercent * percentNow;

            //Получаем градусы стрелки сейчас и плавно двигаем
            float gradNow = Arrow.GetComponent<RectTransform>().rotation.eulerAngles.z;

            //Arrow.GetComponent<RectTransform>().Rotate(0,0, gradNow + ((gradNeed-gradNow) * Time.deltaTime));
            Quaternion RotateNew = new Quaternion();
            Arrow.GetComponent<RectTransform>().rotation = RotateNew;
            if (player != null && player.bolt.GetComponent<PiiController>().ChargeOn && percentNow <= 55)
            {
                Arrow.GetComponent<RectTransform>().Rotate(0, 0, ((gradNeed + gradEmpty) + ((gradNeed + gradEmpty) - (gradNow)) * Time.deltaTime) + 25);
            }
            else {
                Arrow.GetComponent<RectTransform>().Rotate(0, 0, gradNeed + gradEmpty);
            }
        }
    }

    void testSignal() {
        if (gameplayParametrs != null && signalAlpha != null) {
            //Узнаем сколько в процентах осталось времени
            float percentOne = gameplayParametrs.TimeToEnd_max / 100;
            float percentNow = gameplayParametrs.TimeToEnd / percentOne;




            //Получаем цвет
            Color colorNew = signalAlpha.GetComponent<Image>().color;
            //Смещяем
            if (colorNew.a != colorNew.a) {
                colorNew.a = 0;
            }

            if (percentNow < 25 && !gameplayParametrs.OverTime)
            {
                colorNew.a += (alphaNeed - colorNew.a) * Time.deltaTime * (25 - percentNow) / 4;
            }
            else {
                colorNew.a += (0 - colorNew.a) * Time.deltaTime;
            }

            if (colorNew.a >= 0.49f)
            {
                colorNew.a = 0.5f;
                alphaNeed = 0;
            }
            else if(colorNew.a <= 0.01)
            {
                if (percentNow < 25) {
                    alphaNeed = 0.5f;
                }
            }

            //Применяем
            signalAlpha.GetComponent<Image>().color = colorNew;
        }
    }
}
