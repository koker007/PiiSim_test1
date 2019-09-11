using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InerciaPii{

    public Vector2 Smehenie;

    float speed = 0;
    float grad = 0;

    float gradSpeed = 0;
    float speedSpeed = 0;

    //Расчитать новое направление
    public void NewCalc() {

        //изменяем временную скорость
        gradSpeed = Random.Range(-0.1f, 0.1f);
        speedSpeed = Random.Range(-0.1f, 0.1f);

        //Проверка на границы
        if (grad >= 360)
            grad = 0;
        else if (grad < 0)
            grad = 360;

        if (speed > 0.2f)
            speed = 0.2f;
        else if (speed < -0.2f)
            speed = -0.2f;


        //Считаем итоговую скорость
        grad += gradSpeed;
        speed += speedSpeed;

        //Считаем направление
        float x = Mathf.Sin(grad);
        float y = Mathf.Cos(grad);

        //Считем итоговое смещение
        Smehenie = new Vector2(speed * x, speed * y);

    }
}
