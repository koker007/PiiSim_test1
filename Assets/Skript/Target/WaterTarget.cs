using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTarget : MonoBehaviour {

    [SerializeField]
    private float PresureFull = 1;

    private float PresureNow = 0;

    [SerializeField]
    Color color_full = new Color(1, 0.8f, 0);

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PlusPii(float Presure) {
        //Меняем цвет
        PresureNow += Presure;
        //Узнаем насколько процентов заполнено
        if (PresureFull == 0) {
            PresureFull = 1;
        }
        float PercentFull = PresureNow / PresureFull;

        //Вытаскиваем то что надо окрашивать

        
        
    }
}
