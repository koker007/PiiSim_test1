using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomTarget : MonoBehaviour {

    setings seting;

    [SerializeField]
    ParticleSystem Explore;
    [SerializeField]
    ParticleSystem Smoke;

    [SerializeField]
    AudioClip[] Boom;
    [SerializeField]
    AudioSource audioSource;

    [SerializeField]
    bool PlayerDamage = false;

    [SerializeField]
    int Veroatnost_boom = 100;
    [SerializeField]
    float Damage = 0;
    [SerializeField]
    float size_boom_max = 0;
    [SerializeField]
    float speed_boom = 50;

    [SerializeField]
    GameObject DestroyPrefabObj;
    [SerializeField]
    GameObject[] OblomkiChastObj;
    [SerializeField]
    GameObject[] OblomkiPrefabObj;
    [SerializeField]
    int max_oblomki_create = 0;
    [SerializeField]
    float min_dist_oblomki_create = 150;

    [SerializeField]
    GameObject hit_text;

    PiiTarget[] Targets = new PiiTarget[100];
    private float target_sum_now = 0;
    private float size_boom_now = 0;
    private bool boom_yn = false;
    //Время взрыва с момента его начала
    private float time_boom_now = 0;
    //Какую цель нужно проверить сейчас
    private int target_now = 0;
    private int target_max = 0;
    
	// Use this for initialization
	void Start () {
        get_seting_game();
        create_audiosource();
    }
	
	// Update is called once per frame
	void Update () {
        explosion_test2();
	}

    //Привязать настройки игры
    void get_seting_game()
    {
        //Если настроек нет
        if (seting == null)
        {
            //ищем обьект по тегу
            GameObject main_canvas = GameObject.FindWithTag("setings_game");
            //вытаскиваем настройки
            if (main_canvas != null) seting = main_canvas.GetComponent<setings>();
        }
    }

    public void BOOM() {
        if (Boom.Length != 0 && Random.Range(0, 100) < Veroatnost_boom) {
            //Звук
            if (audioSource != null) {
                audioSource.Stop();
                audioSource.maxDistance = 2000;
                audioSource.volume = 1 * seting.game.volume_all * seting.game.volume_sound;
                audioSource.PlayOneShot(Boom[Random.Range(0, Boom.Length)]);
            }
            //Активируем партикл взрыва
            if (Explore != null) {
                ParticleSystem exp = Instantiate(Explore, gameObject.transform);
                exp.Play();
            }
            //Активируем дым
            if (Smoke != null) {
                ParticleSystem smo = Instantiate(Smoke, gameObject.transform);
                smo.Play();
            }

            size_boom_now = 0;
            boom_yn = true;
            for (int num = 0; num < Targets.Length; num++) {
                Targets[num] = null;
            }
        }

        //Замена обьекта на разрушенную модель
        if (DestroyPrefabObj != null) {
            //Ищем оригинальный меш в самом обьекте
            MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
            //Если присутствует отключаем
            if (meshRenderer != null) {
                meshRenderer.enabled = false;
                //колайдер
                BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();
                if(boxCollider != null)
                    boxCollider.enabled = false;

                

            }

            //создаем разрушенное
            GameObject destroyObj = Instantiate(DestroyPrefabObj,  gameObject.transform);
            //Перемещаем
            destroyObj.transform.position = gameObject.transform.position;
            destroyObj.transform.localScale = new Vector3(1, 1, 1);
            destroyObj.transform.rotation = new Quaternion();

        }

        GameObject player_obj = GameObject.FindGameObjectWithTag("Player");
        float player_dist = Vector3.Distance(player_obj.transform.position, gameObject.transform.position);
        //Открепляем отдельные детали обьекта
        if (OblomkiChastObj != null && OblomkiChastObj.Length > 0 && player_dist < min_dist_oblomki_create) {

            //перебираем части
            for (int num_oblomok = 0; num_oblomok < OblomkiChastObj.Length; num_oblomok++) {
                //Если часть существует
                if (OblomkiChastObj[num_oblomok] != null && Random.Range(0, 100) < 50) {
                    //Открепляем ее от объекта меняя родителя
                    Transform transform_parent = gameObject.GetComponentInParent<Transform>();
                    if (transform_parent != null) {
                        //Поменяли родителя
                        OblomkiChastObj[num_oblomok].transform.parent = transform_parent.parent;
                        //Вытаскиваем ригидбоди
                        Rigidbody oblomok_rigidbody = OblomkiChastObj[num_oblomok].GetComponent<Rigidbody>();
                        //если ригид боди нету создаем
                        if (oblomok_rigidbody == null) {
                            oblomok_rigidbody = OblomkiChastObj[num_oblomok].AddComponent<Rigidbody>();
                        }

                        //если у этого обьекта есть масса то обломок делаем частичной массы главного обьекта
                        Rigidbody gameobj_rigidbody = gameObject.GetComponent<Rigidbody>();
                        if (gameobj_rigidbody != null) {
                            oblomok_rigidbody.mass = gameobj_rigidbody.mass / OblomkiChastObj.Length / 0.10f;
                        }
                        else {
                            oblomok_rigidbody.mass = 1;
                        }

                        //придаем импульс
                        //////////////////
                        /*
                        //считаем направление
                        Vector3 vectorImpulse = Vector3.Normalize(OblomkiChastObj[num_oblomok].transform.position - gameObject.transform.position);
                        //Чем массивнее тем слабее отталкивание
                        float mass_koof = 0;
                        if (oblomok_rigidbody.mass != 0)
                            mass_koof = 1 / oblomok_rigidbody.mass;
                        float dist_koof = 0;
                        float distance = Vector3.Distance(gameObject.transform.position, OblomkiChastObj[num_oblomok].transform.position);
                        if (distance > 0)
                            dist_koof = 1 / distance;
                        //Расчет силы оталкивания 
                        float inercia_strong = Damage * dist_koof * mass_koof * 0.02f;
                        //Если сила отталкивания не слабая
                        if (inercia_strong > 0.2f)
                            oblomok_rigidbody.velocity += inercia_strong * vectorImpulse;
                        */

                        //Тест бокс колайдела
                        BoxCollider oblomok_boxCollider = OblomkiChastObj[num_oblomok].GetComponent<BoxCollider>();
                        if (oblomok_boxCollider == null) {
                            OblomkiChastObj[num_oblomok].AddComponent<BoxCollider>();
                        }

                        //Тест пии таргета
                        PiiTarget oblomok_piiTarget = OblomkiChastObj[num_oblomok].GetComponent<PiiTarget>();
                        if (oblomok_piiTarget == null) {
                            oblomok_piiTarget = OblomkiChastObj[num_oblomok].AddComponent<PiiTarget>();
                        }
                        oblomok_piiTarget.heath = 1;
                        oblomok_piiTarget.MinPresure = 1000;
                        
                        /*
                        destroy_timer destroy_Timer = OblomkiChastObj[num_oblomok].GetComponent<destroy_timer>();
                        if (destroy_Timer == null) {
                            destroy_Timer = OblomkiChastObj[num_oblomok].AddComponent<destroy_timer>();
                        }
                        destroy_Timer.timer_starting = true;
                        */
                    }

                }
            }
        }

        //СОздаем префабы обломков
        if (OblomkiPrefabObj != null && OblomkiPrefabObj.Length > 0 && player_dist < min_dist_oblomki_create) {
            //перебираем части
            for (int num_oblomok = 0; num_oblomok < OblomkiPrefabObj.Length; num_oblomok++)
            {
                //Если часть существует
                if (OblomkiPrefabObj[num_oblomok] != null && Random.Range(0, 100) < 90)
                {
                    //Узнаем родителя
                    Transform transform_parent = gameObject.GetComponentInParent<Transform>();

                    //Создаем обьект
                    GameObject OblomokObj = Instantiate(OblomkiPrefabObj[num_oblomok], transform_parent.parent);
                    //Меняем позицию обломка
                    OblomokObj.transform.position = gameObject.transform.position + new Vector3(Random.Range(0, 1), Random.Range(0, 1) + 0.25f, Random.Range(0, 1));

                    /////////////////////////////////////////////////////////////


                    //Вытаскиваем ригидбоди
                    Rigidbody oblomok_rigidbody = OblomokObj.GetComponent<Rigidbody>();
                    //если ригид боди нету создаем
                    if (oblomok_rigidbody == null)
                    {
                        oblomok_rigidbody = OblomokObj.AddComponent<Rigidbody>();
                    }

                    //если у этого обьекта есть масса то обломок делаем частичной массы главного обьекта
                    Rigidbody gameobj_rigidbody = gameObject.GetComponent<Rigidbody>();
                    if (gameobj_rigidbody != null)
                    {
                        oblomok_rigidbody.mass = gameobj_rigidbody.mass / OblomkiPrefabObj.Length / 0.10f;
                    }
                    else
                    {
                        oblomok_rigidbody.mass = 1;
                    }

                    //придаем импульс
                    //////////////////
                    /*
                    //считаем направление
                    Vector3 vectorImpulse = Vector3.Normalize(OblomokObj.transform.position - gameObject.transform.position);
                    //Чем массивнее тем слабее отталкивание
                    float mass_koof = 0;
                    if (oblomok_rigidbody.mass != 0)
                        mass_koof = 1 / oblomok_rigidbody.mass;
                    float dist_koof = 0;
                    float distance = Vector3.Distance(gameObject.transform.position, OblomokObj.transform.position);
                    if (distance > 0)
                        dist_koof = 1 / distance;
                    //Расчет силы оталкивания 
                    float inercia_strong = Damage * dist_koof * mass_koof * 0.02f;
                    //Если сила отталкивания не слабая
                    if (inercia_strong > 0.2f)
                        oblomok_rigidbody.velocity += inercia_strong * vectorImpulse;
                    */

                    //Тест бокс колайдела
                    BoxCollider oblomok_boxCollider = OblomokObj.GetComponent<BoxCollider>();
                    if (oblomok_boxCollider == null)
                    {
                        OblomkiPrefabObj[num_oblomok].AddComponent<BoxCollider>();
                    }

                    //Тест пии таргета
                    PiiTarget oblomok_piiTarget = OblomokObj.GetComponent<PiiTarget>();
                    if (oblomok_piiTarget == null)
                    {
                        oblomok_piiTarget = OblomokObj.AddComponent<PiiTarget>();
                    }
                    oblomok_piiTarget.heath = 1;
                    oblomok_piiTarget.MinPresure = 1000;

                    /*
                    destroy_timer destroy_Timer = OblomokObj.GetComponent<destroy_timer>();
                    if (destroy_Timer == null)
                    {
                        destroy_Timer = OblomokObj.AddComponent<destroy_timer>();
                    }
                    destroy_Timer.time_to_destroy = 30f; //(min_dist_oblomki_create - player_dist) * 2;
                    destroy_Timer.timer_starting = true;
                    */
                }
            }
        }

    }

    public void explosion_test() {
        if (boom_yn) {
            //Если радиус еще маленький
            if (size_boom_now < size_boom_max)
            {
                //Пытаемся взормать обьекты
                //Вытаскиваем обьекты которые внутри
                Collider[] hitTargets = Physics.OverlapSphere(gameObject.transform.position, size_boom_now);

                //вытаскиваем цели
                for (int num_hit = 0; num_hit < hitTargets.Length; num_hit++) {
                    PiiTarget piinow = hitTargets[num_hit].gameObject.GetComponent<PiiTarget>();

                    if (piinow != null) {
                        //Проверяем есть ли эта цель в списке?
                        bool found_yn = false;
                        for (int num_target = 0; num_target < Targets.Length; num_target++)
                        {
                            //Если обьект нашелся в списке
                            //или если эта цель сам этот обьект
                            if (Targets[num_target] == piinow || gameObject == piinow.gameObject)
                            {
                                found_yn = true;
                                num_target = Targets.Length;
                            }
                            //Если список закончелся
                            else if (Targets[num_target] == null)
                            {
                                num_target = Targets.Length;
                            }
                        }
                        //Если не нашлось
                        if (!found_yn)
                        {
                            //Добавляем в свободное
                            for (int num_target = 0; num_target < Targets.Length; num_target++)
                            {
                                if (Targets[num_target] == null)
                                {
                                    //Считаем какой урон нужно нанести
                                    //процент расширенности
                                    float percent = size_boom_now / size_boom_max;
                                    float hit = Damage * (1 - percent);

                                    Targets[num_target] = piinow;

                                    float rand = Random.Range(0.2f,2f);

                                    //Наносим урон
                                    hit_piitarget(piinow, hit * rand);

                                    num_target = 1000;
                                }
                            }
                        }
                    }
                    

                }
                //Проверяем какие из этих целей нету еще в списке и наносим им урон
                

            }
            //Иначе радиус стал слишком большим отключаем взрыв
            else
            {
                boom_yn = false;
            }

            //Разширяем радиус
            size_boom_now = size_boom_now + Time.deltaTime * speed_boom;
        }
    }
    public void explosion_test2() {
        //Если взрыв происходит сейчас
        if (boom_yn) {
            //Debug.Log("Boom");
            //Если сейчас первый кадр взрыва
            if (size_boom_now == 0) {
                //Получаем ригид боди взрывателя если он есть
                Rigidbody rigidbody_main = gameObject.GetComponent<Rigidbody>();
                if (rigidbody_main != null)
                {
                    float mass_koof = 1 / rigidbody_main.mass;
                    rigidbody_main.velocity += new Vector3(0, Damage * 0.05f * mass_koof, 0);
                }

                //Получаем все обьекты как если бы взрыв уже произошел
                Collider[] hitTargets = Physics.OverlapSphere(gameObject.transform.position, size_boom_max);
                //Получаем из них все обьекты типа цель
                for (int num_colider = 0; num_colider < hitTargets.Length; num_colider++) {
                    PiiTarget target = hitTargets[num_colider].GetComponent<PiiTarget>();
                    //Если обьект типа цель
                    if (target != null && target.gameObject != gameObject && target_max < Targets.Length) {
                        //save
                        Targets[target_max] = target;
                        target_max++;
                    }
                }
                //Все обьекты сохранены теперь нужно их отсортировать в сторону увеличения дальности
                for (int num_cycle = 0; num_cycle < Targets.Length; num_cycle++) {
                    for (int num_target = 0; num_target+1 < Targets.Length && Targets[num_target+1] != null; num_target++)
                    {
                        float now = Vector3.Distance(gameObject.transform.position, Targets[num_target].transform.position);
                        float next = Vector3.Distance(gameObject.transform.position, Targets[num_target + 1].transform.position);
                        //Если настояшая цель больше предыдущей
                        if (now > next) {
                            //Меняем их местами
                            PiiTarget now_targ = Targets[num_target];
                            Targets[num_target] = Targets[num_target + 1];
                            Targets[num_target + 1] = now_targ;
                        }
                    }
                }

                //Если игроку важно получить урон
                if (PlayerDamage) {
                    //Вытаскиваем игрока
                    Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
                    float distPlayer = Vector3.Distance(player.transform.position, gameObject.transform.position);
                    if (distPlayer < size_boom_max) {
                        float percent = distPlayer / size_boom_max;
                        float hit = Damage * (1 - percent);

                        if (player.ScoreTime > 0)
                        {
                            //Нужно ли выводить эмоцию урона
                            if (hit > 100) {
                                player.faceController.set_butthurt_voice();
                            }

                            player.ScoreTime -= hit;
                            if (player.ScoreTime < 0) {
                                player.ScoreTime = 0;
                            }

                            //Игрок получил урон теперь надо высветить окно урона
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
                                player.damageWindow.set_all_need(0, 0, need_rot_x * -1f, 0);
                            else player.damageWindow.set_all_need(0, 0, 0, need_rot_x * 1f);

                            if (need_rot_y < 0)
                                player.damageWindow.set_all_need(need_rot_y * -1f, 0, 0, 0);
                            else player.damageWindow.set_all_need(0, need_rot_y * 1f, 0, 0);
                        }

                        PlayerDamage = false;
                    }
                }
            }
            size_boom_now = size_boom_now + Time.deltaTime * speed_boom;

            //Проверяем цель на взрыв если они есть
            if (target_now < target_max && size_boom_max != 0)
            {
                //Цикл пока не будет последняя цель в радиусе поражения
                bool end_yn = false;
                for (; end_yn == false;)
                {
                    //Проверяем растояние до настоящей цели которая сейчас выделена
                    float size_for_target = 99999999;
                    //Если обьекты есть
                    if (target_now < Targets.Length)
                    {
                        if (gameObject != null && Targets[target_now] != null)
                        {
                            size_for_target = Vector3.Distance(gameObject.transform.position, Targets[target_now].gameObject.transform.position);
                        }
                        //Если взрыв прошел эту цель то взрываем и переключаемся на следуюшую
                        if (size_boom_now > size_for_target)
                        {
                            //Считаем какой урон нужно нанести
                            //процент расширенности
                            float percent = size_boom_now / size_boom_max;
                            float hit = Damage * (1 - percent);

                            float rand = Random.Range(0.2f, 2f);

                            //меняем цель
                            target_now++;

                            Targets[target_now - 1].TrigerPii(hit, null, null, hit_text);
                            //отталкиваем цель если можем
                            Rigidbody rigidbody = null;
                            if (Targets[target_now-1] != null)
                                rigidbody = Targets[target_now - 1].GetComponent<Rigidbody>();

                            if (rigidbody != null) {
                                //Чем массивнее тем слабее отталкивание
                                float mass_koof = 0;
                                if (rigidbody.mass != 0)
                                    mass_koof = 1 / rigidbody.mass;
                                float dist_koof = 0;
                                if (size_for_target != 0)
                                   dist_koof = 1 / size_for_target;
                                //считаем вектор отталкивания
                                Vector3 vector_damage = new Vector3(
                                    Targets[target_now - 1].transform.position.x - gameObject.transform.position.x,
                                    Targets[target_now - 1].transform.position.y - gameObject.transform.position.y,
                                    Targets[target_now - 1].transform.position.z - gameObject.transform.position.z);
                                vector_damage = Vector3.Normalize(vector_damage);

                                //Расчет силы оталкивания 
                                float inercia_strong = Damage * dist_koof * mass_koof;
                                //Если сила отталкивания не слабая
                                if (inercia_strong > 0.2f)
                                    rigidbody.velocity += vector_damage * inercia_strong;

                            }
                            //меняем цель
                            //target_now++;
                            //Наносим урон
                            //hit_piitarget(Targets[target_now-1], hit * rand);
                        }
                        //Иначе завершаем цикл в этом кадре
                        else
                        {
                            end_yn = true;
                        }
                    }
                    else {
                        end_yn = true;
                    }

                }

            }
            //Если цели закончелись взрыв завершен
            else {
                boom_yn = false;

            }
        }
    }

    private void hit_piitarget(PiiTarget target, float hit) {
        target.TrigerPii(hit, null, null, hit_text);
        /*
        if (target.heath > 0) {
            target.heath = target.heath - hit;
            if (target.heath <= 0) {
                target.DeathTarget();
            }
        }
        */
    }

    private void create_audiosource() {
        if (audioSource == null) {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.priority = 30;
            //audioSource.volume = 0.8f * seting.game.volume_all * seting.game.volume_sound;
            audioSource.spatialBlend = 0.8f;

            audioSource.minDistance = 5;
            audioSource.maxDistance = 5000;
        }
    }
}
