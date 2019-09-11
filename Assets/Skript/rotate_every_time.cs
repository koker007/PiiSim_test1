using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotate_every_time : MonoBehaviour {

    [SerializeField]
    public float rotate_x = 0;
    [SerializeField]
    public float rotate_y = 0;
    [SerializeField]
    public float rotate_z = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        test_rotate_fix();
    }

    void test_rotate_fix() {
        gameObject.transform.rotation = Quaternion.EulerAngles(rotate_x, rotate_y, rotate_z);
    }
}
