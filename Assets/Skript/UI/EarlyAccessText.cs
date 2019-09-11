using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EarlyAccessText: MonoBehaviour {

    Text logoText;

	// Use this for initialization
	void Start () {
        logoText = gameObject.GetComponent<Text>();
        DemoTest();

    }
	
	// Update is called once per frame
	void Update () {
        TestAlpha();
    }

    void TestAlpha() {
        if (logoText != null) {
            Color colorNew = logoText.color;
            if (colorNew.a > 0) {
                colorNew.a -= Time.deltaTime * 0.15f;
            }
            if (colorNew.a <= 0) {
                colorNew.a = 0.5f;
            }
            logoText.color = colorNew;
        }
    }

    void DemoTest() {
        GameplayParametrs gameplayParametrs = GameObject.FindGameObjectWithTag("setings_game").GetComponent<GameplayParametrs>();
        if (gameplayParametrs != null && !gameplayParametrs.ThisDemoVersion) {
            gameObject.active = false;
        }
    }
}
