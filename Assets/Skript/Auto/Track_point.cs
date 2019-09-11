using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Track_point : MonoBehaviour {

    public Track_point point_next_normal;
    public Track_point point_next_left;
    public Track_point point_next_right;

    [SerializeField]
    public float max_speed = 10;

	// Use this for initialization
	void Start () {
        HideMesh();
	}

    //Спрятать
    void HideMesh() {
        MeshRenderer mesh = gameObject.GetComponent<MeshRenderer>();
        if (mesh != null) {
            //mesh.enabled = false;
            Destroy(mesh);
        }
    }

    //
    Vector3 GetRotateToNext(Track_point track_next) {
        Vector3 retur_vec = new Vector3();

        //Поворот на ровном
        float rotate_y;
        //Узнаем позицию сейчас
        Vector3 pos_now = gameObject.transform.position;
        Vector3 pos_next = track_next.gameObject.transform.position;

        //расчитываем поворот для движения туда куда надо


        return retur_vec;
    }

}
