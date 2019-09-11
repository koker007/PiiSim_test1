using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Score_text_3d : MonoBehaviour {

    private GameObject player;

    private float time_live;
    [SerializeField]
    private float time_live_max = 1;

    private float distance_player = 1;

    [SerializeField]
    Color32 color_start;
    [SerializeField]
    Color32 color_end;

    TextMesh text;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        drawing();
	}

    public void inicialize(GameObject player_func, string text_func, Vector3 position, Color color_func) {
        player = player_func;

        distance_player = Vector3.Distance(gameObject.transform.position, player.transform.position);

        gameObject.transform.position = position;

        Vector3 vector3 = gameObject.transform.position;
        vector3.x = vector3.x + (vector3.x - player.transform.position.x);
        vector3.y = vector3.y + (vector3.y - player.transform.position.y);
        vector3.z = vector3.z + (vector3.z - player.transform.position.z);


        gameObject.transform.LookAt(vector3);

        time_live = 0;

        color_start = color_func;
        gameObject.GetComponent<TextMesh>().text = text_func;
        gameObject.GetComponent<TextMesh>().color = color_start;

        text = gameObject.GetComponent<TextMesh>();
    }


    private void drawing() {
        time_live += Time.deltaTime;
        distance_player = Vector3.Distance(gameObject.transform.position, player.transform.position);

        if (gameObject.GetComponent<TextMesh>().text[gameObject.GetComponent<TextMesh>().text.Length - 1] != '%') {
            distance_player *= 2;
        }

        //Если время жизни вышло, удаляем
        if (time_live >= time_live_max) {
            Destroy(gameObject);
        }
        //Иначе рисуем двигаем
        else if(time_live != 0){
            //Узнаем насколько процентов прожита жизнь
            float percent_live = time_live / (time_live_max/100);

            //считаем цвет
            float r_one_per = (color_end.r - color_start.r)/100;
            float g_one_per = (color_end.g - color_start.g)/100;
            float b_one_per = (color_end.b - color_start.b)/100;

            Color32 color_now = new Color32();
            color_now.r = (byte)(color_start.r + percent_live * r_one_per);
            color_now.g = (byte)(color_start.g + percent_live * g_one_per);
            color_now.b = (byte)(color_start.b + percent_live * b_one_per);


            //Поднимаем очки
            Vector3 position = gameObject.transform.position;
            position.y += distance_player * 0.025f * Time.deltaTime;
            gameObject.transform.position = position;

            gameObject.GetComponent<TextMesh>().characterSize = distance_player * 0.05f;
        }
    }
}
