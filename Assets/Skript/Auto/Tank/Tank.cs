using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : MonoBehaviour {

    bool broke = false;

    public Track_point target_track_point;
    float OtclonenieGradus = 0;
    float DistanceAgress = 0;
    float DistanceNoTrace = 0;
    float DistanceNoTraceStop = 0;
    float ReloadTime = 0;
    float ReloadTimeNow = 0;
    float Agress = 0;

    Player player;
    Rigidbody rigidbody;
    PiiTarget piiTarget;
    setings seting;
    GameplayParametrs gameParam;
    AudioSource audioSourceFire;
    AudioSource audioSourceEngine;

    float rotateNow = 0;

    //Цель 
    public PiiTarget faceTarget;
    float radius_face_ray = 2;
    float speed_face_ray = 0.5f;

    float TimeDistNorm = 0;
    float TimeDistLeft = 0;
    float TimeDistRigh = 0;

    [SerializeField]
    GameObject rayLeft;
    PiiTarget faceTargetLeft;

    [SerializeField]
    GameObject rayRight;
    PiiTarget faceTargetRight;

    [SerializeField]
    GameObject DestroyTriger;

    //Башня танка
    [SerializeField]
    GameObject Tower;

    //Дуло
    [SerializeField]
    GameObject Barrel;

    [SerializeField]
    GameObject ShellOutputPosition;

    //Пуля танка
    [SerializeField]
    GameObject BulletPrefab;

    //партиклс выстрела у дула
    [SerializeField]
    GameObject fireBallerParticlePrefab;
    GameObject fireBallerParticle;

    [SerializeField]
    AudioClip[] FireBarellAudio;

    //левая гусеница
    [SerializeField]
    GameObject LeftTrack;

    //Правая гусеница
    [SerializeField]
    GameObject RightTrack;

    //левые колеса
    [SerializeField]
    GameObject[] LeftWheel;

    //Правые колеса
    [SerializeField]
    GameObject[] RightWhel;

    [SerializeField]
    GameObject ParticleTankBoom;

    //Рандомизация параметров
    void iniRandomParametrs() {
        //Дистанция с которой танк начинает стрелять
        DistanceAgress = Random.Range(30, 100);
        //Дистанция с которой танк сходит с трассы
        DistanceNoTrace = Random.Range(70, 200);
        DistanceNoTraceStop = Random.Range(20, 50);
        //Улг относительно игрока по которому ездит танк сьехав с трассы
        OtclonenieGradus = Random.Range(-15, 15);
        //Время перезарядки танка после стрельбы
        ReloadTime = Random.Range(5, 10);
    }
    void iniPlayer() {
        if (player == null) {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }
    }
    void iniRigidbody() {
        if (rigidbody == null) {
            rigidbody = gameObject.GetComponent<Rigidbody>();
        }
    }
    void iniPiiTarget() {
        if (piiTarget == null) {
            piiTarget = gameObject.GetComponent<PiiTarget>();
        }
    }
    void iniSetings() {
        if (seting == null || gameParam  == null) {
            seting = GameObject.FindGameObjectWithTag("setings_game").GetComponent<setings>();
            gameParam = GameObject.FindGameObjectWithTag("setings_game").GetComponent<GameplayParametrs>();
        }
    }

    void iniAudio() {
        if (audioSourceFire == null && seting != null) {
            audioSourceFire = gameObject.AddComponent<AudioSource>();

            audioSourceFire.priority = 30;
            audioSourceFire.spatialBlend = 0.8f;

            audioSourceFire.minDistance = 5;
            audioSourceFire.maxDistance = 5000;
        }
        if (audioSourceEngine == null && seting == null) {
            audioSourceEngine = gameObject.AddComponent<AudioSource>();

            audioSourceEngine.priority = 50;
            audioSourceEngine.spatialBlend = 0.8f;

            audioSourceEngine.minDistance = 5;
            audioSourceEngine.maxDistance = 200;
        }
    }

    // Use this for initialization
    void Start () {
        iniRandomParametrs();
        iniPlayer();
        iniRigidbody();
        iniPiiTarget();
        iniSetings();
        iniAudio();
    }
	
	// Update is called once per frame
	void Update () {
        //Все перемещения если танк вообще живой
        if (piiTarget != null && piiTarget.heath > 0) {
            //Проверка перевертывания
            Vector3 grad = gameObject.transform.rotation.eulerAngles;
            if ((grad.x < 30 || grad.x > 330) && (grad.z < 30 || grad.z > 330))
            {
                TestTransform();
            }
            //Иначе если танк перевернулся то наносим ему урон
            else {

            }

            //TestTransform();
            TestTower();
            TestTrackNext();

            TestAnimation();
        }
    }

    //Трансформации танка
    void TestTransform() {
        //Сперва выбираем тип перемещения танка..

        //Узнаем растояние до игрока
        float DictanceToPlayer = Vector3.Distance(gameObject.transform.position, player.gameObject.transform.position);

        //Если растояние больше и точка движения есть
        if (DictanceToPlayer > DistanceNoTrace && target_track_point != null)
        {
            //то двигаемся по контрольным точкам
            RotatePoint();
            MovePoint();
        }
        //если меньше
        else if(DictanceToPlayer > 10) {
            //то бездорожный режим
            RotateNoPoint();
            MoveNoPoint();
        }
    }


    void MovePoint(){
        //Узнаем скорость
        if (rigidbody != null && target_track_point != null)
        {
            //Если скорость довольно маленькая
            //уменьшаем радиус
            if (rigidbody.velocity.magnitude < 0.02f)
            {
                radius_face_ray -= Time.deltaTime * speed_face_ray;
            }
            else
            {
                radius_face_ray = 2;
            }

            //Проверка на препятствие впереди
            Ray face = new Ray(transform.position, transform.forward);
            RaycastHit faceHit;
            bool face_triger = false;
            if (Physics.SphereCast(face, radius_face_ray, out faceHit))
            {
                faceTarget = faceHit.collider.gameObject.GetComponent<PiiTarget>();
                if (faceHit.distance < (3 + rigidbody.velocity.magnitude * 0.7f) && faceTarget != null && faceTarget.heath > 0)
                {
                    face_triger = true;
                }
            }
            else
            {
                faceTarget = null;
            }

            //скорость
            float max_speed = target_track_point.max_speed/3;

            int revers = 1;

            //Прибавляем скорость если она маленькая
            if (rigidbody.velocity.magnitude < max_speed && !face_triger)
            {
                rigidbody.velocity = rigidbody.velocity + gameObject.transform.forward * 50f * Time.deltaTime * revers;
            }
        }
    }
    void RotatePoint(){
        //танк поворачивается вне зависимости от скорости

        if (rigidbody != null && target_track_point != null)
        {
            Quaternion rotate_to = Quaternion.LookRotation(target_track_point.transform.position - transform.position);
            float old_y = transform.rotation.y;
            transform.rotation = Quaternion.Lerp(transform.rotation, rotate_to, 0.5f * Time.deltaTime);
            rotateNow = old_y - transform.rotation.y;
        }
    }

    void MoveNoPoint() {

        float DistPlayer = Vector3.Distance(gameObject.transform.position, player.transform.position);

        if (rigidbody != null && DistPlayer > 30)
        {
            //Если скорость довольно маленькая
            //уменьшаем радиус
            if (rigidbody.velocity.magnitude < 0.02f)
            {
                radius_face_ray -= Time.deltaTime * speed_face_ray;
            }
            else
            {
                radius_face_ray = 2;
            }

            //Проверка на препятствие впереди
            Ray faceNormal = new Ray(transform.position, transform.forward);

            RaycastHit faceHit;
            bool face_triger = false;
            if (Physics.SphereCast(faceNormal, radius_face_ray, out faceHit))
            {
                faceTarget = faceHit.collider.gameObject.GetComponent<PiiTarget>();
                if (faceHit.distance < (5 + rigidbody.velocity.magnitude * 0.7f) && faceTarget != null && faceTarget.heath > 0 && faceTarget.MinPresure >= piiTarget.MinPresure)
                {
                    face_triger = true;
                }
            }
            else
            {
                faceTarget = null;
            }

            //скорость
            float max_speed = 3;

            int revers = 1;
            if (face_triger) {
                revers = -1;
            }

            
            float Stop = 1;
            if (DistPlayer < DistanceNoTraceStop) {
                Stop = DistPlayer / DistanceNoTraceStop;
            }

            //Прибавляем скорость если она маленькая
            if (rigidbody.velocity.magnitude < max_speed)
            {
                rigidbody.velocity = rigidbody.velocity + gameObject.transform.forward * 50f * Time.deltaTime * revers * Stop;
            }
        }

    }
    void RotateNoPoint() {
        //Создаем 3 луча
        Ray faceLeft = new Ray(transform.position, rayLeft.transform.forward);
        Ray faceRight = new Ray(transform.position, rayRight.transform.forward);

        //Проверяем каждый луч сперва на разрушаемость обьекта и если нет то вытаскиваем дальность
        float faceDistLeft = 999;
        float faceDistRigh = 999;

        float distRot = 4;

        RaycastHit faceHit;

        //тест дальности боковых лучей
        //Левый
        if (Physics.SphereCast(faceLeft, 4, out faceHit))
        {
            faceDistLeft = faceHit.distance;
            faceTargetLeft = faceHit.collider.gameObject.GetComponent<PiiTarget>();
            //Если прочность больше прочности танка
            if (faceTargetLeft != null && faceTargetLeft.MinPresure < piiTarget.MinPresure)
            {
                faceDistLeft = distRot*2;
            }
        }
        else {
            faceTargetLeft = null;
        }

        //правий
        if (Physics.SphereCast(faceRight, 4, out faceHit))
        {
            faceDistRigh = faceHit.distance;
            faceTargetRight = faceHit.collider.gameObject.GetComponent<PiiTarget>();
            //Если прочность больше прочности танка
            if (faceTargetRight != null && faceTargetRight.MinPresure < piiTarget.MinPresure)
            {
                faceDistRigh = distRot * 2;
            }
        }
        else
        {
            faceTargetRight = null;
        }
        
        TimeDistLeft = TimeDistLeft * 0.7f + faceDistLeft * 0.3f;
        TimeDistRigh = TimeDistRigh * 0.7f + faceDistRigh * 0.3f;

        //Дальность получена, теперь вращаем
        //Если хотябы один из лучей меньше 10 то отворачиваемся от него

        Debug.Log("Left " + TimeDistLeft + " | Right " + TimeDistRigh);
        
        if (TimeDistLeft < distRot || TimeDistRigh < distRot)
        {

            //Отворот лева
            if ((TimeDistLeft < distRot && TimeDistLeft < TimeDistRigh))
            {
                gameObject.transform.Rotate(0, 20 * Time.deltaTime ,0);
            }
            //отворот права
            else if ((TimeDistRigh < distRot && TimeDistRigh < TimeDistLeft))
            {
                gameObject.transform.Rotate(0, -20 * Time.deltaTime, 0);
            }
        }
        //Если препятствий нет то поворачиваемся к игроку под углом
        else {
            if (rigidbody != null && player != null)
            {
                //Нужно ли отклонение в градусах
                float distPlayer = Vector3.Distance(gameObject.transform.position, player.transform.position);
                float otclonenie = OtclonenieGradus;
                if (distPlayer < 200) {
                    otclonenie *= distPlayer / 100; 
                }

                Quaternion rotate_to = Quaternion.LookRotation(player.transform.position - transform.position);
                rotate_to.eulerAngles = new Vector3(rotate_to.eulerAngles.x, rotate_to.eulerAngles.y + otclonenie, rotate_to.eulerAngles.z);
                //rotate_to.SetEulerRotation(0, OtclonenieGradus, 0);
                float old_y = transform.rotation.y;
                transform.rotation = Quaternion.Lerp(transform.rotation, rotate_to, 0.5f * Time.deltaTime);
                rotateNow = old_y - transform.rotation.y;
            }
        }
    }
     
    //проверка следующего пути 
    void TestTrackNext()
    {
        if (target_track_point != null)
        {
            //Если достаточно близко к настоящей точке
            float distanceToTrack = Vector3.Distance(gameObject.transform.position, target_track_point.transform.position);
            if (distanceToTrack < 10)
            {

                //Переключаемся на следуюшую если есть
                if (target_track_point.point_next_normal != null || target_track_point.point_next_left != null || target_track_point.point_next_right != null)
                {
                    float player_turn = 0;
                    //Если есть игрок считаем желание
                    if (player != null && player.gameplayParametrs.get_lvl_now() > 2)
                    {
                        //Узнаем напор игрока
                        float speed_pii = player.bolt.GetComponent<PiiController>().speed_pii;
                        speed_pii -= 12;
                        if (speed_pii < 0)
                            speed_pii = 0;

                        //Узнаем требуемый поворот чтобы смотреть на игрока
                        Quaternion rot_player = Quaternion.LookRotation(gameObject.transform.position, player.transform.position);

                        float raznica = gameObject.transform.eulerAngles.y - rot_player.eulerAngles.y;
                        int plus_1 = 1;
                        if (gameObject.transform.eulerAngles.y < 180)
                        {
                            plus_1 *= -1;
                        }
                        //Смеряем с 
                        if (raznica > 0)
                        {
                            player_turn = 110 * plus_1 * speed_pii;
                        }
                        else
                        {
                            player_turn = -110 * plus_1 * speed_pii;
                        }
                        
                        player_turn = player_turn * -15;
                    }

                    //Если есть поворот и желание повернуть
                    //налево
                    if (player_turn < -30 && target_track_point.point_next_left != null)
                    {
                        target_track_point = target_track_point.point_next_left;
                    }
                    else if (player_turn > 30 && target_track_point.point_next_right != null)
                    {
                        target_track_point = target_track_point.point_next_right;
                    }
                    else if (player_turn >= -30 && player_turn <= 30 && target_track_point.point_next_normal != null)
                    {
                        target_track_point = target_track_point.point_next_normal;
                    }

                    //Если желание не совпадает
                    else if (target_track_point.point_next_normal != null)
                    {
                        target_track_point = target_track_point.point_next_normal;
                    }
                    else if (target_track_point.point_next_left != null)
                    {
                        target_track_point = target_track_point.point_next_left;
                    }
                    else if (target_track_point.point_next_right != null)
                    {
                        target_track_point = target_track_point.point_next_right;
                    }
                }
                //Если точек нету то удаляем
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    void TestTower() {
        //Узнаем растояние до игрока
        float DistanceToPlayer = Vector3.Distance(gameObject.transform.position, player.gameObject.transform.position);

        if (DistanceToPlayer < DistanceAgress) {
            RotateTowerToPlayer(DistanceToPlayer);

            TestFire();
        }
    }
    void TestFire() {
        ReloadTimeNow -= Time.deltaTime;
        if (ReloadTimeNow < 0 && !gameParam.GameOver && gameObject.transform.position.z > player.transform.position.z) {
            ReloadTimeNow = ReloadTime;
            FireShell();
        }
    }

    void RotateTowerToPlayer(float distPlayer) {
        //Нужно повернуть по Y основу
        //Узнаем локальный поворот сейчас

        //Запоминаем локальный поворот башни
        Tower.transform.Rotate(0, -90, 0);
        Quaternion localrotTowerNow = Tower.transform.localRotation;

        Quaternion lookNeedTower = Quaternion.LookRotation(player.gameObject.transform.position - Tower.transform.position);
        lookNeedTower.eulerAngles = new Vector3(0, lookNeedTower.eulerAngles.y, 0);
        Tower.transform.rotation = lookNeedTower;
        Quaternion localrotTowerNeed = Tower.transform.localRotation;
        localrotTowerNeed.eulerAngles = new Vector3(0, localrotTowerNeed.eulerAngles.y, 0);
        //ниже для немедленного поворота
        //Tower.transform.localRotation = localrotTowerNeed;
        //Возвращяем башню в начальное положение
        Tower.transform.localRotation = localrotTowerNow;

        //Считаем насколько надо повернуть
        float RotateGradNeed = localrotTowerNeed.eulerAngles.y - localrotTowerNow.eulerAngles.y;
        //проверяем на выход на 360
        if (RotateGradNeed > 180) {
            RotateGradNeed -= 360;
        }
        else if (RotateGradNeed < -180) {
            RotateGradNeed += 360;
        }


        float SpeedRot = 50;
        if (RotateGradNeed > SpeedRot)
        {
            RotateGradNeed = SpeedRot;
        }
        else if(RotateGradNeed < SpeedRot * -1) {
            RotateGradNeed = SpeedRot * -1;
        }
        RotateGradNeed *= Time.deltaTime;

        Vector3 resulrEuler = new Vector3(0, localrotTowerNow.eulerAngles.y + RotateGradNeed, 0);
        localrotTowerNow.eulerAngles = resulrEuler;
        Tower.transform.localRotation = localrotTowerNow;


        //Теперь нужно повернуть дуло
        //Узнаем разницу положений между игроком и танком
        Vector3 raznica = player.gameObject.transform.position - Barrel.transform.position;
        Quaternion localrotBarrel = Barrel.transform.localRotation;
        localrotBarrel.eulerAngles = new Vector3(0, 0, 0);
        Barrel.transform.localRotation = localrotBarrel;
        float gradusDulo = -100 * (raznica.y / distPlayer) - 3;
        if (gradusDulo > 7)
            gradusDulo = 7;
        else if (gradusDulo < -30)
            gradusDulo = -30;
        Barrel.transform.Rotate(0, 0, gradusDulo);

        Tower.transform.Rotate(0, 90, 0);
    }

    void FireShell(){
        GameObject Bullet = Instantiate(BulletPrefab);
        Bullet.transform.position = ShellOutputPosition.transform.position;
        Rigidbody rigidbodyBullet = Bullet.GetComponent<Rigidbody>();
        rigidbodyBullet.velocity = -1 * Barrel.transform.right * 100;
        rigidbody.velocity = rigidbody.velocity + Barrel.transform.right * 10;

        //Проверяем партикл если старый остался, удаляем
        if (fireBallerParticle != null) {
            Destroy(fireBallerParticle);
        }

        //Создаем новый взрыв
        if (fireBallerParticlePrefab != null) {
            fireBallerParticle = Instantiate(fireBallerParticlePrefab);
            //fireBallerParticle.transform.parent = ShellOutputPosition.transform;
            fireBallerParticle.transform.position = ShellOutputPosition.transform.position;
            //fireBallerParticle.GetComponent<ParticleSystem>().Stop();
            fireBallerParticle.GetComponent<ParticleSystem>().Play();
        }

        //шумим
        if (audioSourceFire != null && FireBarellAudio != null && FireBarellAudio.Length > 0) {
            audioSourceFire.volume = seting.game.volume_all * seting.game.volume_sound;

            audioSourceFire.PlayOneShot(FireBarellAudio[Random.Range(0, FireBarellAudio.Length)]);
        }
    }

    void TestAnimation() {
        //Вытаскиваем материал левой гусеницы
        Material MatLeftTrack = LeftTrack.GetComponent<MeshRenderer>().materials[0];
        //Вытаскиваем материал правой гусеницы
        Material MatRighTrack = RightTrack.GetComponent<MeshRenderer>().materials[0];

        Vector3 SumDvizenie = (gameObject.transform.forward.normalized + rigidbody.velocity.normalized) / 2;
        float speedCalc = Time.deltaTime * SumDvizenie.magnitude * rigidbody.velocity.magnitude;

        if (MatLeftTrack != null && rigidbody != null) {
            Vector2 pos_tex = MatLeftTrack.mainTextureOffset;
            //Vector направления
            pos_tex.y += speedCalc;
            MatLeftTrack.mainTextureOffset = pos_tex;
        }

        if (MatRighTrack != null && rigidbody != null)
        {
            Vector2 pos_tex = MatRighTrack.mainTextureOffset;
            pos_tex.y += speedCalc;
            MatRighTrack.mainTextureOffset = pos_tex;
        }

        if (LeftWheel != null) {
            for (int num = 0; num < LeftWheel.Length; num++) {
                LeftWheel[num].transform.Rotate(0, 0, speedCalc * 100);
            }
        }
        if (RightWhel != null) {
            for (int num = 0; num < RightWhel.Length; num++) {
                RightWhel[num].transform.Rotate(0, 0, speedCalc * 100);
            }
        }
    }

    public void BrokenTank() {
        if (!broke && Tower != null) {
            broke = true;

            //Разрушаем танк
            //Открепляем башню
            Tower.transform.parent = gameObject.transform.parent;
            Rigidbody rigidbodyTower = Tower.AddComponent<Rigidbody>();
            rigidbodyTower.mass = 5;
            rigidbodyTower.velocity += gameObject.transform.up * Random.Range(7f, 15f);

            if (DestroyTriger != null) {
                Destroy(DestroyTriger);
            }

            //Создаем спецэфект взрыва
            if (ParticleTankBoom != null) {
                GameObject partBoom = Instantiate(ParticleTankBoom);
                partBoom.transform.parent = gameObject.transform;
                partBoom.transform.position = Tower.transform.position;

                partBoom.GetComponent<ParticleSystem>().Play();
            }

            rigidbody.mass = 10;
        }
    }
}
