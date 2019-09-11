using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneSpawner : MonoBehaviour {

    [SerializeField]
    GameObject[] PrefabZones;

	// Use this for initialization
	void Start () {
        NoVisualBox();
        SpawnZone();
    }

    void NoVisualBox() {
        MeshRenderer visual = GetComponent<MeshRenderer>();
        if (visual != null) {
            visual.enabled = false;
        }
        
        
    }

    void SpawnZone() {
        if (PrefabZones != null && PrefabZones.Length > 0)
        {

            //Чистим зону от обьектов
            /*
            GameObject[] childrenObj = gameObject.GetComponentsInChildren<GameObject>();

            if (childrenObj != null)
            {
                for (int num = 0; num < childrenObj.Length; num++)
                {
                    Destroy(childrenObj[num]);
                }
            }
            */
            //Место расчищено вставляем префаб зоны
            GameObject newZone = Instantiate(PrefabZones[Random.Range(0, PrefabZones.Length)], gameObject.transform);
            newZone.transform.localPosition = new Vector3(0, 0, 0);

        }
    }
}
