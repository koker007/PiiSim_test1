using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TsunamiPartWave : MonoBehaviour {

    [SerializeField]
    bool ThisTarget = false;
    [SerializeField]
    float smehenie = 30;

    [SerializeField]
    float DamageTargets = 500;

    //скорость мусора толкаемого цунами
    [SerializeField]
    float SpeedTarget = 50;

    float timeEndTsunami = 0;

    PiiTarget piiTarget;
    void iniPiiTarget() {
        if (piiTarget == null) {
            piiTarget = GetComponent<PiiTarget>();
        }
        if (piiTarget != null && !ThisTarget) {
            Destroy(piiTarget);
        }
    }

    CapsuleCollider capsuleCollider;
    void iniCapsuleColider() {
        if (capsuleCollider == null) {
            capsuleCollider = GetComponent<CapsuleCollider>();
        }
    }
    Material material;
    void iniMaterial() {
        if (material == null) {
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            Material[] materials = null;
            if (meshRenderer != null) {
                materials = meshRenderer.materials;
            }
            if (materials != null && materials.Length > 0) {
                material = materials[0];
            }
        }
    }
    setings setings;
    void iniSetings() {
        if (setings == null) {
            setings = GameObject.FindGameObjectWithTag("setings_game").GetComponent<setings>();
        }
    }
    GameplayParametrs gameParam;
    void iniGameParam() {
        if (gameParam == null) {
            gameParam = GameObject.FindGameObjectWithTag("setings_game").GetComponent<GameplayParametrs>();
        }
    }

    Vector3 StartLocPos;
    void iniStartLocPos() {
        if (StartLocPos == null) {
            StartLocPos = gameObject.transform.localPosition;
        }
    }

    bool playerDamage = false;
    Player player;
    int NumHit = 0;
    void iniPlayer() {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    [SerializeField]
    AudioClip[] mainWaterSound;
    AudioSource audioSource;
    void iniAudioSource() {
        if (audioSource == null && mainWaterSound != null && mainWaterSound.Length > 0 && setings != null) {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

	// Use this for initialization
	void Start () {
        iniSetings();
        iniGameParam();
        iniPiiTarget();
        iniPlayer();
        //iniCapsuleColider();
        iniMaterial();
        iniStartLocPos();
        iniAudioSource();
    }
	
	// Update is called once per frame
	void Update () {
        TestRigidBody();
        TestTextureRot();
        TestLocalPosition();
        TestSound();
        TestPlayerDist();
    }

    //Проверка на столкновения
    void TestRigidBody() {
        if (true) {
            Collider[] collider = null;
            //вытаскиваем колайдеры если их нет
            if (collider == null || collider.Length <= 0)
            {
                //collider = Physics.OverlapCapsule(gameObject.transform.position - gameObject.transform.localScale.y * transform.up/2, gameObject.transform.position + gameObject.transform.localScale.y * transform.up/2, capsuleCollider.radius * 2 * transform.localScale.x);
                collider = Physics.OverlapCapsule(gameObject.transform.position - gameObject.transform.localScale.y * transform.up / 2, gameObject.transform.position + gameObject.transform.localScale.y * transform.up / 2, 0.3f * 2 * transform.localScale.x);
            }

            //Если теперь колайдеры есть то проверяем их
            if (collider != null && collider.Length > 0) {
                //Перебираем все колайдеры
                for (int numColl = 0; numColl < collider.Length; numColl++) {
                    //Пытаемся получить пи таргет из колайдера
                    PiiTarget colliderPiiTarget = collider[numColl].GetComponent<PiiTarget>();
                    if (colliderPiiTarget != null)
                    {
                        //Проверяем что это не цунами?
                        TsunamiPartWave tsunamiPartWave = colliderPiiTarget.GetComponent<TsunamiPartWave>();
                        if (tsunamiPartWave == null) {
                            //Это не цунами тогда наносим урон без прибавления очков игроку, если цель еще жива
                            if (colliderPiiTarget.heath > 0) {
                                colliderPiiTarget.heath -= DamageTargets * Time.deltaTime;

                                if (colliderPiiTarget.heath <= 0) {
                                    colliderPiiTarget.heath = 0;
                                    colliderPiiTarget.DeathTarget();
                                }
                            }
                        }

                        //Проверяем обьект на ригид боди
                        Rigidbody rigidbody = colliderPiiTarget.GetComponent<Rigidbody>();
                        if (rigidbody != null)
                        {
                            //Тестим на мочу
                            PiiPrefab piiPrefab = colliderPiiTarget.GetComponent<PiiPrefab>();
                            if (piiPrefab == null)
                            {
                                rigidbody.velocity = rigidbody.velocity * Time.deltaTime * 4 + (transform.parent.up * 3 + transform.parent.forward * -1).normalized * SpeedTarget * Time.deltaTime;
                                rigidbody.freezeRotation = false;
                            }
                        }
                        
                    }
                }
            }
        }
    }
    void TestTextureRot() {
        if (material != null) {
            Vector2 offset = material.mainTextureOffset;
            offset.x += Time.deltaTime * 0.05f;
            material.mainTextureOffset = offset;
        }
    }
    void TestLocalPosition() {
        if (piiTarget != null && StartLocPos != null) {
            Vector3 posPlus = new Vector3(0, -9.16f, 22.5f);
            //получаем растояние сейчас
            Vector3 posNeed = StartLocPos + (posPlus * (100 - piiTarget.heath_percent)/100);
            //Смещаем то что сейчас
            gameObject.transform.localPosition += (posNeed - gameObject.transform.localPosition) *Time.deltaTime;
        }
    }
    void TestSound() {
        if (audioSource != null && !audioSource.isPlaying) {
            //audioSource.volume = setings.game.volume_all * setings.game.volume_sound;
            audioSource.priority = 2;
            audioSource.pitch = Random.Range(0.95f,1.05f);
            audioSource.spatialBlend = 1;
            audioSource.minDistance = 10;
            audioSource.maxDistance = 3000;

            audioSource.loop = true;
            audioSource.clip = mainWaterSound[Random.Range(0, mainWaterSound.Length)];
            audioSource.Play();
            //audioSource.PlayOneShot(mainWaterSound[Random.Range(0, mainWaterSound.Length)]);
        }
        if (audioSource != null && setings != null && gameParam != null) {

            //Расчитываем время с завершения игры
            if (gameParam.GameOver || gameParam.get_lvl_now() >= 6) {
                timeEndTsunami += Time.deltaTime;
            }

            float timeMinVolume = 20;

            float volumeCoof = 1;
            if (timeEndTsunami <= 0) {
                volumeCoof = 1;
            }
            else if (timeEndTsunami < timeMinVolume) {
                volumeCoof = 1 - (timeEndTsunami / timeMinVolume);
            }
            else{
                volumeCoof = 0;
            }

            if (setings != null && setings.game != null)
            {
                audioSource.volume = setings.game.volume_all * setings.game.volume_sound * volumeCoof;
            }
        }
    }
    void TestPlayerDist() {
        //Проверка дистанции до игрока чтобы нанести ему урон и он проиграл
        if (!playerDamage && player != null && gameParam != null) {
            float playerDist = Vector3.Distance(transform.position, player.transform.position);
            if ((playerDist < 30 || NumHit > 0) && gameParam.get_lvl_now() == 5) {


                if (NumHit < 60)
                {
                    if (NumHit == 0)
                    {
                        player.faceController.set_choked_voice();
                    }
                    player.damageWindow.set_all_need(1, 1, 1, 1);
                    player.endViewWindow.set_all_need(1, 1, 1, 1);
                    player.endViewWindowFull.set_all_need(1, 1, 1, 1);

                    player.ScoreTime = 10;
                }
                else {
                    playerDamage = true;
                }

                NumHit++;
            }
        }
    }
}
