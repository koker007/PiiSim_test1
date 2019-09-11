using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerTank : MonoBehaviour {

    [SerializeField]
    Tank LastSpawnTank;

    [SerializeField]
    Track_point StartPoint;

    [SerializeField]
    Tank PrefabTank;

    float timeToSpawnNow = 0;

    Player player;
    GameplayParametrs gameplayParametrs;

    void iniGameParam() {
        if (gameplayParametrs == null) {
            gameplayParametrs = GameObject.FindGameObjectWithTag("setings_game").GetComponent<GameplayParametrs>();
        }
    }
    void iniPlayer() {
        if (player == null) {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }
    }

	// Use this for initialization
	void Start () {
        iniGameParam();
        iniPlayer();
    }
	
	// Update is called once per frame
	void Update () {
        TestSpawn();
    }

    int numTanksSpawn = 0;
    void TestSpawn() {
        timeToSpawnNow -= Time.deltaTime;
        if (timeToSpawnNow < 0)
            timeToSpawnNow = 0;

        if (gameplayParametrs != null && gameplayParametrs.get_lvl_now() >= 4 && timeToSpawnNow == 0) {
            timeToSpawnNow = Random.Range(20, 30);

            float distLastSpawn = 999;
            if (LastSpawnTank != null) {
                distLastSpawn = Vector3.Distance(LastSpawnTank.gameObject.transform.position, gameObject.transform.position);
            }

            if (distLastSpawn > 15 && numTanksSpawn < 6) {
                //+1 танк
                numTanksSpawn++;

                LastSpawnTank = Instantiate(PrefabTank);
                LastSpawnTank.transform.position = gameObject.transform.position;

                if (StartPoint != null) {
                    LastSpawnTank.target_track_point = StartPoint;
                    LastSpawnTank.transform.rotation.SetLookRotation(StartPoint.transform.position);
                }
                else{
                    LastSpawnTank.transform.rotation.SetLookRotation(player.transform.position);
                }
            }
        }
    }
}
