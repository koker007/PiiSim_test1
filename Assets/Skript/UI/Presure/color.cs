using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class color : MonoBehaviour {

    [SerializeField]
    private Player player;
    [SerializeField]
    private PiiController piiController;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        testAngle();
	}

    void testAngle() {
        //Сперва узнаем какому напору равен один градус
        float OneGradPresure = 360 / player.maxPresure;

        //Теперь находим градус для напора в данный момент
        float GradNow = OneGradPresure * piiController.speed_pii;

        //Меняем угол
        gameObject.transform.localRotation = Quaternion.Euler(0, 0, GradNow);
    }
}
