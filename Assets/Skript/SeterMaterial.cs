using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeterMaterial : MonoBehaviour {

    //Материал для установки
    public Material SetMaterial;

    //Материалы для замены 
    Material GetMaterials;
    

	// Use this for initialization
	void Start () {
        foundMat();
        SetALLMaterials();
    }

    //Ищем обьект с материалом по умолчанию
    void foundMat() {
        if (SetMaterial == null) {
            SetMaterial = GameObject.FindGameObjectWithTag("setings_game").GetComponent<SeterMaterial>().SetMaterial;
        }
    }

    void SetALLMaterials() {

        if (SetMaterial != null) {
            //Сперва ищем все мешь рендеры
            MeshRenderer[] AllMeshRenderer = gameObject.GetComponents<MeshRenderer>();

            //Перебираем меши
            for (int num_mesh = 0; num_mesh < AllMeshRenderer.Length; num_mesh++) {
                //Перебираем материалы
                Material[] materialsAllOld = AllMeshRenderer[num_mesh].materials;
                Material[] materialsAllNew = new Material[materialsAllOld.Length];

                for (int num_mat = 0; num_mat < materialsAllOld.Length; num_mat++) {

                    Material materialOld = materialsAllOld[num_mat];

                    Material materialNew = new Material(SetMaterial);
                    materialNew.mainTexture = materialOld.mainTexture;
                    materialNew.SetTexture("_RampTex", SetMaterial.GetTexture("_RampTex"));
                    materialNew.SetTextureScale("_MainTex", materialOld.GetTextureScale("_MainTex"));
                    materialNew.SetTextureOffset("_MainTex", materialOld.GetTextureOffset("_MainTex"));

                    materialsAllNew[num_mat] = materialNew;
                }

                AllMeshRenderer[num_mesh].materials = materialsAllNew;
            }
        }
    }
}
