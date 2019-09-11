using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class svetofor : MonoBehaviour {
    [SerializeField]
    Track_point line_0_point_1;
    [SerializeField]
    Track_point line_0_point_2;

    [SerializeField]
    Track_point line_1_point_1;
    [SerializeField]
    Track_point line_1_point_2;

    [SerializeField]
    Track_point line_2_point_1;
    [SerializeField]
    Track_point line_2_point_2;

    [SerializeField]
    Track_point line_3_point_1;
    [SerializeField]
    Track_point line_3_point_2;

    float timeout_max_1 = 5;
    float timeout_max_2 = 5;
    float timeout_max_3 = 5;
    float timeout_max_4 = 5;

    float num_line_go_now = 0;
    float time_line_to_end = 0;


    float speed_0 = 0;
    float speed_1 = 0;
    float speed_2 = 0;
    float speed_3 = 0;

    Player player;
    void iniPlayer() {
        if (player == null) {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }
    }

    // Use this for initialization
    void Start () {
        HideMesh();
        iniPlayer();

        //запоминаем обычные скорости
        if (line_0_point_1 != null)
            speed_0 = line_0_point_1.max_speed;
        if (line_1_point_1 != null)
            speed_1 = line_1_point_1.max_speed;
        if (line_2_point_1 != null)
            speed_2 = line_2_point_1.max_speed;
        if (line_3_point_1 != null)
            speed_3 = line_3_point_1.max_speed;
	}
	
	// Update is called once per frame
	void Update () {
        TestPerecrestok();

    }

    //Спрятать
    void HideMesh()
    {
        MeshRenderer mesh = gameObject.GetComponent<MeshRenderer>();
        if (mesh != null)
        {
            mesh.enabled = false;
        }
    }

    void TestPerecrestok() {
        if (player != null && player.ScoreTime < 20000)
        {
            time_line_to_end -= Time.deltaTime;

            //Сперва проверяем сколько времени осталось для переключения
            if (time_line_to_end <= 0)
            {
                //Пришло время переключать
                //убираем скорость у нынешней
                if (num_line_go_now == 0 && line_0_point_1 != null)
                {
                    speed_0 = line_0_point_1.max_speed;
                    line_0_point_1.max_speed = 0;
                    if (line_0_point_2 != null)
                        line_0_point_2.max_speed = 0;
                }
                else if (num_line_go_now == 1 && line_1_point_1 != null)
                {
                    speed_1 = line_1_point_1.max_speed;
                    line_1_point_1.max_speed = 0;
                    if (line_1_point_2 != null)
                        line_1_point_2.max_speed = 0;
                }
                else if (num_line_go_now == 2 && line_2_point_1 != null)
                {
                    speed_2 = line_2_point_1.max_speed;
                    line_2_point_1.max_speed = 0;
                    if (line_2_point_2 != null)
                        line_2_point_2.max_speed = 0;
                }
                else if (num_line_go_now == 3 && line_3_point_1 != null)
                {
                    speed_3 = line_3_point_1.max_speed;
                    line_3_point_1.max_speed = 0;
                    if (line_3_point_2 != null)
                        line_3_point_2.max_speed = 0;
                }

                //Занулили, теперь переключаемся на следующее
                num_line_go_now++;
                if (num_line_go_now == 1)
                {
                    if (line_1_point_1 != null)
                    {
                        line_1_point_1.max_speed = speed_1;
                        if (line_1_point_2 != null)
                        {
                            line_1_point_2.max_speed = speed_1;
                        }
                    }
                    else
                    {
                        num_line_go_now = 0;
                    }
                }
                else if (num_line_go_now == 2)
                {
                    if (line_2_point_1 != null)
                    {
                        line_2_point_1.max_speed = speed_2;
                        if (line_2_point_2 != null)
                        {
                            line_2_point_2.max_speed = speed_2;
                        }
                    }
                    else
                    {
                        num_line_go_now = 0;
                    }
                }
                else if (num_line_go_now == 3)
                {
                    if (line_3_point_1 != null)
                    {
                        line_3_point_1.max_speed = speed_3;
                        if (line_3_point_2 != null)
                        {
                            line_3_point_2.max_speed = speed_3;
                        }
                    }
                    else
                    {
                        num_line_go_now = 0;
                    }
                }
                if (num_line_go_now == 0)
                {
                    if (line_0_point_1 != null)
                    {
                        line_0_point_1.max_speed = speed_0;
                        if (line_0_point_2 != null)
                        {
                            line_0_point_2.max_speed = speed_0;
                        }
                    }
                    else
                    {
                        num_line_go_now = 0;
                    }
                }

                //Скорость была изменена
                //ставим таймер
                time_line_to_end = 15;

            }
        }
        else {
            if(line_0_point_1 != null)
                line_0_point_1.max_speed = speed_0;
            if (line_0_point_2 != null)
                line_0_point_2.max_speed = speed_0;
            if (line_1_point_1 != null)
                line_1_point_1.max_speed = speed_1;
            if (line_1_point_2 != null)
                line_1_point_2.max_speed = speed_1;
            if (line_2_point_1 != null)
                line_2_point_1.max_speed = speed_2;
            if (line_2_point_2 != null)
                line_2_point_2.max_speed = speed_2;
            if (line_3_point_1 != null)
                line_3_point_1.max_speed = speed_3;
            if (line_3_point_2 != null)
                line_3_point_2.max_speed = speed_3;
        }
    }
}
