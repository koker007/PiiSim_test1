using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class loadingText : MonoBehaviour {

    Text LoadingText;

    [SerializeField]
    string[] texts_loading;

    float TimeText = 5;
    float timeTextNow = 3;

    void iniText() {
        if (LoadingText == null) {
            LoadingText = gameObject.GetComponent<Text>();
        }
    }

	// Use this for initialization
	void Start () {
        iniText();
    }
	
	// Update is called once per frame
	void Update () {
        TestText();
    }

    void TestText() {
        timeTextNow -= Time.deltaTime;
        if (timeTextNow < 0) {

            if (LoadingText != null) {
                if (LoadingText.text == "Loading" && texts_loading != null && texts_loading.Length > 0)
                {
                    LoadingText.text = texts_loading[Random.Range(0, texts_loading.Length)];
                    timeTextNow = TimeText;
                }
                else {
                    LoadingText.text = "Loading";
                    timeTextNow = TimeText - TimeText/2;
                }
            }
        }
    }
}
