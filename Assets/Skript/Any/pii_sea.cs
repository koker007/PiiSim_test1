using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pii_sea : MonoBehaviour {

    Material material;

	// Use this for initialization
	void Start () {
        material = gameObject.GetComponent<MeshRenderer>().materials[0];
	}
	
	// Update is called once per frame
	void Update () {
        Move_sea();
	}

    void Move_sea() {
        if (material != null) {
            Vector2 pos_tex = material.mainTextureOffset;
            pos_tex.y += 0.01f * Time.deltaTime;
            material.mainTextureOffset = pos_tex;
        }
    }
}
