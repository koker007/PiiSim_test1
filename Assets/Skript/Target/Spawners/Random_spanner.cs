using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Random_spanner : MonoBehaviour {

    [SerializeField]
    GameObject[] SpawnObj;
    [SerializeField]
    float SpawnVeroatnost = 50;
    //вращение
    [SerializeField]
    float start_rotate = 0;
    [SerializeField]
    float random_rotate = 0;

	// Use this for initialization
	void Start () {
        DeletCybe();
        CalcSpawnObj();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    //Удалить куб если он есть
    void DeletCybe() {
        //Получаем куб и выключаем
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (meshRenderer)
            meshRenderer.enabled = false;

        
    }

    //Расчет спавна обьекта
    void CalcSpawnObj() {
        //Расчитываем выроятность появления
        float spawn_ver_now = Random.Range(0,100);
        if (spawn_ver_now < SpawnVeroatnost && SpawnObj != null && SpawnObj.Length > 0) {
            //Нужно спавнить
            int numObj = Random.Range(0, SpawnObj.Length);
            //если обьект есть то спавним
            if (SpawnObj[numObj] != null) {
                //Создаем
                GameObject new_obj = Instantiate(SpawnObj[numObj], gameObject.transform);
                //перемещаем
                new_obj.transform.position = gameObject.transform.position;
                //получаем вращательный рандом
                float rand_rot = Random.Range(0, random_rotate);
                //плюс или минус
                if (Random.Range(0, 100) < 50)
                    rand_rot *= -1;

                new_obj.transform.Rotate(0, rand_rot + start_rotate, 0);
            }
        }
    }
}
