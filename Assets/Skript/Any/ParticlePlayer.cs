using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePlayer : MonoBehaviour {

    public bool play = false;
    bool playOld = false;

    [SerializeField]
    ParticleSystem[] particles;

	// Use this for initialization
	void Start () {
        StopALL();
    }
	
	// Update is called once per frame
	void Update () {
        TestPlay();
    }

    //остановить все
    void StopALL() {
        for (int num = 0; num < particles.Length; num++) {
            if (particles[num] != null) {
                particles[num].Stop();
            }
        }
    }
    void PlayALL() {
        for (int num = 0; num < particles.Length; num++) {
            if (particles[num] != null && particles[num].isStopped) {
                particles[num].Play();
            }
        }
    }

    void TestPlay() {
        if (play && !playOld)
        {
            playOld = true;
            PlayALL();
        }
        else if(!play && playOld) {
            playOld = false;
            StopALL();
        }
    }
}
