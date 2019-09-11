using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class towerTES : MonoBehaviour {

    //для информации была ли ранее тес жива
    bool TESalive = true;

    ParticleSystem smoke;
    PiiTarget target;

	// Use this for initialization
	void Start () {
        //Вытаскиваем дым
        smoke = GetComponentInChildren<ParticleSystem>();
        target = GetComponent<PiiTarget>();
	}
	
	// Update is called once per frame
	void Update () {
        test_TES_live();
    }

    void test_TES_live() {
        if (smoke != null && !smoke.isPlaying && target != null && target.heath > 0) {
            smoke.Play();
        }
    }
}
