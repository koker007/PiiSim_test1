using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandPosStartHouse : MonoBehaviour {

	// Use this for initialization
	void Start () {
        MoveRandomStartPos();
    }

    //Перемещение обьекта на рандомную позицию
    void MoveRandomStartPos() {
        Vector3 startPos = gameObject.transform.position;

        float maxPos = 256;

        startPos.x = Random.Range(startPos.x, maxPos);

        gameObject.transform.position = startPos;
    }
}
