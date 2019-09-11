using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Automobile : MonoBehaviour {

    //Автомобиль спереди
    public Automobile faceAutomobile;

    public int shance_spawn = 100;
    public float speed_max = 3;

    //куда движемся
    public Track_point TrackTo;
    public Player player;

    PiiTarget piiTarget;

    Rigidbody rigidbody;
    float speed = 0;
    float uskor = 0;
    float need_root = 0;

    float turn_next = 0;

    float radius_face_ray = 2;
    float speed_face_ray = 0.5f;

    public float Agress = 0;
    float Beep_timeout = 0;
    [SerializeField]
    beep[] Beep_sound;
    int beep_type = 0;
    AudioSource Beep_audioSource;

    float player_distance_old = 999999;

    //Фары
    [SerializeField]
    ParticleSystem left_face;
    [SerializeField]
    ParticleSystem right_face;
    [SerializeField]
    ParticleSystem left_botm;
    [SerializeField]
    ParticleSystem right_botm;

    //Калеса
    [SerializeField]
    GameObject wheel_left_face;
    [SerializeField]
    GameObject wheel_right_face;
    [SerializeField]
    GameObject wheel_left_botm;
    [SerializeField]
    GameObject wheel_right_botm;

    Quaternion wheel_left_root_local;
    Quaternion wheel_righ_root_local;

    police_car police;
    public bool police_stop = false;
    float police_stop_time_all = 0;
    float police_stop_time_damage = 0;

    GameplayParametrs gameParam;
    void iniGameParam() {
        if (gameParam == null) {
            gameParam = GameObject.FindGameObjectWithTag("setings_game").GetComponent<GameplayParametrs>();
        }
    }
    setings seting;
    void iniSeting() {
        if (seting == null) {
            seting = GameObject.FindGameObjectWithTag("setings_game").GetComponent<setings>();
        }
    }

    // Use this for initialization
    void Start() {
        iniGameParam();
        iniSeting();

        rigidbody = GetComponent<Rigidbody>();
        if(TrackTo != null)
            gameObject.transform.LookAt(TrackTo.transform);
        piiTarget = gameObject.GetComponent<PiiTarget>();

        //Останавливаем фары
        if (left_face != null)
            left_face.Stop();
        if (right_face != null)
            right_face.Stop();
        if (left_botm != null)
            left_botm.Stop();
        if (right_botm != null)
            right_botm.Stop();
        speed_face_ray = Random.Range(0.8f, 1.5f);

        police = gameObject.GetComponent<police_car>();

        //Агрессия автомобиля
        Agress = Random.Range(20f, 80f);
        //Создаем источник звука под клаксон
        Create_BeepAudioSource();
    }

    // Update is called once per frame
    void Update() {
        police_stop = false;

        TestMove();
        TestTrackNext();
        TestTurnLight();
        TestDamadePlayer();

        TestDelite();
    }

    void TestRotate() {
        //Частично поворачиваем
        //gameObject.transform.rotation = Quaternion.Lerp(rotate_now, rotate_need, Time.deltaTime);
        if (rigidbody != null && TrackTo != null) {
            Quaternion rotate_to = Quaternion.LookRotation(TrackTo.transform.position - transform.position);
            float old_y = transform.rotation.eulerAngles.y;
            transform.rotation = Quaternion.Lerp(transform.rotation, rotate_to, rigidbody.velocity.magnitude * 0.2f * Time.deltaTime);
            need_root += (old_y - transform.rotation.eulerAngles.y) * -15;
            need_root /= 2;
        }

    }
    void TestSpeed() {
        //Узнаем скорость
        if (rigidbody != null && TrackTo != null) {

            //останавливаем полицию если запущена сирена и сейчас в зоне игрока
            police_stop = false;
            if (police != null && player != null && player.gameplayParametrs.get_lvl_now() < 5) {
                //Если запущена сирена и игрок близко
                float player_distance = Vector3.Distance(gameObject.transform.position, player.transform.position);
                if (police.police_active_now && player_distance < 23 && piiTarget.heath > 0)
                {
                    police.siren_short_need_play = true;
                    if (player_distance > player_distance_old)
                    {
                        //Debug.Log("player_distance " + player_distance);
                        police_stop = true;
                    }
                    else
                        player_distance_old = player_distance;
                }
                else {
                    police.siren_short_need_play = false;
                }
            }
            //Если скорость довольно маленькая
            //уменьшаем радиус
            if (rigidbody.velocity.magnitude < 0.02f)
            {
                radius_face_ray -= Time.deltaTime * speed_face_ray;
            }
            else {
                radius_face_ray = 2;
            }

            Tank faceTank = null;
            //Проверка на препятствие впереди
            Ray face = new Ray(transform.position, transform.forward);
            RaycastHit faceHit;
            bool face_triger = false;
            if (Physics.SphereCast(face, radius_face_ray, out faceHit))
            {
                faceAutomobile = faceHit.collider.gameObject.GetComponent<Automobile>();
                faceTank = faceHit.collider.gameObject.GetComponent<Tank>();
                if (faceHit.distance < (3 + rigidbody.velocity.magnitude * 0.7f) && faceAutomobile != null && faceAutomobile.gameObject.GetComponent<PiiTarget>() != null && faceAutomobile.gameObject.gameObject.GetComponent<PiiTarget>().heath > 0)
                {
                    //Попытка посигналить
                    TestBeep_NOW();
                    face_triger = true;
                }
            }
            else {
                faceAutomobile = null;
            }
            
            //Прибавление скорости полиции
            float max_speed = TrackTo.max_speed;
            if (police != null && player != null && player.gameplayParametrs.get_lvl_now() > 2 && player.gameplayParametrs.get_lvl_now() < 5) {
                max_speed *= 1.3f;
                if (police.police_active_now && max_speed == 0) {
                    max_speed = 2;
                }
            }
            else if (player.gameplayParametrs.get_lvl_now() >= 5) {
                max_speed *= 2f;
                if (max_speed > 30) {
                    max_speed = 30;
                }
            }

            //Выключение сирены если спереди другая полиция с включеной сиреной
            if (faceAutomobile != null && police != null && faceAutomobile.police != null && faceAutomobile.piiTarget.heath > 0 && faceAutomobile.police.need_sound_yn) {
                police.pause_play_siren = 1;
            }

            int revers = 1;
            //Проверка столкновения лоб в лоб
            //Если автомобиль который я вижу, также видит и меня, движение начинает тот кто более агресивен
            if (faceAutomobile != null && faceAutomobile.faceAutomobile == gameObject.GetComponent<Automobile>() && Agress > faceAutomobile.Agress ||
                //Если это полиция а спереди не полиция
                faceAutomobile != null && police != null && police.police_active_now && faceAutomobile.police == null) {
                face_triger = false;
            }
            //иначе если я не коп а спереди коп который смотрит на меня или у машины с переди больше агрессии или спереди танк
            else if (police == null && faceAutomobile != null && faceAutomobile.police != null && faceAutomobile.faceAutomobile == gameObject.GetComponent<Automobile>() ||
                faceAutomobile != null && faceAutomobile.faceAutomobile == gameObject.GetComponent<Automobile>() && Agress <= faceAutomobile.Agress ||
                police == null && faceAutomobile != null && faceAutomobile.police_stop ||
                faceTank != null && faceHit.distance < (3 + rigidbody.velocity.magnitude * 0.7f)
                ) {
                revers = -1;
                face_triger = false;
                if (max_speed == 0) {
                    max_speed = 2;
                }
            }
            
            //Прибавляем скорость если она маленькая
            if (rigidbody.velocity.magnitude < max_speed && !face_triger && !police_stop)
            {
                rigidbody.velocity = rigidbody.velocity + gameObject.transform.forward * 15f * Time.deltaTime * revers;
                if (police != null)
                    police.siren_short_need_play = false;
            }
            //Если спереди автомобиль и это полиция
            if (face_triger && police != null) {
                police.siren_short_need_play = true;
            }
        }
    }

    void TestMove() {
        //проверка угла наклона
        Vector3 naclon = gameObject.transform.rotation.eulerAngles;
        bool canMove = false;
        if ((naclon.x < 20 || naclon.x > 340) && (naclon.z < 20 || naclon.z > 340))
            canMove = true;

        if (piiTarget != null && piiTarget.heath > 0 && canMove)
        {
            TestRotate();
            TestSpeed();
            TestRotateWheel();
        }

        if (!canMove && piiTarget != null && piiTarget.heath > 0)
        {
            piiTarget.heath -= Time.deltaTime * 20;
            if (piiTarget.heath <= 0) {
                piiTarget.heath = 0;
                piiTarget.DeathTarget();
            }
        }


    }

    //проверка следующего пути 
    void TestTrackNext() {
        if (TrackTo != null)
        {
            //Если достаточно близко к настоящей точке
            float distanceToTrack = Vector3.Distance(gameObject.transform.position, TrackTo.transform.position);
            if (distanceToTrack < 5)
            {
                               
                //Переключаемся на следуюшую если есть
                if (TrackTo.point_next_normal != null || TrackTo.point_next_left != null || TrackTo.point_next_right != null)
                {
                    float player_turn = 0;
                    //Если есть игрок считаем желание
                    if (player != null && player.gameplayParametrs.get_lvl_now() > 2) {
                        //Узнаем напор игрока
                        float speed_pii = player.bolt.GetComponent<PiiController>().speed_pii;
                        speed_pii -=12;
                        if (speed_pii < 0)
                            speed_pii = 0;

                        //Узнаем требуемый поворот чтобы смотреть на игрока
                        Quaternion rot_player = Quaternion.LookRotation(gameObject.transform.position, player.transform.position);

                        float raznica = gameObject.transform.eulerAngles.y - rot_player.eulerAngles.y;
                        int plus_1 = 1;
                        if (gameObject.transform.eulerAngles.y < 180) {
                            plus_1 *= -1;
                        }
                        //Смеряем с 
                        if (raznica > 0) {
                            player_turn = 110 * plus_1 * (speed_pii + 10);
                        }
                        else{
                            player_turn = -110 * plus_1 * (speed_pii + 10);
                        }

                        if (police != null)
                            player_turn = player_turn * -20; //-15 old
                    }
                    turn_next = turn_next + player_turn;

                    //Если есть поворот и желание повернуть
                    //налево
                    if (turn_next < -30 && TrackTo.point_next_left != null)
                    {
                        TrackTo = TrackTo.point_next_left;
                    }
                    else if (turn_next > 30 && TrackTo.point_next_right != null)
                    {
                        TrackTo = TrackTo.point_next_right;
                    }
                    else if (turn_next >= -30 && turn_next <= 30 && TrackTo.point_next_normal != null)
                    {
                        TrackTo = TrackTo.point_next_normal;
                    }

                    //Если желание не совпадает
                    else if (TrackTo.point_next_normal != null)
                    {
                        TrackTo = TrackTo.point_next_normal;
                    }
                    else if (TrackTo.point_next_left != null)
                    {
                        TrackTo = TrackTo.point_next_left;
                    }
                    else if (TrackTo.point_next_right != null)
                    {
                        TrackTo = TrackTo.point_next_right;
                    }

                    //Тестим желание
                    turn_next = Random.Range(-100, 100);
                }
                //Если точек нету то удаляем
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    void Create_BeepAudioSource() {
        if (Beep_audioSource == null && Beep_sound != null && Beep_sound.Length > 0) {
            //Выбираем тип сигнала из предлагаемых
            beep_type = Random.Range(0, Beep_sound.Length);

            //Создаем источник звука под клаксон
            Beep_audioSource = gameObject.AddComponent<AudioSource>();
            //теперь регулируем громкость и прочее
            Beep_audioSource.minDistance = 3;
            Beep_audioSource.maxDistance = 10000;

            Beep_audioSource.spatialBlend = 1;


        }
    }

    void TestBeep_NOW() {
        if (Beep_audioSource != null &&
           !Beep_audioSource.isPlaying &&
           seting != null &&
           Beep_sound != null && 
           Beep_sound[beep_type] != null && 
           Beep_sound[beep_type].signal_auto.Length > 0 && 
           Beep_timeout <= 0
           )
        {

            //выбираем сигнал и сигналим
            int num_beep = Random.Range(0, Beep_sound[beep_type].signal_auto.Length);
            if (Beep_sound[beep_type].signal_auto[num_beep] != null)
            {
                //Нужно ли вообще сигналить?
                float need_beep = Random.Range(0f, 100f);

                Beep_timeout = Random.Range(3f, 7f);
                if (need_beep > Agress && player != null && player.gameplayParametrs.get_lvl_now() > 1)
                {
                    Beep_audioSource.volume = seting.game.volume_all * seting.game.volume_sound * 1f;
                    if (Beep_audioSource.volume > 1) {
                        Beep_audioSource.volume = 1;
                    }
                    Beep_audioSource.minDistance = 10;
                    Beep_audioSource.maxDistance = 1500;
                    Beep_audioSource.PlayOneShot(Beep_sound[beep_type].signal_auto[num_beep]);
                }
            }
            else
            {
                Beep_timeout = 0;
            }
        }
    }
    void TestTurnLight() {
        //Если машина хочет повернуть
        if (turn_next < -35 && TrackTo.point_next_left != null && left_botm != null && left_face != null && !left_face.isPlaying) {
            left_face.Play();
            left_botm.Play();
        }
        if (turn_next > 35 && TrackTo.point_next_right != null && right_botm != null && right_face != null && !right_face.isPlaying) {
            right_face.Play();
            right_botm.Play();
        }
    }

    //Попытка вращения колес
    void TestRotateWheel() {
        if (wheel_left_face != null && wheel_right_face != null && wheel_left_botm != null && wheel_right_botm != null && rigidbody != null) {
            //вытаскиваем повороты без влево-вправо
            if (wheel_left_root_local != null) {
                wheel_left_face.transform.localRotation = wheel_left_root_local;
            }
            if (wheel_righ_root_local != null) {
                wheel_right_face.transform.localRotation = wheel_righ_root_local;
            }


            //узнаем скорость и вращаем все обьекты
            float rotate_need_speed = rigidbody.velocity.magnitude * 3;
            wheel_left_face.transform.Rotate(rotate_need_speed, 0, 0);
            wheel_right_face.transform.Rotate(rotate_need_speed, 0, 0);
            wheel_left_botm.transform.Rotate(rotate_need_speed, 0, 0);
            wheel_right_botm.transform.Rotate(rotate_need_speed, 0, 0);

            //Вращанули теперь запоминаем
            wheel_left_root_local = wheel_left_face.transform.localRotation;
            wheel_righ_root_local = wheel_right_face.transform.localRotation;

            //теперь поворот колес в сторону поворота машины
            Quaternion locRotLeft = wheel_left_face.transform.localRotation;
            Vector3 eulerLeft = locRotLeft.eulerAngles;
            eulerLeft.y = eulerLeft.y + need_root;
            locRotLeft.eulerAngles = eulerLeft;
            wheel_left_face.transform.localRotation = locRotLeft;

            Quaternion locRotRigh = wheel_right_face.transform.localRotation;
            Vector3 eulerRigh = locRotRigh.eulerAngles;
            eulerRigh.y = eulerRigh.y + need_root;
            locRotRigh.eulerAngles = eulerRigh;
            wheel_left_face.transform.localRotation = locRotRigh;
        }
    }

    void TestDamadePlayer() {
        //Если работает тригер полицейской остановки
        if (police_stop && player != null && piiTarget.heath > 0)
        {

            //Прибавляем обшее время остановки
            police_stop_time_all += Time.deltaTime;
            police_stop_time_damage += Time.deltaTime;

            //Если пришло время для урона
            if (police_stop_time_damage > 0.25f)
            {
                police_stop_time_damage = 0;

                //Уменьшаем напор
                if (player.ScoreTime > 0)
                {
                    float damage = 10 * police_stop_time_all;
                    player.ScoreTime -= damage;
                    if (player.ScoreTime < 0)
                    {
                        player.ScoreTime = 0;
                    }

                    //расчет для отображения урона в индикаторе игрока
                    //Текущий поворот камеры игрока
                    Quaternion now_rotate_cam = player.camera.transform.rotation;
                    //Требуемый поворот камеры игрока для того чтобы смотреть прямо в центр
                    Quaternion need_rotate_cam = Quaternion.FromToRotation(player.camera.transform.position, gameObject.transform.position);

                    //Разница по горизонтали
                    float need_rot_y = need_rotate_cam.ToEulerAngles().y - now_rotate_cam.ToEulerAngles().y;
                    //разница по вертикали
                    float need_rot_x = need_rotate_cam.ToEulerAngles().x - now_rotate_cam.ToEulerAngles().x;

                    //Запихать в урон
                    if (need_rot_x < 0)
                        player.damageWindow.set_all_need(0, 0, need_rot_x * -1f * police_stop_time_all, 0);
                    else player.damageWindow.set_all_need(0, 0, 0, need_rot_x * 1f * police_stop_time_all);

                    if (need_rot_y < 0)
                        player.damageWindow.set_all_need(need_rot_y * -1f * police_stop_time_all, 0, 0, 0);
                    else player.damageWindow.set_all_need(0, need_rot_y * 1f * police_stop_time_all, 0, 0);

                }
            }

        }
    }

    void TestDelite() {
        if (piiTarget != null && piiTarget.heath > 0 && gameParam != null && gameParam.get_lvl_now() > 5) {
            Destroy(gameObject);
        }
    }
}
