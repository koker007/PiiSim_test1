using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pii_water_home : MonoBehaviour {

    [SerializeField]
    Player player;
    [SerializeField]
    WallTarget Wall;

    Material material;
    float plus_max = 1;
    float start_pos_y = 0;

    //максимальное количество очков
    float points_max = 2000;
    float points_start = 500;

	// Use this for initialization
	void Start () {
        start_pos_y = gameObject.transform.position.y;
        material = gameObject.GetComponent<MeshRenderer>().materials[0];
    }
	
	// Update is called once per frame
	void Update () {
        TestPiiWater();

    }

    //Тест лужи мочи
    void TestPiiWater() {
        if (player != null) {
            //Если нужно поднимать
            if (player.gameplayParametrs.get_lvl_now() < 2) {
                //узнаем в процентах настоящий предел
                float percent_now = 0;
                if (player.ScoreALL > points_start)
                    percent_now = ((player.ScoreALL - points_start) / points_max);
                else percent_now = 0;
                //узнали теперь считаем нужную высоту и применяем ее
                Vector3 position_now = gameObject.transform.position;
                position_now.y = start_pos_y + (plus_max * percent_now);
                position_now.y =  gameObject.transform.position.y + (position_now.y - gameObject.transform.position.y)/20;

                if (player.ScoreTime > points_max && Wall != null) {
                    Wall.DestroyWall();
                }
                gameObject.transform.position = position_now;

                if (material != null)
                {
                    Vector2 pos_tex = material.mainTextureOffset;
                    pos_tex.y += 0.05f * Time.deltaTime;
                    material.mainTextureOffset = pos_tex;
                }
            }
            //Иначе если нужен спуск
            else if (start_pos_y != gameObject.transform.position.y) {
                //Уменьшаем плавно
                Vector3 position_now = gameObject.transform.position;
                position_now.y -= 0.1f * Time.deltaTime;
                if (position_now.y < start_pos_y) {
                    position_now.y = start_pos_y;
                }
                gameObject.transform.position = position_now;

                if (material != null)
                {
                    Vector2 pos_tex = material.mainTextureOffset;
                    pos_tex.y += 0.5f * Time.deltaTime;
                    material.mainTextureOffset = pos_tex;
                }
            }
        }
    }
}
