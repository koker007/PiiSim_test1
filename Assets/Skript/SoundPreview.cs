using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPreview : MonoBehaviour {

    [SerializeField]
    private GameController gameController;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (gameController.Preview_yn) {
            gameController.GamePlay();
        }
	}
}
