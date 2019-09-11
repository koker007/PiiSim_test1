using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wall_broken_creator : MonoBehaviour {

    [SerializeField]
    GameObject[] models_broken_wall;
    GameObject model;

    [SerializeField]
    bool left_yn = false;
    [SerializeField]
    bool up_yn = false;

	// Use this for initialization
	void Start () {
        create_model();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    //Создать модель
    void create_model() {
        //Если есть что создавать
        if (models_broken_wall.Length > 0)
        {
            //Скрываем квадрат
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            gameObject.GetComponent<BoxCollider>().enabled = false;

            //Узнаем какую модель создать
            int num_model = Random.Range(0, models_broken_wall.Length);
            model = Instantiate(models_broken_wall[num_model], gameObject.transform);
            //Перемешаем
            model.transform.position = gameObject.transform.position;

            if (left_yn)
            {
                model.transform.Rotate(0, 180, 0);
            }
            if (up_yn) {
                model.transform.Rotate(90, 0, 0);
            }

            //Если нужно развурнуть
            if (Random.Range(0, 100) < 50) {
                model.transform.Rotate(-90, 0, 0);
                model.transform.Rotate(0, 0, -180);
            }
        }
        else {
            Destroy(gameObject);
        }
    }
}
