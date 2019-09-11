using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BildTarget : MonoBehaviour {

    public bool need_destroy = false;
    private bool old_need_destroy = false;

    float fool_size_z = 0;
    float now_size_z = 0;

    [SerializeField]
    float CoofSdviga = 1;

    [SerializeField]
    GameObject Particle_smoke_destroy;
    [SerializeField]
    GameObject Particle_smoke_fire;
    [SerializeField]
    GameObject Particle_fire;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Destroing();
	}

    //Уничтожение
    void Destroing() {
        if (need_destroy) {
            //Если старт первый
            //Узнаем размер по z
            if (!old_need_destroy) {
                //Cоздаем дым
                if (Particle_smoke_destroy != null) {
                    Vector3 pos_parent = gameObject.transform.position;
                    Instantiate(Particle_smoke_destroy, pos_parent, Particle_smoke_destroy.transform.rotation);
                }
                if (Particle_smoke_fire != null && Particle_fire != null && Random.Range(0,100) < 20) {
                    Vector3 pos_parent = gameObject.transform.position;
                    Instantiate(Particle_smoke_fire, pos_parent, Particle_smoke_fire.transform.rotation);
                    Instantiate(Particle_fire, pos_parent, Particle_fire.transform.rotation);
                }


                BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();

                if (boxCollider != null)
                {
                    fool_size_z = (boxCollider.size.z * CoofSdviga) / 1.1f;
                }
                else {
                    need_destroy = false;
                }
                old_need_destroy = true;
            }

            //Двигаем вниз
            //Считаем насколько сдвинуть в этом кадре
            float now_frame_z = (fool_size_z / 1.5f) * Time.deltaTime;
            now_size_z += now_frame_z;

            Vector3 pos = gameObject.transform.position;
            pos.y -= now_frame_z;
            gameObject.transform.position = pos;

            if (now_size_z > fool_size_z) {
                float return_size = now_size_z - fool_size_z;
                Vector3 pos2 = gameObject.transform.position;
                pos2.y += return_size;
                gameObject.transform.position = pos2;

                need_destroy = false;
            }
        }
    }
}
