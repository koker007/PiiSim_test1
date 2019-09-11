using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public bool OpenMenu_yn = false;
    public bool GamePlay_yn = false;
    public bool Preview_yn = false;
    public bool Pause_yn = false;

    public bool Mouse_control_yn = false;

    // Use this for initialization
    void Start () {
        Preview_yn = true;
        GamePlay_yn = false;
        OpenMenu_yn = false;
        Pause_yn = true;
        Mouse_control_yn = false;
    }

    public void OpenMenu() {
        Preview_yn = false;
        GamePlay_yn = false;
        OpenMenu_yn = true;
        Pause_yn = true;
        Mouse_control_yn = false;
    }

    public void GamePlay() {
        GamePlay_yn = true;
        Preview_yn = false;
        OpenMenu_yn = false;
        Pause_yn = false;
        Mouse_control_yn = true;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
