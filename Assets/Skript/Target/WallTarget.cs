using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTarget : MonoBehaviour {

    PiiTarget[] targets;

    [SerializeField]
    int need_destroy = 6;
    int need_targets_to_destroy;
    [SerializeField]
    Player player;
    [SerializeField]
    AudioClip clipDestroy;

    [SerializeField]
    WallTarget wallTargetDestroy;
    [SerializeField]
    lvl_text lvl_Text;
    [SerializeField]
    string text_destroy;

    [SerializeField]
    setings seting;
    void iniSetings() {
        if (seting == null) {
            seting = GameObject.FindGameObjectWithTag("setings_game").GetComponent<setings>();
        }
    }

    [SerializeField]
    private GameplayParametrs gameplayParametrs;

    bool CrashWall_yn = false;

    void TestDestroys() {
        if (!CrashWall_yn)
        {
            targets = GetComponentsInChildren<PiiTarget>();

            //Узнаем количество еще живых целей
            int targets_live_now = 0;
            for (int num = 0; num < targets.Length; num++)
            {
                if (targets[num].heath > 0)
                {
                    targets_live_now++;
                }
            }


            //Если стена еще не сломана
            if (need_targets_to_destroy >= targets_live_now)
            {
                DestroyWall();
            }
        }
    }

    public void DestroyWall() {
        if (!CrashWall_yn) {
            CrashWall_yn = true;
            //Ломаем
            for (int num_now = 0; num_now < targets.Length; num_now++)
            {
                //targets[num_now].gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                if (targets[num_now].heath > 0){
                    targets[num_now].heath = 0;
                    targets[num_now].gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                }
            }

            //Если игрок есть то говорим ему передвигаться на позицию
            if (player != null)
            {
                player.move_to_3();
            }
            if (gameObject.GetComponent<AudioSource>() != null && clipDestroy != null && seting != null)
            {
                AudioSource audioSource = gameObject.GetComponent<AudioSource>();

                audioSource.volume = seting.game.volume_all * seting.game.volume_sound;
                audioSource.PlayOneShot(clipDestroy);
            }

            if (wallTargetDestroy != null) {
                wallTargetDestroy.DestroyWall();
            }

            //if (lvl_Text != null && text_destroy != null) {
            //    lvl_Text.text_next = text_destroy;
            //}
            if (gameplayParametrs != null) {
                gameplayParametrs.start_lvl_2();
            }
        }
    }

	// Use this for initialization
	void Start () {
        iniSetings();
        targets = gameObject.GetComponentsInChildren<PiiTarget>();
        need_targets_to_destroy = targets.Length - need_destroy;
	}
	
	// Update is called once per frame
	void Update () {
        TestDestroys();
    }
}
