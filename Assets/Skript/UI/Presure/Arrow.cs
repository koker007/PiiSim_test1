using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{

    [SerializeField]
    private PiiController piiController;

    float[] oldPresure = new float[20];

    [SerializeField]
    float arrowSpeedFor360Grad = 1;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        testAngleArrow();
    }

    void testAngleArrow()
    {
        //Сперва сдвигаем все данные
        for (int num = 1; num < oldPresure.Length; num++)
        {
            oldPresure[num - 1] = oldPresure[num];
        }
        //Запоминаем новое
        oldPresure[oldPresure.Length - 1] = piiController.speed_pii;

        //Находим среднее
        float srednee = 0;
        for (int num = 0; num < oldPresure.Length; num++)
        {
            srednee += oldPresure[num];
        }
        srednee = srednee / oldPresure.Length;

        //Выясняем насколько новое значение отличается от среднего
        float raznoct = piiController.speed_pii - srednee;

        //Узнаем какая скорость для одного угла
        float OneSpeed = 360 / arrowSpeedFor360Grad;

        //Узнаем угл поворота стрелки
        float GradArrowNow = OneSpeed * raznoct * -1;

        //Меняем угол
        gameObject.transform.localRotation = Quaternion.Euler(0, 0, GradArrowNow);
    }
}