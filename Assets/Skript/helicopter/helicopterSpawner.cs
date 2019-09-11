using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class helicopterSpawner : MonoBehaviour {

    [SerializeField]
    GameObject PrefabHelicopter;

    //вертолет живущий шас
    helicopter helicopterLiveNow;

    //стартовая цель
    [SerializeField]
    helicopterPoint helicopterMainPoint;

    Player player;

    float timeToSpawn = 60;
    bool timeToSpawnGo = false;

	// Use this for initialization
	void Start () {
        setPlayer();

    }
	
	// Update is called once per frame
	void Update () {
        TestSpawn();

    }

    void setPlayer() {
        if(player == null)
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    void TestSpawn() {
        if (player != null && PrefabHelicopter != null && helicopterMainPoint != null) {
            if (player.gameplayParametrs.get_lvl_now() >= 3 && !timeToSpawnGo)
                timeToSpawnGo = true;


            //если сейчас нету живого вертолета
            if (helicopterLiveNow == null || (helicopterLiveNow != null && helicopterLiveNow.GetComponent<PiiTarget>().heath <= 0))
            {
                //Если время запущено
                if (timeToSpawnGo)
                {
                    timeToSpawn -= Time.deltaTime;

                    //и дошло до нуля
                    if (timeToSpawn < 0)
                    {
                        timeToSpawn = 0;

                        //Спавним вертолет
                        helicopterLiveNow = Instantiate<GameObject>(PrefabHelicopter, gameObject.transform).GetComponent<helicopter>();
                        helicopterLiveNow.setNewTarget(helicopterMainPoint.gameObject);
                        helicopterLiveNow.gameObject.transform.position = gameObject.transform.position;

                        //Отсрачиваем время появления следующего
                        if (player.gameplayParametrs.get_lvl_now() == 3)
                        {
                            timeToSpawn = Random.Range(0, 30);
                        }
                        else if (player.gameplayParametrs.get_lvl_now() == 4)
                        {
                            timeToSpawn = Random.Range(0, 60);
                        }
                        else if (player.gameplayParametrs.get_lvl_now() == 5)
                        {
                            timeToSpawn = Random.Range(0, 90);
                        }
                    }
                }
            }
        }
    }
}
