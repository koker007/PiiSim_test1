using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class lvl_text : MonoBehaviour {

    public float alpha_speed = 2;
    private bool closing = false;

    public string text_next = " ";

    private Text text;
    public float alpha;

	// Use this for initialization
	void Start () {
       text = gameObject.GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        draw_text();
	}

    private void draw_text() {
        //Проверяем чему сейчас равна альфа
        //Если альфа не равна нулю то изменяем альфу
        alpha = text.color.a;

        if (text.color.a > 0)
        {
            Color color = text.color;

            //ветка опен
            if (closing == false)
            {
                color.a += alpha_speed * Time.deltaTime;
                if (color.a >= 1)
                {
                    color.a = 1;
                    closing = true;
                }
            }
            //Ветка клос
            else
            {
                color.a -= alpha_speed * Time.deltaTime;
                if (color.a < 0)
                {
                    color.a = 0;
                }
            }
            text.color = color;
        }
        //Если альфа нулевая то меняем текст и стартуем
        else {
            if (text.text != text_next) {
                text.text = text_next;

                closing = false;

                Color color = text.color;
                color.a += alpha_speed * Time.deltaTime;
                text.color = color;
            }
        }


    }
}
