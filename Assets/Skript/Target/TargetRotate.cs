using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Скрипт придающий обьекту-цели вращение до тех пор пока оно не мертвое
public class TargetRotate : MonoBehaviour {

    //скорость вращения
    [SerializeField]
    float speed_rotate = 1;
    //прибавляем или убавляем
    [SerializeField]
    bool plus_rotate_yn = true;

    public float size_x = 0;
    public float size_y = 0;
    public float size_z = 0;

    //Рандомное вращение
    [SerializeField]
    bool random_pause = false;
    float pause = 0;
    float rotate_now = 0;
    float rotate_need = 0;

    //Вытаскиваем цель
    PiiTarget target;

	// Use this for initialization
	void Start () {
        target = gameObject.GetComponent<PiiTarget>();
	}
	
	// Update is called once per frame
	void Update () {
        TestRotate();
	}

    //Тест вращения
    void TestRotate() {
        //есть ли цель? она жива?
        if (target != null && target.heath > 0) {
            pause -= Time.deltaTime;

            //Узнаем насколько
            float rotate_now = speed_rotate * Time.deltaTime;

            //Если режим с паузами
            if (random_pause) {
                //Если нету паузы
                if (pause <= 0)
                {
                    pause = 0;
                    this.rotate_now += rotate_now;

                    if (plus_rotate_yn)
                    {
                        gameObject.transform.Rotate(0, 0, rotate_now);
                    }
                    else
                    {
                        gameObject.transform.Rotate(0, 0, rotate_now * -1);
                    }
                    //Если вращение достаточно долгое останавливаем
                    if (this.rotate_now > rotate_need)
                    {
                        pause = Random.Range(2, 20);
                    }
                }
                //Если пауза
                else
                {
                    //сбрасываем движение
                    this.rotate_now = 0;
                    rotate_need = Random.Range(10, 180);

                    //рандомная сторона вращения
                    float random_vector = Random.Range(0, 100);
                    if (random_vector > 50)
                        plus_rotate_yn = false;
                    else plus_rotate_yn = true;
                }
            }
            //Если режим без пауз
            if (!random_pause) {
                if (plus_rotate_yn)
                {
                    gameObject.transform.Rotate(0, 0, rotate_now);
                }
                else
                {
                    gameObject.transform.Rotate(0, 0, rotate_now * -1);
                }
            }

            //вытаскиваем колайдер коробки
            BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();
            if (boxCollider != null) {
                //Vector3 vector3 = new Vector3(boxCollider.transform.localScale.x * boxCollider.size.x, boxCollider.transform.localScale.y * boxCollider.size.y, boxCollider.transform.localScale.z * boxCollider.size.z);

                //Проверка столкновения
                Collider[] colliders = Physics.OverlapBox(boxCollider.transform.position, new Vector3(size_x, size_y, size_z), gameObject.transform.rotation);

                //Если есть то вращаем обратно
                if (colliders != null && colliders.Length > 0) {
                    if (plus_rotate_yn)
                    {
                        gameObject.transform.Rotate(0, 0, rotate_now * -1);
                        plus_rotate_yn = false;
                    }
                    else
                    {
                        gameObject.transform.Rotate(0, 0, rotate_now * 1);
                        plus_rotate_yn = true;
                    }


                }
            }
        }
    }
}
