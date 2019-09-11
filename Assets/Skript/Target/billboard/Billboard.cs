using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour {

    [SerializeField]
    private GameObject[] positions;

    [SerializeField]
    private GameObject[] advertising_prefabs;

    // Use this for initialization
    void Start () {
        add_adverting_prefabs();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    //Добавить рекламму на позицию
    void add_adverting_prefabs() {
        //Сперва обходим все позиции
        for (int num_pos = 0; num_pos < positions.Length; num_pos++) {
            //Если позиция существует
            if (positions[num_pos] != null) {
                //Выбираем рандомный номер
                int num_adver = Random.Range(0, advertising_prefabs.Length);

                //Если рекламмы под таким номером нет то еще раз
                if (advertising_prefabs[num_adver] == null)
                {
                    num_adver = Random.Range(0, advertising_prefabs.Length);
                }

                //Если рекламма под таким номером есть
                if (advertising_prefabs[num_adver] != null) {
                    //Создаем экземпляр рекламмы
                    GameObject new_adver = Instantiate(advertising_prefabs[num_adver]);

                    //Задаем родителя
                    new_adver.transform.parent = positions[num_pos].transform;
                    //задаем позицию
                    new_adver.transform.position = positions[num_pos].transform.position;
                    //задаем размер
                    new_adver.transform.localScale = new Vector3(
                        new_adver.transform.localScale.x * gameObject.transform.localScale.x * 0.97f,
                        new_adver.transform.localScale.y * gameObject.transform.localScale.y * 0.97f,
                        new_adver.transform.localScale.z * gameObject.transform.localScale.z * 0.97f);

                    //Поворачиваем
                    new_adver.transform.localEulerAngles = new Vector3(0, 0, 0);

                }
            }
        }
    }
}
