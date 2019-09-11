using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

    [SerializeField]
    RawImage numerot0;
    [SerializeField]
    RawImage numerot1;
    [SerializeField]
    RawImage numerot2;
    [SerializeField]
    RawImage numerot3;
    [SerializeField]
    RawImage numerot4;

    [SerializeField]
    Texture image0;
    [SerializeField]
    Texture image1;
    [SerializeField]
    Texture image2;
    [SerializeField]
    Texture image3;
    [SerializeField]
    Texture image4;
    [SerializeField]
    Texture image5;
    [SerializeField]
    Texture image6;
    [SerializeField]
    Texture image7;
    [SerializeField]
    Texture image8;
    [SerializeField]
    Texture image9;

    GameplayParametrs gameplayParametrs;
    void getGameplayParam() {
        gameplayParametrs = GameObject.FindGameObjectWithTag("setings_game").GetComponent<GameplayParametrs>();
    }

    // Use this for initialization
    void Start () {
        getGameplayParam();
        VisualTime(0);
    }
	
	// Update is called once per frame
	void Update () {
        TestVisual();
    }

    Texture getTexture(int num) {
        if (num == 0)
            return image0;
        else if (num == 1)
            return image1;
        else if (num == 2)
            return image2;
        else if (num == 3)
            return image3;
        else if (num == 4)
            return image4;
        else if (num == 5)
            return image5;
        else if (num == 6)
            return image6;
        else if (num == 7)
            return image7;
        else if (num == 8)
            return image8;
        else if (num == 9)
            return image9;
        else
            return image8;

    }
    //Визуализировать время
    void VisualTime(float time) {
        int num4 = (int)(time / 1000);
        int num3 = ((int)(time / 100))   - (num4 * 10);
        int num2 = ((int)(time / 10))    - (num4 * 100)     - (num3 * 10);
        int num1 = ((int)(time / 1))     - (num4 * 1000)    - (num3 * 100)      - (num2 * 10);
        int num0 = (int)((time / 0.1)    - (num4 * 10000)   - (num3 * 1000)     - (num2 * 100) - (num1 * 10));

        bool numVisual = false;
        if (numVisual || num4 != 0){
            Color newColor = numerot4.color;
            newColor.a = 1;
            numerot4.color = newColor;

            numerot4.texture = getTexture(num4);

            numVisual = true;
        }
        else {
            Color newColor = numerot4.color;
            newColor.a = 0;
            numerot4.color = newColor;
        }

        if (numVisual || num3 != 0)
        {
            Color newColor = numerot3.color;
            newColor.a = 1;
            numerot3.color = newColor;
            numerot3.texture = getTexture(num3);

            numVisual = true;
        }
        else
        {
            Color newColor = numerot3.color;
            newColor.a = 0;
            numerot3.color = newColor;
        }

        if (numVisual || num2 != 0)
        {
            Color newColor = numerot2.color;
            newColor.a = 1;
            numerot2.color = newColor;
            numerot2.texture = getTexture(num2);

            numVisual = true;
        }
        else
        {
            Color newColor = numerot2.color;
            newColor.a = 0;
            numerot2.color = newColor;
        }

        if (numVisual || num1 != 0)
        {
            Color newColor = numerot1.color;
            newColor.a = 1;
            numerot1.color = newColor;
            numerot1.texture = getTexture(num1);

            numVisual = true;
        }
        else
        {
            Color newColor = numerot1.color;
            newColor.a = 0;
            numerot1.color = newColor;
        }

        if (numVisual || num0 != 0)
        {
            Color newColor = numerot0.color;
            newColor.a = 1;
            numerot0.color = newColor;
            numerot0.texture = getTexture(num0);

            numVisual = true;
        }
        else
        {
            Color newColor = numerot0.color;
            newColor.a = 0;
            numerot0.color = newColor;
        }
    }

    void TestVisual() {
        if (gameplayParametrs != null) {
            VisualTime(gameplayParametrs.TimeGamePlay);
        }
    }
}
