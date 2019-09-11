using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inercia_test : MonoBehaviour {

    public float x = 1;
    public float y = 1;
    public float z = 1;

	// Use this for initialization
	void Start () {
        Rigidbody rb;
        rb = gameObject.GetComponent<Rigidbody>();

        rb.velocity = new Vector3(rb.velocity.x + x, rb.velocity.y + y, rb.velocity.z + z);
    }
	
	// Update is called once per frame
	void Update () {

    }
}
