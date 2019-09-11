using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetRand : MonoBehaviour {

    [SerializeField]
    float Veroatnost = 0;

	// Use this for initialization
	void Start () {
        if (Random.Range(0, 100) > Veroatnost) {
            Destroy(gameObject);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
