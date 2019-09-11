using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Auto_spawner : MonoBehaviour {

    //префабы автомобилей
    [SerializeField]
    Automobile[] autos;

    //Точки для начала движения
    [SerializeField]
    Track_point[] track_Points;
    [SerializeField]
    Player player;

    //последний заспавненый автомобиль
    GameObject LastSpawner;
    float time_lastspawn = 0;
    [SerializeField]
    float timeout_spawn_mnozitel = 1;
    float timeout_spawn = 2;
    float timeout_spawn_next = 0;
    [SerializeField]
    bool polise_need_siren = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        TestSpawn();
    }

    //тест спавна
    void TestSpawn() {
        time_lastspawn += Time.deltaTime;

        //Проверка есть ли что спавнить и есть ли старт пути?
        if (autos != null && autos.Length > 0 && track_Points != null && track_Points.Length > 0) {
            //На каком растоянии от спавна последний заспавненный автомобиль
            float distance_last_spawn_auto = 999999;
            if (LastSpawner != null) {
                distance_last_spawn_auto = Vector3.Distance(gameObject.transform.position, LastSpawner.transform.position);
            }

            //Расчет максимального количества автомобилей сейчас
            if (player.gameplayParametrs.get_lvl_now() == 0) {
                timeout_spawn = 20 * timeout_spawn_mnozitel;
            }
            else if (player.gameplayParametrs.get_lvl_now() == 1)
                timeout_spawn = 4 * timeout_spawn_mnozitel;
            else if (player.gameplayParametrs.get_lvl_now() == 2)
                timeout_spawn = 4 * timeout_spawn_mnozitel;

            else if (player.gameplayParametrs.get_lvl_now() == 3)
                timeout_spawn = 4 * timeout_spawn_mnozitel;

            else if (player.gameplayParametrs.get_lvl_now() == 4)
                timeout_spawn = 20 * timeout_spawn_mnozitel;

            else if (player.gameplayParametrs.get_lvl_now() == 5)
                timeout_spawn = 50 * timeout_spawn_mnozitel;

            else if (player.gameplayParametrs.get_lvl_now() > 5)
                timeout_spawn = 99999 * timeout_spawn_mnozitel;
            else {
                timeout_spawn = 4 * timeout_spawn_mnozitel;
            }

            //Проверяем пришло ли время для спавна
            if (time_lastspawn > timeout_spawn_next && distance_last_spawn_auto > 10) {

                //Выбираем автомобиль
                int num_auto = Random.Range(0, autos.Length);
                if (autos[num_auto] != null) {

                    bool its_police = false;
                    //Проверяем если есть игрок то узнаем какой уровень и его мошьность
                    float itog_shanse = autos[num_auto].shance_spawn;
                    float player_shance = 1;
                    if (autos[num_auto].GetComponent<police_car>() != null)
                    {
                        its_police = true;
                    }

                    if (player != null)
                    {
                        //Изменения шанса пояления этого авто
                        PiiController piiController = player.bolt.GetComponent<PiiController>();
                        player_shance = piiController.speed_pii - 12;
                        if (player_shance < 0) player_shance = 0;
                        player_shance *= 8;
                    }
                    

                    //проверяем удачу появления этого автомобиля
                    float shance = Random.Range(0f, 100f);

                    if (player_shance != 0)
                    {
                        if (its_police)
                        {
                            //Увеличиваем шанс
                            itog_shanse = itog_shanse * player_shance;
                        }
                        else
                        {
                            //Уменьшаем шанс
                            itog_shanse = itog_shanse / player_shance;
                        }
                    }


                    if (shance < itog_shanse) {
                        //спавним
                        LastSpawner = Instantiate(autos[num_auto].gameObject, gameObject.transform);
                        //Перемещаем на место спавна
                        LastSpawner.transform.position = gameObject.transform.position;
                        
                        //Заспавнено.. Теперь даем рандомный маршрут из списка
                        int num_track = Random.Range(0, track_Points.Length);
                        Automobile automobile = LastSpawner.GetComponent<Automobile>();
                        automobile.TrackTo = track_Points[num_track];

                        time_lastspawn = 0;
                        timeout_spawn_next = Random.Range(1.1f,  timeout_spawn);

                        //Добавляем в память игрока если он есть
                        if (player != null) {
                            automobile.player = player;
                        }
                        //Если это полиция включаем сирену если надо
                        if (its_police && player != null && player.gameplayParametrs.get_lvl_now() > 2) {
                            LastSpawner.GetComponent<police_car>().police_active_now = true;
                            LastSpawner.GetComponent<police_car>().need_sound_yn = polise_need_siren;
                        }
                    }
                }
            }
        }
    }

}
