using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudrantPartCTRL : MonoBehaviour {

    [SerializeField]
    ParticleSystem MainParticle;

    float beginSpeed = 20;
    float beginSize = 10;
    float complite = 0;
    float dist = 0;

    [SerializeField]
    AudioClip[] audioClip;

    setings seting;
    AudioSource audioSource1;
    AudioSource audioSource2;
    void iniSound() {
        if (seting == null) {
            seting = GameObject.FindGameObjectWithTag("setings_game").GetComponent<setings>();
        }
        if (seting != null && audioSource1 == null && audioSource2 == null && audioClip != null && audioClip.Length > 0) {
            audioSource1 = gameObject.AddComponent<AudioSource>();
            audioSource2 = gameObject.AddComponent<AudioSource>();

            Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            dist = Vector3.Distance(player.gameObject.transform.position, gameObject.transform.position);

            float distCoof = dist / 500;
            if (distCoof > 1) distCoof = 1;
            audioSource1.priority = (int)(distCoof * 127);
            audioSource2.priority = (int)(distCoof * 127);

            audioSource1.volume = seting.game.volume_all * seting.game.volume_sound;
            audioSource2.volume = seting.game.volume_all * seting.game.volume_sound;

            audioSource1.spatialBlend = 0.8f;
            audioSource2.spatialBlend = 0.8f;

            audioSource1.minDistance = 2;
            audioSource2.minDistance = 2;

            audioSource1.maxDistance = 1000;
            audioSource2.maxDistance = 1000;

        }
    }


    // Use this for initialization
    void Start() {
        if (MainParticle != null && !MainParticle.isPlaying)
        {
            MainParticle.Play();
            //beginSpeed = MainParticle.startSpeed;
            //beginSize = MainParticle.startSize;

        }

        iniSound();

    }

    // Update is called once per frame
    void Update() {
        TestSpeed();
        TestSound();
        TestUpObj();
    }

    //проверка стартовой скорости от времени жизни
    void TestSpeed() {
        if (MainParticle != null && MainParticle.isPlaying && !MainParticle.loop) {
            complite = MainParticle.time / MainParticle.duration;
            MainParticle.startSpeed = beginSpeed * (1 - complite);
            MainParticle.startSize = beginSize * (1 - complite);


        }
        if (MainParticle != null && !MainParticle.isPlaying) {
            GameObject.Destroy(gameObject);
        }
    }

    //Тест звука
    void TestSound() {
        if (audioSource1 != null && audioSource2 != null && audioClip != null && audioClip.Length > 0) {

            float intensiv = 1 - complite; 

            //Сперва узнаем силу от 0 до 1
            //Узнаем деление одного звука
            float delenieOne = 1.0f / audioClip.Length;
            //Узнаем количество целых делений сейчас
            int deleniiNow = (int)(intensiv / delenieOne);

            //Debug.Log("delenieNow " + deleniiNow);

            //громкость смешивания
            float blend1 = 0;
            float blend2 = 0;

            if (deleniiNow < 1)
            {
                blend1 = 0;
                blend2 = (intensiv - (delenieOne * deleniiNow)) / delenieOne;
                if (audioSource2.clip != audioClip[0]) {
                    audioSource2.clip = audioClip[0];
                    audioSource1.clip = null;

                    audioSource2.Stop();
                    audioSource2.Play();

                    audioSource1.Stop();
                }

                audioSource2.loop = true;
            }
            else{
                blend2 = (intensiv - (delenieOne * deleniiNow)) / delenieOne;
                blend1 = 1 - blend2;
                if (audioSource1.clip != audioClip[deleniiNow - 1] || audioSource2.clip != audioClip[deleniiNow]) {
                    audioSource1.clip = audioClip[deleniiNow - 1];

                    if (deleniiNow < audioClip.Length)
                        audioSource2.clip = audioClip[deleniiNow];

                    audioSource1.Stop();
                    audioSource2.Stop();

                    audioSource1.Play();
                    audioSource2.Play();
                }

                audioSource1.loop = true;
                audioSource2.loop = true;
            }

            audioSource1.volume = seting.game.volume_all * seting.game.volume_sound * blend1;
            audioSource2.volume = seting.game.volume_all * seting.game.volume_sound * blend2;


        }
    }

    //тест поднятия обьекта напором вверх
    void TestUpObj() {
        //получение бокса поднятия
        //получение всех обьектов внутри проверяемой зоны
        Collider[] colliders = Physics.OverlapBox(gameObject.transform.position, new Vector3(1, (beginSpeed * (1 - complite))/6, 1));

        if (colliders != null && colliders.Length > 0) {
            for (int numColl = 0; numColl < colliders.Length; numColl++) {
                Rigidbody rigidbody = colliders[numColl].GetComponent<Rigidbody>();
                if (rigidbody != null) {
                    float EndSpeed = 1 - complite;
                    rigidbody.velocity = rigidbody.velocity * Time.deltaTime * 2 + gameObject.transform.up * (beginSpeed * EndSpeed);

                    Quaternion speedRot = rigidbody.rotation;
                    speedRot.eulerAngles += new Vector3(Time.deltaTime * 20 * EndSpeed * Random.Range(-1, 1),Time.deltaTime * 20 * EndSpeed * Random.Range(-1, 1), Time.deltaTime * 20 * EndSpeed * Random.Range(-1, 1));
                    rigidbody.rotation = speedRot;
                }
            }
        }
    }
}
