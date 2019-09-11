using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawner_flyer : MonoBehaviour {

    [SerializeField]
    GameObject PrefabFlyer;

    [SerializeField]
    float timeToSpawn = 0;
    [SerializeField]
    bool randomStartToSpawn = false;
    [SerializeField]
    float randomTimeMin = 45;
    [SerializeField]
    float randomTimeMax = 60;

    [SerializeField]
    int lvl_start_spawn;
    [SerializeField]
    int lvl_end_spawn;

    GameplayParametrs gameplayParametrs;


    void iniStartTime() {
        if (randomStartToSpawn) {
            timeToSpawn = Random.Range(randomTimeMin, randomTimeMax);
        }
    }
    void iniGameplayParam() {
        if (gameplayParametrs == null) {
            gameplayParametrs = GameObject.FindGameObjectWithTag("setings_game").GetComponent<GameplayParametrs>();
        }
    }
    void offMeshRender() {
        GetComponent<MeshRenderer>().enabled = false;
    }

	// Use this for initialization
	void Start () {
        iniStartTime();
        iniGameplayParam();
        offMeshRender();
    }
	
	// Update is called once per frame
	void Update () {
        testSpawn();
    }

    void testSpawn() {
        //Сперва проверяем есть ли вообще модель
        if (PrefabFlyer != null && gameplayParametrs != null && gameplayParametrs.get_lvl_now() >= lvl_start_spawn && gameplayParametrs.get_lvl_now() < lvl_end_spawn ) {
            //Расчитываем пришло ли время или нет
            timeToSpawn -= Time.deltaTime;
            if (timeToSpawn <= 0) {
                timeToSpawn = Random.Range(randomTimeMin, randomTimeMax);

                GameObject flyer = Instantiate(PrefabFlyer);
                flyer.transform.position = transform.position;
                flyer.transform.rotation = transform.rotation;
            }
        }
    }
}
