using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flyer : MonoBehaviour {

    public float speed = 150;
    public float MaxLifeTime = 300;
    private float lifeTime = 0;
    public bool needRandomRot = false;
    private float random_rot = 0;

    [SerializeField]
    GameObject particleSmokeEngine;

    Rigidbody rigidbody;
    PiiTarget piiTarget;

    [SerializeField]
    AudioClip audioStart;
    [SerializeField]
    AudioClip audioEngine;

    bool soundStantPlay = false;
    AudioSource audioSourceStart;
    AudioSource audioSourceEngine;
    ParticleSystem partSmokeEngine;

    setings seting;


    void iniRigidbody() {
        if (rigidbody == null) {
            rigidbody = GetComponent<Rigidbody>();
        }
    }
    void iniPiiTarget() {
        if (piiTarget == null) {
            piiTarget = GetComponent<PiiTarget>();
        }
    }

    void iniAudio() {
        if (audioStart != null && audioSourceStart == null) {
            audioSourceStart = gameObject.AddComponent<AudioSource>();
            audioSourceStart.clip = audioStart;
        }
        if (audioEngine != null && audioSourceEngine == null) {
            audioSourceEngine = gameObject.AddComponent<AudioSource>();
            audioSourceEngine.clip = audioEngine;
        }
        
    }

    void iniSmokeEngine() {
        if (particleSmokeEngine != null) {
            GameObject partSmoke = Instantiate(particleSmokeEngine);
            partSmoke.transform.position = transform.position;
            partSmoke.transform.parent = transform;

            partSmokeEngine = partSmoke.GetComponent<ParticleSystem>();
        }
    }
    void iniSeting() {
        if (seting == null) {
            seting = GameObject.FindGameObjectWithTag("setings_game").GetComponent<setings>();
        }
    }
    void iniRandom() {
        random_rot = Random.Range(-5, 5);
    }

	// Use this for initialization
	void Start () {
        iniRigidbody();
        iniPiiTarget();
        iniAudio();
        iniSmokeEngine();
        iniSeting();
        iniRandom();
    }
	
	// Update is called once per frame
	void Update () {
        TestFly();
        TestPartEngine();
        TestPlaySound();
        TestTime();
        TestDelite();
        TestRotate();
    }

    void TestFly()
    {
        if (rigidbody != null && piiTarget != null)
        {
            if (piiTarget.heath > 0)
            {
                //rigidbody.velocity = new Vector3(0, Time.deltaTime * 2, Time.deltaTime * 150);
                rigidbody.velocity = transform.forward * Time.deltaTime * speed;
                //if (rigidbody.velocity.magnitude < speed * Time.deltaTime)
                //    rigidbody.velocity += transform.forward * Time.deltaTime * speed;
            }
        }
    }
    void TestPartEngine() {
        if (partSmokeEngine != null && !partSmokeEngine.isPlaying && piiTarget != null && piiTarget.heath > 0) {
            partSmokeEngine.Play();
        }
        else if (partSmokeEngine != null && partSmokeEngine.isPlaying && piiTarget != null && piiTarget.heath <= 0)
        {
            partSmokeEngine.Stop();

        }
    }
    void TestPlaySound() {
        if (audioSourceStart != null && !soundStantPlay && seting != null) {
            soundStantPlay = true;
            audioSourceStart.volume = seting.game.volume_all * seting.game.volume_sound;
            audioSourceStart.spatialBlend = 0.8f;
            audioSourceStart.maxDistance = 3000;
            audioSourceStart.Play();
        }
        if (audioSourceEngine != null && !audioSourceEngine.isPlaying && seting != null) {
            audioSourceEngine.volume = seting.game.volume_all * seting.game.volume_sound;
            audioSourceEngine.spatialBlend = 0.3f;
            audioSourceEngine.maxDistance = 3000;
            audioSourceEngine.Play();
        }
    }
    void TestTime() {
        lifeTime += Time.deltaTime;
    }
    void TestDelite() {
        if (lifeTime > MaxLifeTime) {
            Destroy(gameObject);
        }
    }
    void TestRotate() {
        if (needRandomRot && lifeTime > 20 && lifeTime < 40) {
            Quaternion quaternion_now = transform.rotation;
            Vector3 euler_now = quaternion_now.eulerAngles;

            euler_now = new Vector3(euler_now.x, euler_now.y + random_rot * Time.deltaTime, euler_now.z);
            quaternion_now.eulerAngles = euler_now;
            transform.rotation = quaternion_now;
        }
    }
}
