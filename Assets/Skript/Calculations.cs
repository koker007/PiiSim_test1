using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Calculations{

    //Для уменьшения выгоды коофициент должен быть от 1 до 2
    //Для увеличения выгоды коофициент должен быть от 0 до 1
    //Коофициент не должен быть больше 2 или меньше 0, а число отрицательным
    public static float coofing_num(float number, float coefficient)
    {
        //Проверка границ
        if (number < 0)
            number = 0;

        if (coefficient < 0)
            coefficient = 0;
        else if (coefficient > 2)
            coefficient = 2;

        float up = Mathf.Pow(number, 2);
        float down = Mathf.Pow(number, coefficient);

        float return_number = 0;
        if (down != 0)
        {
            return_number = up / down;
        }

        return return_number;
    }

}
