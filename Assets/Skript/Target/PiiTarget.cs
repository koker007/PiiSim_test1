using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiiTarget : MonoBehaviour {
    steam_achievement achievement;

    //эмоция страха в процессе
    [SerializeField]
    bool fear_emotion = false;
    //Эмоция страха после смерти предмета
    [SerializeField]
    bool fear_emotion_dead = false;

    [SerializeField]
    float startDistToKill = 0; //Дистанция для стартового уничтожения этой цели

    //Быстрое убийство для теста
    [SerializeField]
    bool killing = false;

    public float score = 1;
    public float score_destroy = 20;
    public float heath = 1;
    public float heath_percent = 100;
    public float MinPresure = 0;

    private float start_heath = 0;

    [SerializeField]
    private int lvl_target = 0;

    [SerializeField]
    PiiTarget[] SiskoTarget;

    [SerializeField]
    float second_to_destroy = 30;
    float numSetSecToDestroy = 0;

    [SerializeField]
    ParticleSystem particle;
    [SerializeField]
    ParticleSystem particleDie;

    public Player player_save;

    //Обьект который спавнится при смерти этой цели
    [SerializeField]
    GameObject ObjSpawnDie;

    GameObject score_text;
    Color colorNeed = new Color(1,1,1);
    Color colorNow = new Color(1,1,1);
    private struct MeshColors {
        public Color[] colors;
        public bool[] colorfound;
    }
    MeshColors[] meshColors;

    float ScoreSum;

    //Стартовая рандомизация цвета
    bool randomColor = true;

    GameplayParametrs gameplayParametrs;

	// Use this for initialization
	void Start () {
        start_heath = heath;
        Inicialize();
        get_steam_achivment();
        get_player();
        SetMaterialSetings();
        //iniRandomColor();
        iniGameParam();
    }
	
	// Update is called once per frame
	void Update () {
        TestPiiFailTime();
        TestDelete();
        TestColor();
        TestStartKill();
    }


    void iniGameParam() {
        if (gameplayParametrs == null) {
            gameplayParametrs = GameObject.FindGameObjectWithTag("setings_game").GetComponent<GameplayParametrs>();
        }
    }
    void get_steam_achivment()
    {
        if (achievement == null)
        {
            //ищем обьект по тегу
            GameObject steam_manager = GameObject.FindWithTag("steam_manager");
            //вытаскиваем настройки
            if (steam_manager != null) achievement = steam_manager.GetComponent<steam_achievement>();
        }
    }
    void get_player() {
        if (player_save == null) {
            //ищем обьект по тегу
            GameObject player_obj = GameObject.FindWithTag("Player");
            //вытаскиваем настройки
            if (player_obj != null) player_save = player_obj.GetComponent<Player>();
        }
    }

    public float get_start_heath() {
        return start_heath;
    }

    public void Inicialize() {
        //Создаем частицы если они есть
        if (particle != null) {
            GameObject part = Instantiate(particle.gameObject, gameObject.transform);
            part.transform.position.Set(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z); 
            particle = part.GetComponent<ParticleSystem>();
            particle.Stop();
        }
        if (particleDie != null)
        {
            GameObject part = Instantiate(particleDie.gameObject, gameObject.transform);
            part.transform.position.Set(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
            particleDie = part.GetComponent<ParticleSystem>();
            particleDie.Stop();
        }

        //Применяем пользовательский материал c шейдером
        //gameObject.AddComponent<SeterMaterial>();       
    }

    //Убийство цели
    public void DeathTarget() {

        if (heath > 0)
            heath = 0;

        achievement.plus_1_destroy_object();
        player_save.PlusCombo();

        //Уменьшаем вес
        Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();

        if (rigidbody != null)
        {
            //Размораживаем
            rigidbody.constraints = RigidbodyConstraints.None;
        }

        HingeJoint hingeJoint = gameObject.GetComponent<HingeJoint>();
        if (hingeJoint != null) {
            GameObject.Destroy(hingeJoint);
        }

        //Если есть частицы смерти
        if (particleDie != null)
        {
            particleDie.Play();
        }

        //Воспросизводим звук смерти
        TargetSound sounds = gameObject.GetComponent<TargetSound>();
        if (sounds != null)
        {
            gameObject.GetComponent<TargetSound>().PlaySoundDestroy();
        }

        //Спавним обьект после смерти
        if (ObjSpawnDie != null) {
            GameObject DieObj = Instantiate(ObjSpawnDie, gameObject.transform.parent);
            DieObj.transform.position = gameObject.transform.position;
        }

        //Если это постройка
        BildTarget bild = gameObject.GetComponent<BildTarget>();
        if (bild != null)
        {
            bild.need_destroy = true;
        }
        //Если цель автомобиль
        CarStatic CarSiren = gameObject.GetComponent<CarStatic>();
        if (CarSiren != null)
        {
            CarSiren.OffSignal();
        }

        //Если это автомобиль
        Automobile automobile = gameObject.GetComponent<Automobile>();
        if (automobile != null && rigidbody != null)
        {
            rigidbody.mass = rigidbody.mass / 100;

            police_car police = gameObject.GetComponent<police_car>();
            if (police != null && achievement != null)
            {
                achievement.plus_1_destroy_police();
            }
        }

        //Если это танк
        Tank tank = gameObject.GetComponent<Tank>();
        if (tank != null) {
            tank.BrokenTank();
        }
        //Если это пуля танка
        TankBullet tankBullet = gameObject.GetComponent<TankBullet>();
        if (tankBullet != null) {
            tankBullet.testBoom = true;
        }

        flyer fly = gameObject.GetComponent<flyer>();
        if (fly != null && achievement != null) {
            achievement.destroy_fighter();
        }

        //Взрываем
        BoomTarget boom = gameObject.GetComponent<BoomTarget>();
        if (boom != null)
        {
            boom.BOOM();
        }

        //Убиваем все дочерние обьекты
        //PiiTarget[] piiTargets = gameObject.GetComponentsInChildren<PiiTarget>();
        //if (piiTargets != null) {
        //    for (int num = 0; num < piiTargets.Length; num++) {
        //        if (piiTargets[num] != gameObject.GetComponent<PiiTarget>()) {
        //            piiTargets[num].DeathTarget();
        //        }
        //   }
        //}

        //убиваем все дочерние обьекты по новому
        if (SiskoTarget != null && SiskoTarget.Length != 0)
        {
            for (int x = 0; x < SiskoTarget.Length; x++)
            {
                if (SiskoTarget[x] != null && SiskoTarget[x].heath > 0)
                {
                    SiskoTarget[x].DeathTarget();
                }
            }
        }
    }

    float TryTestPiiFall = 0;
    private void TestPiiFailTime() {
        if (TryTestPiiFall > 12) {
            TryTestPiiFall = 12;
        }
        if (TryTestPiiFall >= 0) {
            TryTestPiiFall -= Time.deltaTime * 0.5f;
        }
    }
    //Оповещает игрока что нанести урон не получается
    public void TrigerPiiFail(GameObject score_Text_func) {
        TryTestPiiFall++;
        if (score_Text_func != null && TryTestPiiFall > 10)
        {
            if (score_text != null)
                GameObject.Destroy(score_text);

            score_text = Instantiate(score_Text_func);
            //score_text.transform.parent = gameObject.transform;
            Score_text_3d score_Text_3D = score_text.GetComponent<Score_text_3d>();

            if (heath > 0)
            {
                Color color_new = new Color(1f, 0.0f, 0, 0.9f);
                score_Text_3D.inicialize(player_save.gameObject, "X", gameObject.transform.position, color_new);
            }
        }
    }
    public void TrigerPii(float presure, FaceController faceController, PiiController piiController, GameObject score_Text_func) {

        //Если у мочи достаточно инерции и если у предмета еще есть здоровье
        if (presure >= MinPresure && heath != 0) {

            float score_plus_now = 0;

            if (heath > 0) {

                //Сперва проверяем напрягся ли игрок
                if (player_save != null && player_save.bolt.GetComponent<PiiController>().ChargeOn) {
                    heath -= presure;
                    if (!gameplayParametrs.GameOver)
                    {
                        score_plus_now = score * 1.5f;
                        player_save.PlusScore(score_plus_now);
                    }
                }
                else if (player_save != null && !player_save.bolt.GetComponent<PiiController>().ChargeOn) {
                    score_plus_now = score;
                    player_save.PlusScore(score_plus_now);
                }
                heath -= presure;
                
                if (heath < 0)
                    heath = 0;
                heath_percent = heath /(start_heath / 100);
                //Если есть частицы то воспроизводим их
                if (particle != null) {
                    particle.Play();
                }

                SetColorNeed(heath_percent);

                //Если здоровье закончилось
                if (heath <= 0)
                {
                    DeathTarget();
                    test_lvl_target(player_save.gameplayParametrs);

                    score_plus_now = score * score_destroy;
                    player_save.PlusScore(score_plus_now);

                    if (faceController != null && fear_emotion_dead && Random.Range(0, 100) < 50) {
                        faceController.set_fear(3f);
                    }

                    helicopter helicopterObj = gameObject.GetComponent<helicopter>();
                    if (helicopterObj != null) {
                        player_save.SetFullCharge();
                    }
                }
                //Иначе звук попадания
                else {
                    //Воспросизводим звук попадания
                    TargetSound sounds = gameObject.GetComponent<TargetSound>();
                    if (sounds != null)
                    {
                        gameObject.GetComponent<TargetSound>().PlaySoundPii();
                    }

                    //Если цель автомобиль воспроизводим сигнализацию
                    CarStatic CarSiren = gameObject.GetComponent<CarStatic>();
                    if (CarSiren != null) {
                        CarSiren.StartSignal();
                    }
                    //Если цель полиция то запускаем сирену
                    police_car police = gameObject.GetComponent<police_car>();
                    if (police != null && !police.police_active_now) {
                        police.police_active_now = true;
                        police.need_sound_yn = true;
                    }
                }
            }

            if (faceController != null && !piiController.ChargeOn) {
                if (Random.Range(0, 100) < 1.1f)
                {
                    faceController.set_bliss_voice(1.2f);
                }
                if (Random.Range(0, 100) < 0.1f)
                {
                    faceController.set_surprise_voice(1.2f);
                }

                if (Random.Range(0, 100) < 0.1f && fear_emotion) {
                    faceController.set_choked_voice();
                }
            }


            if (score_Text_func != null)
            {
                if (score_text != null)
                    GameObject.Destroy(score_text);

                score_text = Instantiate(score_Text_func);
                Score_text_3d score_Text_3D = score_text.GetComponent<Score_text_3d>();

                ScoreSum += score_plus_now;
                //Проверка того что нужно написать в зависимости от здоровья
                if (heath > 0)
                {
                    Color color_new = new Color(1f, 0.92f, 0, 0.62f);
                    score_Text_3D.inicialize(player_save.gameObject, ((int)heath_percent).ToString() + "%", gameObject.transform.position, color_new);
                }
                else {
                    Color color_new = new Color(1f, 0.47f, 0, 0.62f);
                    score_Text_3D.inicialize(player_save.gameObject, ((int)ScoreSum).ToString(), gameObject.transform.position, color_new);
                }
            }
            

        }
    }

    private void test_lvl_target(GameplayParametrs parametrs) {
        //проверка уровня обьекта
        if (lvl_target != 0)
        {
            if (lvl_target == 3)
            {
                parametrs.start_lvl_3();
            }
            if (lvl_target == 4)
            {
                parametrs.start_lvl_4();
            }
            if (lvl_target == 5)
            {
                parametrs.start_lvl_5();
            }
            if (lvl_target == 6) {
                parametrs.start_lvl_6();
            }
        }
    }

    bool TeststartKiilNeed = true;
    float TestStartKillTime = 0;
    void TestStartKill() {

        if (TeststartKiilNeed)
        {
            TestStartKillTime += Time.deltaTime;
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null && TestStartKillTime > 1)
            {
                float distToPlayer = Vector3.Distance(player.transform.position, gameObject.transform.position);
                if (startDistToKill > 1f && startDistToKill < distToPlayer)
                {
                    Destroy(gameObject);
                }
            }
            if (TestStartKillTime > 1) {
                TeststartKiilNeed = false;
            }
        }
    } //Удаление в начале если растояние больше указанного
    void TestComand() {
        if (killing && heath > 0) {
            killing = true;
            heath = 0;
            DeathTarget();
        }
    }
    void TestDelete() {
        if (heath <= 0) {
            second_to_destroy -= Time.deltaTime;

            if (second_to_destroy <= 0) {
                Destroy(gameObject);
            }
        }
    }
    void TestColor() {

        if (heath > 0)
        {
            //Постоянно уменьшаем требуемый цвет до белого
            if (colorNeed.r < 0.99 || colorNeed.g < 0.99 || colorNeed.b < 0.99)
            {
                colorNeed.r = colorNeed.r + (1 - colorNeed.r) * Time.deltaTime / 5;
                colorNeed.g = colorNeed.g + (1 - colorNeed.g) * Time.deltaTime / 5;
                colorNeed.b = colorNeed.b + (1 - colorNeed.b) * Time.deltaTime / 5;
                if (colorNeed.r > 1)
                    colorNeed.r = 1;
                if (colorNeed.g > 1)
                    colorNeed.g = 1;
                if (colorNeed.b > 1)
                    colorNeed.b = 1;
            }
        }
        else {
            //Постоянно уменьшаем требуемый цвет до белого
            if (colorNeed.r < 0.99 || colorNeed.g < 0.99 || colorNeed.b < 0.99)
            {
                colorNeed.r = colorNeed.r + (1 - colorNeed.r) * Time.deltaTime / 20;
                colorNeed.g = colorNeed.g + (1 - colorNeed.g) * Time.deltaTime / 20;
                colorNeed.b = colorNeed.b + (1 - colorNeed.b) * Time.deltaTime / 20;
                if (colorNeed.r > 1)
                    colorNeed.r = 1;
                if (colorNeed.g > 1)
                    colorNeed.g = 1;
                if (colorNeed.b > 1)
                    colorNeed.b = 1;
            }
        }

        //Применяем изменения
        if (colorNeed != colorNow && (colorNow.r != 0.99 || colorNow.g != 0.99 || colorNow.b != 0.99)) {
            colorNow.r = colorNow.r + (colorNeed.r - colorNow.r) * Time.deltaTime;
            if (colorNow.r > 0.99) colorNow.r = colorNeed.r;
            colorNow.g = colorNow.g + (colorNeed.g - colorNow.g) * Time.deltaTime;
            if (colorNow.g > 0.99) colorNow.g = colorNeed.g;
            colorNow.b = colorNow.b + (colorNeed.b - colorNow.b) * Time.deltaTime;
            if (colorNow.b > 0.99) colorNow.b = colorNeed.b;

            //Ищем меш рендеры
            MeshRenderer[] meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();

            //Знаем сколько мешей у обьекта, создаем для запоминания базового цвета
            if (meshColors == null)
                meshColors = new MeshColors[meshRenderers.Length];

            //Перебираем меши
            for (int num_mesh = 0; num_mesh < meshRenderers.Length; num_mesh++) {

                //Создаем материалы которые будем изменять
                Material[] materialsNew = meshRenderers[num_mesh].materials;

                //Если первичного цвета, нет сохраняем
                bool needSaveColor = false;
                if (num_mesh < meshColors.Length && meshColors[num_mesh].colors == null)
                {
                    meshColors[num_mesh].colors = new Color[materialsNew.Length];
                    meshColors[num_mesh].colorfound = new bool[materialsNew.Length];

                    needSaveColor = true;
                }

                //Перебираем все материалы обьекта
                for (int num_mat = 0; num_mat < materialsNew.Length; num_mat++) {

                    //Узнаем был ли найден параметр "цвет"
                    if (num_mesh < meshColors.Length && num_mat < meshColors[num_mesh].colorfound.Length && !meshColors[num_mesh].colorfound[num_mat])
                    {
                        //Если в этом шейдере есть ключ цвета
                        if (materialsNew[num_mat].HasProperty("_Color") && !meshColors[num_mesh].colorfound[num_mat])
                        {
                            //запоминаем базовый цвет и говорим что ключ найден
                            meshColors[num_mesh].colors[num_mat] = new Color(materialsNew[num_mat].color.r, materialsNew[num_mat].color.g, materialsNew[num_mat].color.b);
                            meshColors[num_mesh].colorfound[num_mat] = true;

                            //запоминаем базовый цвет и говорим что ключ найден
                            //meshColors[num_mesh].colors[num_mat] = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
                        }
                    }

                    //Если параметр цвета есть
                    else if(num_mesh < meshColors.Length && num_mat < meshColors[num_mesh].colorfound.Length && meshColors[num_mesh].colorfound[num_mat]) {
                        materialsNew[num_mat].SetColor("_Color", colorNow * meshColors[num_mesh].colors[num_mat]);
                    }
                }

                //Применяем все изменения
                meshRenderers[num_mesh].materials = materialsNew;
            }
        }

    }
    void SetMaterialSetings() {
        //Ищем меш рендеры
        MeshRenderer[] meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();

        for (int num_mesh = 0; num_mesh < meshRenderers.Length; num_mesh++)
        {

            Material[] materialsNew = meshRenderers[num_mesh].sharedMaterials;

            for (int num_mat = 0; num_mat < materialsNew.Length; num_mat++)
            {
                //if (materialsNew[num_mat].IsKeywordEnabled("_GlossyReflections")) {
                    //Debug.Log("_GlossyReflections");
                    //materialsNew[num_mat].EnableKeyword("_SPECULARHIGHLIGHTS_OFF");
                    materialsNew[num_mat].SetFloat("_Glossiness", 0f);
                    materialsNew[num_mat].EnableKeyword("_GlossyReflections");
                //}
            }

            //meshRenderers[num_mesh].materials = materialsNew;
        }
    }

    void SetColorNeed(float percent) {
        float percentSmall = percent / 100;

        Color color100 = new Color(1f, 1f, 0.75f);
        Color color0 = new Color(1, 1, 0);

        //не деленое на 100
        Color OnePercent = new Color((color100.r - color0.r), (color100.g - color0.g), (color100.b - color0.b));

        colorNeed = new Color(color0.r + OnePercent.r*percentSmall, color0.g + OnePercent.g*percentSmall, color0.b + OnePercent.b*percentSmall);
    }

    public void SetTimeToDelete(float new_time) {
        if (second_to_destroy < new_time) {
            numSetSecToDestroy = numSetSecToDestroy + 2f;
            second_to_destroy += new_time/numSetSecToDestroy;
        }
    }

    //Рандомный цвет при старте, выключено
    void iniRandomColor() {
        if (randomColor) {
            randomColor = false;    
            MeshRenderer[] meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();

            for (int num_mesh = 0; num_mesh < meshRenderers.Length; num_mesh++) {
                Material[] materials = meshRenderers[num_mesh].materials;
                for (int num_mat = 0; num_mat < materials.Length; num_mat++) {
                    //пытаемся получить цвет из материала если такой параметр есть
                    //Если в этом шейдере есть ключ цвета
                    if (materials[num_mat].HasProperty("_Color"))
                    {
                        //получаем базовый цвет
                        Color colorBase = materials[num_mat].color;
                        if (colorBase.r < 0.01f && colorBase.g > 0.99f && colorBase.b < 0.01f) {
                            //переделываем базовый цвет
                            materials[num_mat].color = new Color(Random.Range(0.0f, 0.6f), Random.Range(0.0f, 0.6f), Random.Range(0.0f, 0.6f));
                        }
                    }
                }
            }
        }
    }

}
