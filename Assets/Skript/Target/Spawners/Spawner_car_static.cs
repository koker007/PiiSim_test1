using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner_car_static : MonoBehaviour {

    [SerializeField]
    private GameObject[] cars_police;

    [SerializeField]
    private GameObject[] cars_legkovay;

    [SerializeField]
    private GameObject[] cars_gruzovay;

    [SerializeField]
    private GameObject[] cars_avtobuss;

    public float void_ver = 50;

    public float police_ver = 50;
    public bool police = false;

    public float legkovaya_ver = 50;
    public bool legkovaya = false;

    public float gruzovaya_ver = 50;
    public bool gruzovaya = false;

    public float avtobuss_ver = 50;
    public bool avtobuss = false;


	// Use this for initialization
	void Start () {
        Spawn();
        HideMesh();
    }
	
	// Update is called once per frame
	void Update () {
    }


    //Спавнить автомобиль
    void Spawn() {
        bool spawn_ok = false;
        while (!spawn_ok) {
            int type = Random.Range(0, 4);
            GameObject car = null;

            if (type == 0 && void_ver > Random.Range(0, 100)) {
                spawn_ok = true;
            }
            else if (type == 1 && police_ver > Random.Range(0, 100) && cars_police.Length != 0 && police == true)
            {
                car = Instantiate(cars_police[Random.Range(0, cars_police.Length)], gameObject.transform);
                spawn_ok = true;
            }
            else if (type == 2 && legkovaya_ver > Random.Range(0, 100) && cars_legkovay.Length != 0 && legkovaya == true)
            {
                car = Instantiate(cars_legkovay[Random.Range(0, cars_legkovay.Length)], gameObject.transform);
                spawn_ok = true;
            }
            else if (type == 3 && gruzovaya_ver > Random.Range(0, 100) && cars_gruzovay.Length != 0 && gruzovaya == true)
            {
                car = Instantiate(cars_gruzovay[Random.Range(0, cars_gruzovay.Length)], gameObject.transform);
                spawn_ok = true;
            }
            else if (type == 4 && avtobuss_ver > Random.Range(0, 100) && cars_avtobuss.Length != 0 && avtobuss == true)
            {
                car = Instantiate(cars_avtobuss[Random.Range(0, cars_avtobuss.Length)], gameObject.transform);
                spawn_ok = true;
            }

            if (car != null) {
                //вытаскиваем длину
                //наХОДИМ все боксы
                BoxCollider[] boxColliders = car.GetComponentsInChildren<BoxCollider>();
                float dlina = 0;
                //Перебираем все боксы и оставляем самый большой размер, это будет длина
                for (int boxNum = 0; boxNum < boxColliders.Length; boxNum++) {
                    if (dlina < boxColliders[boxNum].size.x)
                        dlina = boxColliders[boxNum].size.x;
                    if (dlina < boxColliders[boxNum].size.y)
                        dlina = boxColliders[boxNum].size.y;
                    if (dlina < boxColliders[boxNum].size.z)
                        dlina = boxColliders[boxNum].size.z;
                }

                car.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
                CarStatic carStatic = car.GetComponent<CarStatic>();
                if (carStatic != null) {
                    car.transform.position += car.transform.forward * (dlina / 2 + carStatic.SpawnSdvig);
                }
            }
        }
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

}
