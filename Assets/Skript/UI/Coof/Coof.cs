using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Coof : MonoBehaviour {

    [SerializeField]
    Text CoofText;

    GameplayParametrs gameplayParametrs;
    void IniGameParam() {
        if (gameplayParametrs == null) {
            gameplayParametrs = GameObject.FindGameObjectWithTag("setings_game").GetComponent<GameplayParametrs>();
        }
    }

	// Use this for initialization
	void Start () {
        IniGameParam();
    }
	
	// Update is called once per frame
	void Update () {
        VisualizedCoof();
    }

    void VisualizedCoof() {
        if (gameplayParametrs != null && CoofText != null)
        {
            if (gameplayParametrs.TimeGamePlay <= 0)
            {
                CoofText.text = "0";
            }
            else
            {
                CoofText.text = System.Convert.ToString(gameplayParametrs.CoofST);
            }
        }
    }
}
