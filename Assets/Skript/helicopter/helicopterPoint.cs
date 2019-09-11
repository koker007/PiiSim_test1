using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class helicopterPoint : MonoBehaviour {

	// Use this for initialization
	void Start () {
        HideMesh();

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    //Спрятать
    void HideMesh()
    {
        MeshRenderer mesh = gameObject.GetComponent<MeshRenderer>();
        if (mesh != null)
        {
            mesh.enabled = false;
        }
    }
}
