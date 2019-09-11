using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NukeBoom : MonoBehaviour {

    [SerializeField]
    float timeToBoom = 0;
    bool timePlay = false;

    [SerializeField]
    int Lvl_boom = 0;

    [SerializeField]
    GameObject TsunamiObj;
    [SerializeField]
    float TsunamiStart = -3;

    [SerializeField]
    AudioClip[] EarthShake;
    AudioSource EarthShakeSourse;
    bool EarthShakePlay = false;

    [SerializeField]
    AudioClip[] BigBoom;
    AudioSource BigBoomSourse;
    bool BigBoomPlay = false;

    GameplayParametrs gameplayParametrs;
    void iniGameParam() {
        if (gameplayParametrs == null) {
            gameplayParametrs = GameObject.FindGameObjectWithTag("setings_game").GetComponent<GameplayParametrs>();
        }
    }

    ParticlePlayer particleBoom;
    void iniParticles() {
        if (particleBoom == null) {
            particleBoom = GetComponent<ParticlePlayer>();
        }
    }

    setings seting;
    void iniSeting() {
        if (seting == null) {
            seting = GameObject.FindGameObjectWithTag("setings_game").GetComponent<setings>();
        }
    }

    bool playerHit = false;
    Player player;
    void iniPlayer() {
        if (player == null) {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }
    }
    void iniAudio() {
        if (EarthShakeSourse == null) {
            EarthShakeSourse = gameObject.AddComponent<AudioSource>();
        }
        if (BigBoomSourse == null) {
            BigBoomSourse = gameObject.AddComponent<AudioSource>();
        }
    }

    //Класс получающий все автомобили
    class SirenGeter {

        Vector3 positionBoom;
        bool testEnd = false;
        int maxTesting = 50;

        //колайдеры
        bool testColiderEnd = false;    //завершен ли тест колайдеров
        int numColiderTest = 0;         //Колайдер проверяемый последним
        Collider[] collider;            //Все колайдеры
        int SumAutofound = 0;

        //Найдены ли все автомобили или нет
        bool CarFound = false;
        int numCarTest = 0;
        CarStatic[] carStatics;

        //сортировка
        bool CarSort = false;
        int numSort = 0; //обьект сортировки в настоящий момент

        //воспроизведение
        public bool Boom = false; //Взрываем ли сейчас
        const float speedBoom = 260; //Скорость распространения взрыва
        float timeBoomNow = 0; //Сколько времени прошло с начала взрыва
        int numCarBoom = 0; //обрабатываемая машина в данный момент

        public SirenGeter(Transform transform) {
            collider = Physics.OverlapSphere(transform.position, 2000);
            positionBoom = transform.position;
        }

        //Поиск целей и их сортировка
        void testCar() {
            //Если тест не завешен то выполняем
            if (!testEnd && collider != null && Time.deltaTime < 0.2f) {
                //вытаскиваем колайдеры если их нет
                if (collider == null || collider.Length <= 0) {
                    collider = Physics.OverlapSphere(positionBoom, 9000);
                }

                //Если колайдеры не прошли тест
                else if (!testColiderEnd) {
                    //Проверяем колайдеры на автомобиль
                    for (int num = 0; numColiderTest < collider.Length && num < maxTesting; numColiderTest++, num++) {
                        //Вытаскиваем из колайдера автомобиль
                        CarStatic carStaticNow = null;
                        if (collider[numColiderTest] != null) {
                            carStaticNow = collider[numColiderTest].GetComponent<CarStatic>();
                        }

                        if (carStaticNow != null)
                            SumAutofound++;

                        //Если это последняя проверка то завершаем
                        if (collider.Length == numColiderTest + 1) {
                            testColiderEnd = true;
                        }
                    }
                }


                //Если все автомобили не были найдены
                else if (!CarFound) {
                    //Если еще не создан массив
                    if (carStatics == null)
                    {
                        //Создаем массив
                        carStatics = new CarStatic[SumAutofound];
                        numColiderTest = 0;
                        numCarTest = 0;
                    }
                    else {
                        //Снова перебираем массив колайдера но на этот раз запоминаем автомобиль
                        for (int num = 0; numColiderTest < collider.Length && num < maxTesting; numColiderTest++, num++)
                        {
                            //Вытаскиваем из колайдера автомобиль
                            CarStatic carStaticNow = null;
                            if (collider[numColiderTest] != null) {
                                carStaticNow = collider[numColiderTest].GetComponent<CarStatic>();
                            }

                            if (carStaticNow != null) {
                                //вытаскиваем автомобиль
                                carStatics[numCarTest] = carStaticNow;
                                numCarTest++;
                            }


                            //Если это последняя проверка то завершаем
                            if (collider.Length == numColiderTest + 1 || carStatics.Length == numCarTest + 1)
                            {
                                CarFound = true;
                            }
                        }
                    }
                }
                //Если нужно отсортировать
                else if (!CarSort) {

                    CarStatic carStaticTime;

                    //продолжаем сортировку со старого места
                    for (int num = 0; numSort < carStatics.Length && num < maxTesting; num++, numSort++) {
                        float DistNow = 999999;
                        if (carStatics[numSort] != null) {
                           DistNow = Vector3.Distance(positionBoom, carStatics[numSort].gameObject.transform.position);
                        }

                        //начинаем сравнивать настоящий номер с остальными
                        for (int numTest = numSort; numTest < carStatics.Length; numTest++) {
                            //Получаем растояние до обьектов
                            float DistTest = 999999;
                            if (carStatics[numTest] != null)
                            {
                                Vector3.Distance(positionBoom, carStatics[numTest].gameObject.transform.position);
                            }

                            //Если сравниваемый номер меньше сравниваемого настоящего
                            if (DistTest < DistNow) {
                                //Значит меняем их местами
                                //Запоминаем настоящий
                                carStaticTime = carStatics[numSort];
                                //Ставим тестируемый на место настоящего
                                carStatics[numSort] = carStatics[numTest];
                                //Ставим запомненый на место тестируемого
                                carStatics[numTest] = carStaticTime;

                                //Замена выполнена
                            }
                        }
                    }
                    //Говорим что сортировка завершена
                    CarSort = true;
                }
                //Если все тесты пыполнены а это не указано то завершаеим
                else if (!testEnd) {
                    testEnd = true;
                }
            }
        }

        void testPlaySiren() {
            //Если нужно взрывать
            if (Boom && testEnd && numCarBoom < carStatics.Length) {
                //Cчитаем время
                timeBoomNow += Time.deltaTime;

                float distBoom = timeBoomNow * speedBoom;
                //Проверяем есть ли вообще еще машина
                if (carStatics[numCarBoom] != null)
                {
                    float distCar = Vector3.Distance(positionBoom, carStatics[numCarBoom].transform.position);
                    if (distCar < distBoom)
                    {
                        carStatics[numCarBoom].StartSignal();
                        numCarBoom++;
                    }
                }
                else {
                    numCarBoom++;
                }
            }
        }

        public void Update() {
            testCar();
            testPlaySiren();
        }
    }
    SirenGeter sirenGeter;

	// Use this for initialization
	void Start () {
        iniGameParam();
        iniPlayer();
        iniParticles();
        sirenGeter = new SirenGeter(transform);
        iniSeting();
        iniAudio();
    }
	
	// Update is called once per frame
	void Update () {
        testBoom();
        testTime();
        testSound();
        sirenGeter.Update();
    }

    void testBoom() {
        if (!timePlay && particleBoom != null && gameplayParametrs != null && gameplayParametrs.get_lvl_now() == Lvl_boom) {
            timePlay = true;
        }

        //Нанести урон игроку
        if (!playerHit && timePlay && player != null) {
            float DistBoom = timeToBoom * -260;
            if (DistBoom > 1000) {
                playerHit = true;
                player.damageWindow.set_all_need(1,1,1,1);
                //player.ScoreTime -= 1500;
                player.ScoreTime -= player.ScoreTime / 4;
            }
        }

    }
    void testSound() {
        if (timePlay) {
            float DistBoom = timeToBoom * -260;
            if (!EarthShakePlay && EarthShake != null && EarthShakeSourse != null && seting != null && EarthShake.Length > 0) {
                EarthShakePlay = true;
                EarthShakeSourse.volume = 0.5f * seting.game.volume_all * seting.game.volume_sound;
                EarthShakeSourse.priority = 25;
                EarthShakeSourse.PlayOneShot(EarthShake[Random.Range(0, EarthShake.Length)]);
            }
            if (!BigBoomPlay && BigBoom != null && BigBoomSourse != null && seting != null && BigBoom.Length > 0 && DistBoom > 1000) {
                BigBoomPlay = true;
                BigBoomSourse.volume = seting.game.volume_all * seting.game.volume_sound;
                BigBoomSourse.priority = 20;
                BigBoomSourse.spatialBlend = 0.3f;
                BigBoomSourse.minDistance = 300;
                BigBoomSourse.maxDistance = 10000;
                BigBoomSourse.PlayOneShot(BigBoom[Random.Range(0, BigBoom.Length)]);
            }
        }
    }

    void testTime() {
        if (timePlay) {
            timeToBoom -= Time.deltaTime;
            if (timeToBoom < 0 && !particleBoom.play) {
                particleBoom.play = true;
                sirenGeter.Boom = true;
            }

            if (timeToBoom < TsunamiStart && TsunamiObj != null && !TsunamiObj.active) {
                TsunamiObj.active = true;
            }
        }
    }
    
}
