using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreMehUI : MonoBehaviour {

    [SerializeField]
    private PiiController piiController;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        gameObject.transform.Rotate(new Vector3(0, 0, piiController.speed_pii*Time.deltaTime*100));
	}
}
