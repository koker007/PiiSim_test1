using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroy_timer : MonoBehaviour {

    [SerializeField]
    public float time_to_destroy = 0;

    public bool timer_starting = true;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        test_time();
	}

    void test_time() {
        if (timer_starting) {
            time_to_destroy -= Time.deltaTime;
            if (time_to_destroy <= 0) {
                Destroy(gameObject);
            }
        }
    }
}
