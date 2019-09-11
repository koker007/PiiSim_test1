using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TsunamiMain : MonoBehaviour {

    Vector3 startPos;
    float lifeTime = 0;
    float endTime = 0;
    float SpeedMove = 10;

    Player player;
    void iniPlayer() {
        if (player == null) {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }
    }

    GameplayParametrs gameplayParametrs;
    void iniGameParam() {
        if (gameplayParametrs == null) {
            gameplayParametrs = GameObject.FindGameObjectWithTag("setings_game").GetComponent<GameplayParametrs>();
        }
    }

    void iniStart() {
        //если игрок получен и есть параметры игры
        if (player != null && gameplayParametrs != null)
        {
            startPos = transform.position;

            //Расчитываем растояние от цунами до игрока
            float DistToPlayer = Vector3.Distance(transform.position, player.transform.position);
            //получаем скорость до конца уровня
            if (gameplayParametrs.TimeToEnd > 0)
                SpeedMove = DistToPlayer / gameplayParametrs.TimeToEnd;
        }
        //Иначе удаляем обьект
        else {
            Destroy(gameObject);
        }
    }

	// Use this for initialization
	void Start () {
        iniPlayer();
        iniGameParam();

        iniStart();
    }
	
	// Update is called once per frame
	void Update () {
        TestTime();
        TestMove();
    }

    void TestTime()
    {
        lifeTime += Time.deltaTime;

        if (gameplayParametrs != null && (gameplayParametrs.GameOver || gameplayParametrs.get_lvl_now() > 5)) {
            endTime += Time.deltaTime;
        }
    }
    void TestMove() {

        //смещаем вниз
        float maxTimeUP = 10; //через 10 сек максимальная высота
        float downCoof = 0;
        if (lifeTime <= 0)
        {
            downCoof = 1;
        }
        else if (lifeTime < maxTimeUP)
        {
            downCoof = 1 - (lifeTime / maxTimeUP);
        }
        else {
            downCoof = 0;
        }
        //Опускаем цунами
        Vector3 newPos = transform.position;

        //newPos.y = startPos.y - downCoof * 2.50f;
        newPos.y = startPos.y - downCoof * 50;

        float speedCoof = 1;
        if (endTime > 0 && endTime < 60) {
            speedCoof = 1 - (endTime / 60);
        }
        else if (endTime >= 60) {
            speedCoof = 0;
        }
        //двигаемся с указанной скоростью
        //Vector3 newPos = transform.position;
        newPos.z -= speedCoof * SpeedMove * Time.deltaTime;
        transform.position = newPos;
    }
}
