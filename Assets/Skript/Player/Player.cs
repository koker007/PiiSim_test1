using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour {

    setings seting;
    [SerializeField]
    public GameObject menu;

    [SerializeField]
    bool unity_redactor_yn = true;

    public bool mouse_control_on = true;

    [SerializeField]
    public GameObject camera;
    [SerializeField]
    public GameObject bolt;
    [SerializeField]
    private GameController gameController;

    [SerializeField]
    public FaceController faceController;

    [SerializeField]
    public GameplayParametrs gameplayParametrs;

    [SerializeField]
    private Transform player_pos_1;
    [SerializeField]
    private Transform player_pos_2;
    [SerializeField]
    private Transform player_pos_3;

    //Цель к которой двигается игрок
    private Transform target_move;
    private Transform Start_move;
    private float speed_Move;
    private float start_move_time;
    private float travelLenght;
    private bool travel2 = false;

    InerciaPii inerciaPii = new InerciaPii();
    public float ViewAngle_x = 0;
    public float ViewAngle_y = 0;

    public float ViewCamMinus = 0;

    public float MouseSpeed = 1;
    public bool old_mouse_lock = false;

    public float maxPresure = 20;
    [SerializeField]
    public float coof_presure_plus = 0.05f;

    //Общее количество очков
    public float ScoreALL = 0;
    public float ScoreCheats = 0;
    public void ScorePlusCheat(float plus) {
        ScoreALL += plus;
        ScoreCheats += plus;
    }
    //Количество очков влияющее на напор
    public float ScoreTime = 100;

    private float ComboSum = 0;
    private float ComboTime = 0;

    //Проценты здоровья последнего атакованного обьекта
    public float percent_heath_last_target = 0;

    //Коофициекн напора
    public float coof_pressure = 0.1f;

    //Сыт ли сейчас
    public bool PiiNow = false;

    private float view_angle_old_x = 0;
    private float view_angle_old_y = 0;

    [SerializeField]
    lvl_text lvl_Text;

    [SerializeField]
    public steam_achievement steam_Achievement;

    public DamageWindow damageWindow;
    public DamageWindow presureWindow;
    public DamageWindow endViewWindow;
    public DamageWindow endViewWindowFull;

	// Use this for initialization
	void Start () {
        move_to_1();
        set_seting();
        set_all_window();
    }

	
	// Update is called once per frame
	void Update () {
        StartPiiTest();
        ViewAngleCalc();

        Calculation_score_time();

        Test_move_2();
        moving_test();
        TestCombo();
        testViewAngle();
        TestLocalPosCam();
    }

    void StartPiiTest() {
        if (!PiiNow && Input.GetMouseButtonDown(0) && !menu.active && !gameplayParametrs.GameOver) {
            //Если это не демо версия
            //if (gameplayParametrs != null && !gameplayParametrs.ThisDemoVersion) {
                PiiNow = true;
                gameplayParametrs.start_lvl_1();
            //}

            //if (lvl_Text != null) {
            //    lvl_Text.text_next = "LVL I";
            //}
        }
    }

    void set_seting()
    {
        //Если настроек нет
        if (seting == null)
        {
            //ищем обьект по тегу
            GameObject obj = GameObject.FindWithTag("setings_game");
            //вытаскиваем настройки
            if (obj != null) seting = obj.GetComponent<setings>();
        }
    }
    void set_all_window() {
        if (damageWindow == null) damageWindow = GameObject.FindWithTag("damage_window").GetComponent<DamageWindow>();
        if (presureWindow == null) presureWindow = GameObject.FindWithTag("presure_window").GetComponent<DamageWindow>();
        if (endViewWindow == null) endViewWindow = GameObject.FindWithTag("end_view_window").GetComponent<DamageWindow>();
        if (endViewWindowFull == null) endViewWindowFull = GameObject.FindWithTag("end_view_window_full").GetComponent<DamageWindow>();
    }

    public void PlusScore(float score) {
        ScorePlusCheat(score);
        //ScoreALL += score;
        ScoreTime += score;

        PiiController pii = bolt.GetComponent<PiiController>();
        if (pii.ChargeNow < pii.ChargeMax) {
            pii.ChargeNow += 0.1f;
        }
    }

    //Повернуть камеру на указанный угл
    public void ViewAngleCalc() {
        //Сперва проверяем активен ли гейм плей и Активно ли управление мышью
        if (gameController.GamePlay_yn && gameController.Mouse_control_yn && gameplayParametrs != null && !gameplayParametrs.pause)
        {

            //блочим курсор
            if (Cursor.lockState != CursorLockMode.Locked && mouse_control_on)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else if (!mouse_control_on && old_mouse_lock == true) {
                old_mouse_lock = false;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            float dx = Input.GetAxis("Mouse X");
            float dy = Input.GetAxis("Mouse Y");

            PiiController piiController = bolt.GetComponent<PiiController>();

            //меняем угл просмотра
            if (seting == null && mouse_control_on)
            {
                ViewAngle_y += dx * MouseSpeed * Time.deltaTime;
                ViewAngle_x -= dy * MouseSpeed * Time.deltaTime;
            }
            else if (seting != null && seting.game != null && mouse_control_on)
            {
                if (!piiController.ChargeOn)
                {
                    ViewAngle_y += dx * seting.game.speedController * 0.5f;
                    ViewAngle_x -= dy * seting.game.speedController * 0.5f;
                }
                else{
                    ViewAngle_y += dx * seting.game.speedController * Time.deltaTime;
                    ViewAngle_x -= dy * seting.game.speedController * Time.deltaTime;
                }
            }

            //Debug.Log(mouse_pos_old_y + "  " + Input.mousePosition.y);
            //Cursor.lockState = CursorLockMode.None;

            float view_angle_new_x = ViewAngle_x - (view_angle_old_x - ViewAngle_x)/1.1f;
            float view_angle_new_y = ViewAngle_y - (view_angle_old_y - ViewAngle_y)/1.1f;



            view_angle_old_x = ViewAngle_x;
            view_angle_old_y = ViewAngle_y;


            //ViewAngle_x = view_angle_new_x;
            //ViewAngle_y = view_angle_new_y;

            //PiiController piiController = bolt.GetComponent<PiiController>();
            //Расчет постоянного смещения
            //inerciaPii.NewCalc();
            float piicoof = piiController.speed_pii / 10;
            if (piicoof > 1)
                piicoof = 1;
            if (piiController.ChargeOn)
            {
                inerciaPii.NewCalc();
                ViewAngle_x += (inerciaPii.Smehenie.x / 6) * piicoof;
                ViewAngle_y += (inerciaPii.Smehenie.y / 6) * piicoof;

                ViewAngle_x = view_angle_new_x;
                ViewAngle_y = view_angle_new_y;
            }
            else {
                //ViewAngle_x += (inerciaPii.Smehenie.x / 3) * piicoof;
                //ViewAngle_y += (inerciaPii.Smehenie.y / 3) * piicoof;
            }

            //}


            //Проверяем границы
            if (ViewAngle_x > 50)
                ViewAngle_x = 50;
            else if (ViewAngle_x < -60)
                ViewAngle_x = -60;

            //Границы по бокам ограничиваем ускорением в противоположную сторону
            float ViewAngle_y_max = 85;
            if (ViewAngle_y > ViewAngle_y_max)
            {
                //ViewAngle_y = 50;
                float raznica = ViewAngle_y - ViewAngle_y_max;
                if (!piiController.ChargeOn)
                    ViewAngle_y -= raznica / 2 * Time.deltaTime * 5;
                else ViewAngle_y -= raznica / 2 * Time.deltaTime;
                endViewWindow.set_all_need(0, raznica/30,0,0);
            }
            else if (ViewAngle_y < -1 * ViewAngle_y_max)
            {
                //ViewAngle_y = -50;
                float raznica = ViewAngle_y + ViewAngle_y_max;
                if(!piiController.ChargeOn)
                    ViewAngle_y -= raznica/2 * Time.deltaTime * 5;
                else ViewAngle_y -= raznica / 2 * Time.deltaTime;
                endViewWindow.set_all_need(-1 * raznica / 30, 0, 0, 0);
            }

            //Меняем данные у обьектов
            gameObject.transform.eulerAngles = new Vector3(0, ViewAngle_y);

            float presure = bolt.GetComponent<PiiController>().speed_pii;

            camera.transform.eulerAngles = new Vector3(ViewAngle_x + ViewCamMinus, ViewAngle_y);
            bolt.transform.eulerAngles = new Vector3(ViewAngle_x + presure*0.4f, ViewAngle_y);

            //Визуализация лицевого индикатора
            if (faceController != null)
                faceController.set_face_grad(ViewAngle_y);

        }
        else if (gameplayParametrs.pause) {
            if (!mouse_control_on && old_mouse_lock == true || Cursor.lockState == CursorLockMode.Locked)
            {
                old_mouse_lock = false;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }

    public void Calculation_score_time() {
        if (gameController.GamePlay_yn) {
            if (ScoreTime > 0) {
                ScoreTime -= Time.deltaTime * 0.35f;
            }
        }
    }

    private void Test_move_2() {
        //Если возможность переключения есть и напор достаточный
        if (!travel2 && ScoreALL > 250) {
            travel2 = true;

            move_to_2();
        }
    }

    public void move_to_1() {
        if (player_pos_1 != null) {
            Start_move = gameObject.transform;
            target_move = player_pos_1;

            speed_Move = 2f;
            start_move_time = Time.time;
            travelLenght = Vector3.Distance(Start_move.position, target_move.position);
        }
    }

    public void move_to_2() {
        if (player_pos_2 != null) {
            Start_move = gameObject.transform;
            target_move = player_pos_2;

            speed_Move = 0.0003f;

            start_move_time = Time.time;

            travelLenght = Vector3.Distance(Start_move.position, target_move.position);
        }
    }
    public void move_to_3() {
        if (player_pos_3 != null)
        {
            Start_move = gameObject.transform;
            target_move = player_pos_3;

            speed_Move = 0.0008f;

            start_move_time = Time.time;

            travelLenght = Vector3.Distance(Start_move.position, target_move.position);
        }
    }

    private void moving_test() {
        //Если цель движения есть то двигаемся
        if (target_move != null && travelLenght != 0 && gameplayParametrs != null && !gameplayParametrs.pause) {
            //Если скорость нулевая
            if (speed_Move <= 0.0f) {
                speed_Move = 0.1f;
            }

            //Перемещенное растояние
            float distCovered = (Time.time - start_move_time) * speed_Move;

            float fracJourney = distCovered / travelLenght;

            if (fracJourney >= 1)
            {
                gameObject.transform.position = target_move.position;
                target_move = null;
                Start_move = null;
                start_move_time = 0;
                speed_Move = 0;
            }
            else {
                gameObject.transform.position = Vector3.Lerp(Start_move.position, target_move.position, fracJourney);
            }
        }
    }

    void testViewAngle() {
        PiiController piiController = bolt.GetComponent<PiiController>();
        Camera cam = camera.GetComponent<Camera>();
        if (piiController != null && cam != null) {
            if (piiController.ChargeOn && cam.fieldOfView != 65) {
                cam.fieldOfView += (65 - cam.fieldOfView) * Time.deltaTime;
            }
            else if (!piiController.ChargeOn && cam.fieldOfView != 60) {
                cam.fieldOfView += (60 - cam.fieldOfView) * Time.deltaTime;
            }
        }
    }

    private void TestCombo() {
        //Проверка времени комбы
        if (ComboTime <= 0) {
            ComboSum = 0;
        }

        //Если комбо набралось
        if (ComboSum >= 3 && faceController != null) {
            faceController.set_agress(ComboTime);
        }
        

        //Уменьшаем время комбы
        ComboTime -= Time.deltaTime;
        if (ComboTime < 0)
            ComboTime = 0;
    }
    public void PlusCombo() {
        //Прибавляем комбу обновляем время
        ComboSum++;
        ComboTime = 3;
    }

    public void SetFullCharge() {
        PiiController piiController = bolt.GetComponent<PiiController>();
        if (piiController != null && piiController.ChargeNow < piiController.ChargeMax) {
            piiController.ChargeNow = piiController.ChargeMax;
        }
    }

    //Локальное смещение камеры по оси x
    float old_rot_y = 0;
    void TestLocalPosCam() {
        //Возвращение камеры в нулевую позицию
        float needToZero = 0 - camera.transform.localPosition.x;
        Vector3 newPosition = camera.transform.localPosition;
        newPosition.x += (needToZero / 2) * Time.deltaTime;

        //Смещяем камеру от центра в зависимости от того какие изменения
        float izmenenie = old_rot_y - gameObject.transform.localRotation.ToEulerAngles().y;
        old_rot_y = gameObject.transform.localRotation.ToEulerAngles().y;

        PiiController piiController = bolt.GetComponent<PiiController>();
        if (piiController.ChargeOn)
        {
            newPosition.x -= (izmenenie / 100) * piiController.speed_pii;
            if (newPosition.x > 0.5f)
            {
                newPosition.x = 0.5f;
            }
            else if (newPosition.x < -0.5f)
            {
                newPosition.x = -0.5f;
            }
            camera.transform.localPosition = newPosition;
        }
        else {
            newPosition.x -= (izmenenie / 500) * piiController.speed_pii;
            if (newPosition.x > 0.5f)
            {
                newPosition.x = 0.5f;
            }
            else if (newPosition.x < -0.5f)
            {
                newPosition.x = -0.5f;
            }
            camera.transform.localPosition = newPosition;
        }
    }
}
