using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiiPrefab : MonoBehaviour {

    public FaceController faceController;

    static int OldID = 0;
    public int ID = 0;
    static int particle_play_now = 0;
    bool particle_this_play_yn = false;

    public PiiController piiController;

    private Player player;

    private float TimeLive = 0;
    private float TimeDie = 5;
    public float Presure = 0;

    private const float speed_coof = 3;

    PiiSound soundObject;
    bool firstTriger = true;

    private ParticleSystem Pii_particle;
    private float distance_player;

    [SerializeField]
    Score_text_3d score_Text_3D;

    bool TestToDestroyTarget = false;

    Rigidbody rigidbody;
    bool smallSpeed = false;
    void iniRigidBody() {
        if (rigidbody == null) {
            rigidbody = gameObject.GetComponent<Rigidbody>();
        }
    }

    // Use this for initialization
    void Start()
    {
        OldID++;
        ID = OldID;

        //Получаем систему частиц
        Pii_particle = gameObject.GetComponentInChildren<ParticleSystem>();
        if (Pii_particle != null) {
            Pii_particle.Pause();
        }

        iniRigidBody();

        charge_on = piiController.ChargeOn;

    }


    // Update is called once per frame
    void Update () {
        TimeTest();
        SizeTest();

    }

    //Проверка времени жизни
    void TimeTest()
    {
        TimeLive += Time.deltaTime;
        if (TimeLive > TimeDie) {
            Destroy(gameObject);
        }

        //Проверка ригид боди
        if (rigidbody != null) {
            if (!smallSpeed && rigidbody.velocity.magnitude < 2) {
                smallSpeed = true;

                TimeDie = TimeLive + 2;
            }
        }
    }

    bool charge_on = false;
    void SizeTest() {
        distance_player = Vector3.Distance(gameObject.transform.position, player.transform.position);

        //Меняем размер мочи
        if (charge_on)
        {
            gameObject.transform.localScale = new Vector3(distance_player * 1f, distance_player * 1f, distance_player * 1f);
        }
        else {
            gameObject.transform.localScale = new Vector3(distance_player * 0.5f, distance_player * 0.5f, distance_player * 0.5f);
        }
    }
    void ParticleTest() {
        if (Pii_particle != null)
        {
            //Если факт не соответствует ожиданиям
            if (!Pii_particle.isPlaying && particle_this_play_yn)
            {
                particle_this_play_yn = false;
                particle_play_now--;
            }
            else if (Pii_particle.isPlaying && !particle_this_play_yn)
            {
                particle_this_play_yn = true;
                particle_play_now++;
            }
        }
    }

    public void SetPosition(Vector3 pos) {
        gameObject.transform.position = pos;
    }

    public void SetSpeed(Quaternion quaternion_func, float speed) {

        //Корректируем градус
        Quaternion quaternion_new = new Quaternion(quaternion_func.x, quaternion_func.y, quaternion_func.z, quaternion_func.w);
        Rigidbody rb;
        rb = gameObject.GetComponent<Rigidbody>();


        float coof_x =
            Mathf.Sin(quaternion_new.eulerAngles.y / 57.2958f) * Mathf.Cos(quaternion_new.eulerAngles.x/57.2958f);
        float coof_y = 
            Mathf.Sin(quaternion_new.eulerAngles.x / 57.2958f) * -1;
        float coof_z =
            Mathf.Cos(quaternion_new.eulerAngles.y / 57.2958f) * Mathf.Cos(quaternion_new.eulerAngles.x / 57.2958f);

        rb.velocity = new Vector3(coof_x * speed_coof * speed, coof_y * speed_coof * speed, coof_z * speed_coof * speed);
    }


    public void SetPlayer(Player player_func) {
        player = player_func;
    }
    public void SetSoundPii(PiiSound piiSound) {
        soundObject = piiSound;
    }

    private void OnTriggerEnter(Collider other)
    {
        PiiTarget target = other.GetComponent<PiiTarget>();
        if (target != null) {
            target.SetTimeToDelete(20);
        }
        if (target != null && target.heath > 0 && target.MinPresure < Presure) {
            target.TrigerPii(Presure * 1.1f, faceController, piiController, score_Text_3D.gameObject);
            player.percent_heath_last_target = target.heath / (target.get_start_heath() / 100);

            //Если в это время активны частицы
            if (Pii_particle != null && Pii_particle.isPlaying) {
                particle_play_now--;
                particle_this_play_yn = false;
                Pii_particle.Stop();
            }

            Destroy(gameObject);

        }
        //Если до урона не хватает совсем чучуть
        else if (!TestToDestroyTarget && target != null && target.heath > 0 && target.MinPresure > Presure && (target.MinPresure - target.MinPresure/3) < Presure) {
            //Проверка на урон произведена
            TestToDestroyTarget = true;
            target.TrigerPiiFail(score_Text_3D.gameObject);
        }
        else if (target == null) {
            if (Pii_particle != null && Pii_particle.isPlaying != true && particle_play_now < 3) {
                ParticleSystem.MainModule mainmod = Pii_particle.main;
                mainmod.startSize = Presure * 0.05f;
                mainmod.startSpeed = Presure * 0.1f;

                ParticleSystem.ShapeModule shapemod = Pii_particle.shape;
                shapemod.radius = distance_player * 0.05f;

                Pii_particle.Play();
            }
        }

        if (firstTriger) {
            TimeDie = TimeLive + 1.5f;
            firstTriger = false;
            soundObject.set_new_pii(gameObject.GetComponent<PiiPrefab>());
        }
    }
}
