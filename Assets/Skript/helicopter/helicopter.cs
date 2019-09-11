using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class helicopter : MonoBehaviour {

    setings seting;
    Player player;

    [SerializeField]
    GameObject main_rotor;
    [SerializeField]
    GameObject small_rotor;

    [SerializeField]
    AudioClip mainRotorClip;
    AudioSource mainRotor;

    PiiTarget piiTarget;
    Rigidbody rigidbody;

    float rotate_speed = 10;

    float height_fly = 60;
    float radius_fly_triger = 15;

    bool move_down_now_yn = false;
    float position_y_old = 0;

    [SerializeField]
    GameObject TargetMove;

	// Use this for initialization
	void Start () {
        rigidbody = GetComponent<Rigidbody>();
        piiTarget = GetComponent<PiiTarget>();

        ini_setings();
        ini_player();
    }
	
	// Update is called once per frame
	void Update () {
        test_sound_rotor();
        if (piiTarget != null && piiTarget.heath > 0)
        {
            test_faling();
            UpDownMove();
            calcRotateToMove();
        }

    }

    //Функция поддерживаюшая тягу вертолета
    void UpDownMove() {
        //Получаем то куда смотит обьект

        //Пускаем лучи 
        //в низ
        Ray down = new Ray(transform.position + Vector3.up * radius_fly_triger, Vector3.down);
        RaycastHit downHit;
        GameObject down_obj = null;
        if(Physics.SphereCast(down, radius_fly_triger, out downHit))
            down_obj = downHit.collider.gameObject;

        //В направлении движения XZ
        Vector3 faceVec = new Vector3(gameObject.transform.forward.x, gameObject.transform.forward.y, gameObject.transform.forward.z);
        faceVec = Vector3.Normalize(faceVec);
        Ray moveXZ = new Ray(transform.position - radius_fly_triger * faceVec, faceVec);
        RaycastHit faceXZHit;
        GameObject face_obj = null;
        if (Physics.SphereCast(moveXZ, radius_fly_triger, out faceXZHit))
            face_obj = faceXZHit.collider.gameObject;

        bool down_triger = false;
        
        if ((downHit.distance < height_fly && down_obj != null) || 
            faceXZHit.distance < radius_fly_triger * 2 && face_obj != null)
        {
            down_triger = true;
        }

        float speed_rotate_rotor = 10;

        //Узнаем растояние от вертолета до цели
        float distToTarget = Vector3.Distance(gameObject.transform.position, TargetMove.transform.position);

        if (gameObject.transform.position.y < (40 + distToTarget/5)) {
            down_triger = true;
            move_down_now_yn = true;
        }

        //Если тригер сработал запускаем двигатель
        if (down_triger)
        {
            float dist_percent = 0;
            float dist_percent_down = height_fly / downHit.distance;
            if (dist_percent < dist_percent_down && down_obj != null) {
                dist_percent = dist_percent_down;
            }
            float dist_percent_face = height_fly / faceXZHit.distance;
            if (dist_percent < dist_percent_face && face_obj != null) {
                dist_percent = dist_percent_face;
            }

            if (dist_percent > 3) dist_percent = 3;
            //Если падает
            if (move_down_now_yn || rigidbody.velocity.magnitude < 2)
            {
                rigidbody.velocity = (rigidbody.velocity + transform.up * rotate_speed * dist_percent * 0.5f * Time.deltaTime);
                speed_rotate_rotor = rotate_speed * dist_percent * 0.5f * Time.deltaTime;
            }


        }
        else {
            //Если падает
            if (move_down_now_yn && rigidbody.velocity.magnitude > 2)
            {
                rigidbody.velocity = (rigidbody.velocity + transform.up * rotate_speed * Time.deltaTime * 0.5f);
                speed_rotate_rotor = rotate_speed * 0.5f * Time.deltaTime;
            }

        }

        if (mainRotor != null) {
            mainRotor.pitch = 1 + (speed_rotate_rotor / 15);
        }

        //Вращение лопостей
        if (main_rotor != null)
            main_rotor.transform.Rotate(0, 80 * 1 * speed_rotate_rotor, 0);
        if (small_rotor != null)
            small_rotor.transform.Rotate(80 * 3 * speed_rotate_rotor, 0, 0);
    }

    void ini_setings() {
        if (seting == null) {
            seting = GameObject.FindGameObjectWithTag("setings_game").GetComponent<setings>();
        }
    }
    void ini_player() {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    //Тест на падение
    void test_faling() {
        //Проверка взлет или падение
        if (position_y_old > gameObject.transform.position.y)
        {
            move_down_now_yn = true;
        }
        else {
            move_down_now_yn = false;
        }

        position_y_old = gameObject.transform.position.y;
    }

    void test_sound_rotor() {
        if (seting != null && player != null) {
            if (mainRotor != null)
            {
                mainRotor.volume = seting.game.volume_all * seting.game.volume_sound;

                //Проверка растояния и регулировка громкости
                float distance = Vector3.Distance(gameObject.transform.position, player.gameObject.transform.position);
                float max_dist = 250;
                float min_dist = 50;
                if (distance > max_dist+min_dist)
                {
                    mainRotor.spatialBlend = 1;
                }
                else
                {
                    mainRotor.spatialBlend = (distance-min_dist) / max_dist;
                }



                if (gameObject.GetComponent<PiiTarget>().heath > 0 && !mainRotor.isPlaying)
                {
                    mainRotor.Play();
                }
                else if(gameObject.GetComponent<PiiTarget>().heath <= 0 && mainRotor.isPlaying)
                {
                    mainRotor.Stop();
                }
            }
            else if(mainRotor == null) {
                mainRotor = gameObject.AddComponent<AudioSource>();
                mainRotor.clip = mainRotorClip;
                mainRotor.priority = 10;
                mainRotor.loop = true;
                mainRotor.spatialBlend = 0.99f;
                mainRotor.minDistance = 1;
                mainRotor.maxDistance = 1500;
            }
        }
    }

    public void setNewTarget(GameObject NewTarget) {
        TargetMove = NewTarget;
    }

    //наклон в сторону цели
    void calcRotateToMove() {
        if (TargetMove != null && rigidbody != null)
        {
            float speed = 1;
            //Меняем скорость в зависимости от растояния до обьекта
            float distance = Vector3.Distance(new Vector3(TargetMove.transform.position.x, 0, TargetMove.transform.position.z), new Vector3(gameObject.transform.position.x, speed, gameObject.transform.position.z));
            

            float speed2vec = Vector3.Distance(new Vector3(0,0,0), new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z));

            if ((speed2vec > 1 && distance < 10) || (speed2vec < 1 && distance > 10))
                speed = (distance/ (speed2vec * Time.deltaTime)) - (((speed2vec * Time.deltaTime * 500)) / distance);


            if (speed > 20)
            {
                speed = 20;
            }
            else if (speed < -20)
            {
                speed = -20;
            }
            speed = speed * distance* 0.05f;

            Quaternion rotate_now = new Quaternion(gameObject.transform.rotation.x, gameObject.transform.rotation.y, gameObject.transform.rotation.z, gameObject.transform.rotation.w);

            Quaternion rotate_to = Quaternion.LookRotation(new Vector3(TargetMove.transform.position.x, 0, TargetMove.transform.position.z) - new Vector3(gameObject.transform.position.x, speed, gameObject.transform.position.z));
            float old_y = transform.rotation.y;
            gameObject.transform.rotation = Quaternion.Lerp(transform.rotation, rotate_to, Time.deltaTime);
            
        }
    }
}
