using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rainbow : MonoBehaviour {

    Player player;
    void iniPlayer() {
        if (player == null) {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }
    }

    float alpha = 0.5f;
    MeshRenderer meshRenderer;
    void iniMeshRender() {
        if (meshRenderer == null) {
            meshRenderer = gameObject.GetComponent<MeshRenderer>();
        }
    }

    void LookToPlayer() {
        gameObject.transform.LookAt(player.gameObject.transform);
    }

	// Use this for initialization
	void Start () {
        iniMeshRender();
        iniPlayer();
        //LookToPlayer();
    }
	
	// Update is called once per frame
	void Update () {
        TestAlpha();
        TestMaterial();
    }

    //Вычисление альфы
    void TestAlpha() {
        //уменьшение
        alpha -= 0.025f * Time.deltaTime;
        if (alpha < 0) alpha = 0;

        float rot_horizont = player.transform.rotation.eulerAngles.y;
        float rot_vertical = player.bolt.transform.localRotation.eulerAngles.x;

        float rot_hor_need = -25.0f;
        float rot_ver_need = -30.0f;
        float cmeshenie = 30.0f;

        float coof_hor = 0;
        if (rot_horizont > 180) {
            rot_horizont -= 360.0f;
        }
        float raznicaHor = rot_hor_need - rot_horizont;
        if (raznicaHor > 0 && raznicaHor < cmeshenie) {
            coof_hor = 1 - (raznicaHor / cmeshenie);
        }
        else if (raznicaHor < 0 && raznicaHor > -cmeshenie) {
            coof_hor = 1 - (raznicaHor / -cmeshenie);
        }

        float coof_ver = 0;
        if (rot_vertical > 180)
        {
            rot_vertical -= 360.0f;
        }
        float raznicaVer = rot_ver_need - rot_vertical;
        if (raznicaVer > 0 && raznicaVer < cmeshenie/2)
        {
            coof_ver = 1 - (raznicaVer / cmeshenie/2);
        }
        else if (raznicaVer < 0 && raznicaVer > -cmeshenie/2)
        {
            coof_ver = 1 - (raznicaVer / -cmeshenie/2);
        }

        //Debug.Log("rot_horizont" + rot_horizont + " rot_hor_need" + rot_hor_need + " coof_hor" + coof_hor);

        //провека силы напора
        PiiController bolt = player.bolt.GetComponent<PiiController>();

        float powerBolt = bolt.speed_pii + 10;
        float coofBolt = 0;
        float powerBoltMax = 60;
        if (powerBolt > 40) {
            if (powerBolt > powerBoltMax)
            {
                coofBolt = 1;
            }
            else {
                coofBolt = 1 - ((powerBoltMax - powerBolt)/20);
            }

            float newAlpha = coof_hor * coof_ver * coofBolt;
            if (newAlpha > alpha)
            {
                alpha += (newAlpha - alpha)/3f;
            }
        }

    }

    void TestMaterial() {
        if (meshRenderer != null) {
            Color NewColor = meshRenderer.material.color;
            NewColor.a = alpha;
            meshRenderer.material.color = NewColor;
        }
    }
}
