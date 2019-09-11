using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class loading : MonoBehaviour {

    [SerializeField]
    Image loadingImg;

    [SerializeField]
    Image loadingImg2;

    [SerializeField]
    AudioSource audioSource;
    [SerializeField]
    AudioSource musicSource;

    [SerializeField]
    setings setings;
    bool reload_ok = false;

    public int sceneID = 1;

    float loading_progress_need = 0;
    float loading_progress_now = 0;

    float attention_alpha_need = 1;

	// Use this for initialization
	void Start () {
        StartCoroutine(AsyncLoad());
	}
	
	// Update is called once per frame
	void Update () {
        calc_loading_now();
        calc_attention_alpha(loadingImg);
        draw_image_indicator(loadingImg2);

        testSound();
    }

    void calc_loading_now() {
        if (loading_progress_now != loading_progress_need) {
            loading_progress_now += ((loading_progress_need - loading_progress_now) / 16) * Time.deltaTime;
        }
    }

    void calc_attention_alpha(Image image) {
        if (image != null) {
            Color color_new = image.color;

            //Если нужно прибавить
            if (color_new.a < attention_alpha_need) {
                color_new.a += Time.deltaTime;

                if(color_new.a > 1)
                {
                    color_new.a = 1;
                    attention_alpha_need = 0;
                }
                   
            }
            //Если нужно убавить
            else if (color_new.a > attention_alpha_need) {
                color_new.a -= Time.deltaTime;

                if (color_new.a < 0)
                {
                    color_new.a = 0;
                    attention_alpha_need = 1;
                }
            }

            image.color = color_new;
        }
    }

    void draw_image_indicator(Image image) {
        if (image != null) {
            image.fillAmount = 0.05f + (0.9f - loading_progress_now);
        }
    }

    void testSound() {
        if (audioSource != null && audioSource.clip != null && setings != null) {
            if (!audioSource.isPlaying) {
                audioSource.Play();
            }
            audioSource.volume = setings.game.volume_all * setings.game.volume_music * 0.4f;
            audioSource.pitch = 0.9f + loading_progress_now;
        }

        if (musicSource != null && musicSource.clip != null && setings != null) {
            if (!musicSource.isPlaying) {
                musicSource.Play();
            }
            musicSource.volume = setings.game.volume_all * setings.game.volume_music * 0.3f;
        }
    }

    IEnumerator AsyncLoad() {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneID);
        while (!operation.isDone)
        {
            loading_progress_need = (operation.progress);
            yield return null;
        }
    }
}
