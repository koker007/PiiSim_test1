using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageWindow : MonoBehaviour {

    [SerializeField]
    bool start_clear = false;
    [SerializeField]
    float speed_sec = 1;

    [SerializeField]
    Image left;
    [SerializeField]
    Image right;
    [SerializeField]
    Image up;
    [SerializeField]
    Image down;



    float alpha_left_need = 0;
    float alpha_right_need = 0;
    float alpha_up_need = 0;
    float alpha_down_need = 0;

	// Use this for initialization
	void Start () {
        start_zero_alpha();
    }
	
	// Update is called once per frame
	void Update () {
        calc_need_to_normal();
        calc_to_need();
    }

    void start_zero_alpha() {

        if (start_clear)
        {
            Color leftnew = left.color;
            Color rightnew = right.color;
            Color upnew = up.color;
            Color downnew = down.color;

            leftnew.a = 0;
            rightnew.a = 0;
            upnew.a = 0;
            downnew.a = 0;

            left.color = leftnew;
            right.color = rightnew;
            up.color = upnew;
            down.color = downnew;
        }
    }

    //Уменьшает стремление до нуля
    void calc_need_to_normal() {

        alpha_left_need = alpha_left_need - alpha_left_need / speed_sec * Time.deltaTime;
        alpha_right_need = alpha_right_need - alpha_right_need / speed_sec * Time.deltaTime;
        alpha_up_need = alpha_up_need - alpha_up_need / speed_sec * Time.deltaTime;
        alpha_down_need = alpha_down_need - alpha_down_need / speed_sec * Time.deltaTime;

    }

    //бвигает альфу до стремления
    void calc_to_need() {
        //Узнаем разницу на которую нужно сместить
        float smeshenie_left = left.color.a - alpha_left_need;
        float smeshenie_right = right.color.a - alpha_right_need;
        float smeshenie_up = up.color.a - alpha_up_need;
        float smeshenie_down = down.color.a - alpha_down_need;

        Color leftnew = left.color;
        Color rightnew = right.color;
        Color upnew = up.color;
        Color downnew = down.color;

        leftnew.a = leftnew.a - (smeshenie_left / speed_sec * Time.deltaTime);
        rightnew.a = rightnew.a - (smeshenie_right / speed_sec * Time.deltaTime);
        upnew.a = upnew.a - (smeshenie_up / speed_sec * Time.deltaTime);
        downnew.a = downnew.a - (smeshenie_down / speed_sec * Time.deltaTime);

        left.color = leftnew;
        right.color = rightnew;
        up.color = upnew;
        down.color = downnew;
    }

    //Утанавливает стремление
    public void set_all_need(float need_left, float need_right, float need_up, float need_down) {

        if (alpha_left_need < need_left && need_left <= 1)
            alpha_left_need = need_left;

        if(alpha_right_need < need_right && need_right <= 1)
            alpha_right_need = need_right;

        if(alpha_up_need < need_up && need_up <= 1)
            alpha_up_need = need_up;

        if (alpha_down_need < need_down && need_down <= 1)
            alpha_down_need = need_down;



        if (need_left > 1)
            alpha_left_need = 1;

        if (need_right > 1)
            alpha_right_need = 1;

        if (need_up > 1)
            alpha_up_need = 1;

        if (need_down > 1)
            alpha_down_need = 1;

    }


}
