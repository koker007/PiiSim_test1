using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankBullet : MonoBehaviour {

    PiiTarget piiTarget;
    BoomTarget boomTarget;
    Rigidbody rigidbody;

    public bool testBoom = false;
    bool testBoomOk = false;

    [SerializeField]
    GameObject rendererObj;

    [SerializeField]
    float TimeDelMax = 3;
    float TimeDelNow = 3;

    void iniPiitarget() {
        if (piiTarget == null) {
            piiTarget = gameObject.GetComponent<PiiTarget>();
        }
    }
    void iniBoomtarget() {
        if (boomTarget == null) {
            boomTarget = gameObject.GetComponent<BoomTarget>();
        }
    }
    void iniRigidbody() {
        if (rigidbody == null) {
            rigidbody = gameObject.GetComponent<Rigidbody>();
        }
    }

	// Use this for initialization
	void Start () {
        iniPiitarget();
        iniBoomtarget();
        iniRigidbody();
    }
	
	// Update is called once per frame
	void Update () {
        TestBoom();
        TestRoot();
    }

    //Если снаряд что-то задел
    private void OnTriggerEnter(Collider other)
    {
        if (!testBoom)
        {
            testBoom = true;
            PiiPrefab piiPrefab = other.GetComponent<PiiPrefab>();
            if (piiPrefab != null)
            {
                steam_achievement steam_Achievement = GameObject.FindGameObjectWithTag("steam_manager").GetComponent<steam_achievement>();
                if (steam_Achievement != null)
                    steam_Achievement.plus_1_tank_shell();
            }
        }
    }


    void TestBoom() {
        //Если нужно тестить взрыв
        if (testBoom && !testBoomOk && piiTarget != null && boomTarget != null) {
            //Если здоровье больше нуля значит нужно инициализировать взрыв самому
            if (!testBoomOk)
            {
                testBoomOk = true;
                piiTarget.DeathTarget();

                if (rendererObj != null)
                {
                    rendererObj.active = false;
                }

                TimeDelNow = TimeDelMax;
                if (rigidbody != null) {
                    rigidbody.velocity = new Vector3(0, 0, 0);
                    rigidbody.constraints = RigidbodyConstraints.FreezeAll;
                }
            }
            else {
                TimeDelNow -= Time.deltaTime;
                if (TimeDelNow < 0) {
                    Destroy(gameObject);
                }
            }
        }
    }
    void TestRoot() {
        if (rigidbody != null && piiTarget != null && piiTarget.heath > 0) {
            gameObject.transform.forward = rigidbody.velocity.normalized;
        }
    }
}
