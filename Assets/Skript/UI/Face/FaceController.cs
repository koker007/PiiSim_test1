using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FaceController : MonoBehaviour {


    [SerializeField]
    GameObject faceObj;
    Image faceImage;

    [SerializeField]
    VoiceCtrl Voice;
    [SerializeField]
    FaceAudio faceAudio;

    public Sprite normal;
    public Sprite normal_left;
    public Sprite normal_right;

    public Sprite normal_close;
    public Sprite normal_left_close;
    public Sprite normal_right_close;

    public Sprite bliss;
    public Sprite bliss_left;
    public Sprite bliss_right;

    public Sprite bliss_close;
    public Sprite bliss_left_close;
    public Sprite bliss_right_close;

    public Sprite surprise;
    public Sprite surprise_left;
    public Sprite surprise_right;

    public Sprite surprise_close;
    public Sprite surprise_left_close;
    public Sprite surprise_right_close;

    public Sprite stress;
    public Sprite stress_left;
    public Sprite stress_right;

    public Sprite aggress;
    public Sprite aggress_left;
    public Sprite aggress_right;

    public Sprite fear;
    public Sprite fear_left;
    public Sprite fear_right;

    public Sprite butthurt;
    public Sprite butthurt_left;
    public Sprite butthurt_right;

    bool left = false;
    bool right = false;

    float aggress_sec = 0;
    float bliss_sec = 0;
    float surprise_sec = 0;
    float stress_sec = 0;
    float fear_sec = 0;
    float butthurt_sec = 0;

    float close_sec = 0;

    float face_grad_plus = 0;
    float face_grad_fact = 0;

    // Use this for initialization
    void Start () {
        faceImage = faceObj.GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
        time_minus();
        grad_calc();
        close_calc();

        DrawFace();
    }

    void set_left() {
        left = true;
        right = false;
    }
    void set_center() {
        left = false;
        right = false;
    }
    void set_right() {
        left = false;
        right = true;
    }

    public void set_face_grad(float new_face_grad_fact) {
        face_grad_fact = new_face_grad_fact;
    }

    void grad_calc() {
        //Считаем рандом 
        if (Random.Range(0, 100) < 0.5f)
        {
            face_grad_plus = Random.Range(-30, 30);
        }

        //Проверка пределов
        if (face_grad_plus > 30)
        {
            face_grad_plus = 30;
        }
        else if (face_grad_plus < -30)
        {
            face_grad_plus = -30;
        }

        //Уменьшаем рандом
        if (face_grad_plus > 0)
        {
            face_grad_plus -= Time.deltaTime * 5;
        }
        if (face_grad_plus < 0)
        {
            face_grad_plus += Time.deltaTime * 5;
        }

        float face_itog = face_grad_fact + face_grad_plus;

        if (face_itog > 20)
        {
            set_right();
        }
        else if (face_itog < -20)
        {
            set_left();
        }
        else
        {
            set_center();
        }
    }
    void close_calc() {
        if (Random.Range(0, 100) < 2.1f) close_sec = 0.1f;
    }

    public void set_bliss(float sec) {
        bliss_sec += sec;
        surprise_sec = 0;
    }
    public void set_surprise(float sec) {
        surprise_sec += sec;
        bliss_sec = 0;
    }
    public void set_stress(float sec) {
        stress_sec += sec;
    }
    public void set_agress(float sec) {
        aggress_sec = sec;
    }
    public void set_fear(float sec) {
        fear_sec = sec;
    }

    public void set_butthurt(float sec) {
        butthurt_sec = sec;
    }

    public void set_bliss_voice(float sec) {
        if (fear_sec == 0 && butthurt_sec == 0)
        {
            set_bliss(sec);
            if (Voice != null && faceAudio != null && faceAudio.bliss != null && faceAudio.bliss.Length != 0)
            {
                if (!Voice.get_and_play_clip(faceAudio.bliss[Random.Range(0, faceAudio.bliss.Length - 1)]))
                    Voice.get_and_play_clip(faceAudio.bliss[Random.Range(0, faceAudio.bliss.Length - 1)]);
            }
        }
    }
    public void set_surprise_voice(float sec) {
        if (fear_sec == 0 && butthurt_sec == 0) {
            set_surprise(sec);
            if (Voice != null && faceAudio != null && faceAudio.surprise != null && faceAudio.surprise.Length != 0)
            {
                if (!Voice.get_and_play_clip(faceAudio.surprise[Random.Range(0, faceAudio.surprise.Length - 1)]))
                    Voice.get_and_play_clip(faceAudio.surprise[Random.Range(0, faceAudio.surprise.Length - 1)]);
            }
        }
    }
    public void set_choked_voice() {
        set_fear(1.0f);
        if (Voice != null && faceAudio != null && faceAudio.choked != null && faceAudio.choked.Length != 0) {
            if (!Voice.get_and_play_clip(faceAudio.choked[Random.Range(0, faceAudio.choked.Length - 1)]))
                Voice.get_and_play_clip(faceAudio.choked[Random.Range(0, faceAudio.choked.Length - 1)]);


            aggress_sec = 0;
            bliss_sec = 0;
            surprise_sec = 0;
            stress_sec = 0;
        }
    }

    public void set_butthurt_voice() {
        if (fear_sec == 0) {
            set_butthurt(0.7f);
            if (Voice != null && faceAudio != null && faceAudio.butthurt != null && faceAudio.butthurt.Length != 0)
            {
                if (!Voice.get_and_play_clip(faceAudio.butthurt[Random.Range(0, faceAudio.butthurt.Length - 1)]))
                    Voice.get_and_play_clip(faceAudio.butthurt[Random.Range(0, faceAudio.butthurt.Length - 1)]);


                aggress_sec = 0;
                bliss_sec = 0;
                surprise_sec = 0;
                stress_sec = 0;
            }
        }
    }

    void time_minus() {
        aggress_sec -= Time.deltaTime;
        bliss_sec -= Time.deltaTime;
        surprise_sec -= Time.deltaTime;
        stress_sec -= Time.deltaTime;
        fear_sec -= Time.deltaTime;
        butthurt_sec -= Time.deltaTime;
        if (butthurt_sec > 0) Debug.Log("butthurt " + butthurt_sec);

        close_sec -= Time.deltaTime;

        if (aggress_sec < 0) aggress_sec = 0; 
        if (bliss_sec < 0) bliss_sec = 0;
        if (surprise_sec < 0) surprise_sec = 0;
        if (stress_sec < 0) stress_sec = 0;
        if (fear_sec < 0) fear_sec = 0;
        if (butthurt_sec < 0) butthurt_sec = 0;
        if (close_sec < 0) close_sec = 0;
    }

    //нарисовать эмоцию
    void DrawFace() {

        //Нормальное лицо
        if (aggress_sec == 0 && butthurt_sec == 0 && bliss_sec == 0 && surprise_sec == 0 && stress_sec == 0 && fear_sec == 0) {
            if (close_sec <= 0)
            {
                //Лево
                if (left && !right)
                    faceImage.sprite = normal_left;
                //Право
                else if (!left && right)
                    faceImage.sprite = normal_right;
                //Центр
                else if (!left && !right)
                    faceImage.sprite = normal;
            }
            else {
                //Лево
                if (left && !right)
                    faceImage.sprite = normal_left_close;
                //Право
                else if (!left && right)
                    faceImage.sprite = normal_right_close;
                //Центр
                else if (!left && !right)
                    faceImage.sprite = normal_close;
            }
        }

        else if (butthurt_sec > 0) {
            if (close_sec <= 0)
            {
                //Лево
                if (left && !right)
                    faceImage.sprite = butthurt_left;
                //Право
                else if (!left && right)
                    faceImage.sprite = butthurt_right;
                //Центр
                else if (!left && !right)
                    faceImage.sprite = butthurt;
            }
            else
            {
                //Лево
                if (left && !right)
                    faceImage.sprite = butthurt_left;
                //Право
                else if (!left && right)
                    faceImage.sprite = butthurt_right;
                //Центр
                else if (!left && !right)
                    faceImage.sprite = butthurt;
            }
        }

        //Улыбка
        else if (bliss_sec > 0) {
            if (close_sec <= 0)
            {
                //Лево
                if (left && !right)
                    faceImage.sprite = bliss_left;
                //Право
                else if (!left && right)
                    faceImage.sprite = bliss_right;
                //Центр
                else if (!left && !right)
                    faceImage.sprite = bliss;
            }
            else {
                //Лево
                if (left && !right)
                    faceImage.sprite = bliss_left_close;
                //Право
                else if (!left && right)
                    faceImage.sprite = bliss_right_close;
                //Центр
                else if (!left && !right)
                    faceImage.sprite = bliss_close;
            }
        }
        //stress
        else if (stress_sec > 0) {
            //Лево
            if (left && !right)
                faceImage.sprite = stress_left;
            //Право
            else if (!left && right)
                faceImage.sprite = stress_right;
            //Центр
            else if (!left && !right)
                faceImage.sprite = stress;
        }
        //surprise
        else if (surprise_sec > 0)
        {
            if (close_sec <= 0)
            {
                //Лево
                if (left && !right)
                    faceImage.sprite = surprise_left;
                //Право
                else if (!left && right)
                    faceImage.sprite = surprise_right;
                //Центр
                else if (!left && !right)
                    faceImage.sprite = surprise;
            }
            else {
                //Лево
                if (left && !right)
                    faceImage.sprite = surprise_left_close;
                //Право
                else if (!left && right)
                    faceImage.sprite = surprise_right_close;
                //Центр
                else if (!left && !right)
                    faceImage.sprite = surprise_close;
            }

        }
        //fear
        else if (fear_sec > 0)
        {
            //Лево
            if (left && !right)
                faceImage.sprite = fear_left;
            //Право
            else if (!left && right)
                faceImage.sprite = fear_right;
            //Центр
            else if (!left && !right)
                faceImage.sprite = fear;
        }
        //Агресия
        else if (aggress_sec > 0)
        {
            //Лево
            if (left && !right)
                faceImage.sprite = aggress_left;
            //Право
            else if (!left && right)
                faceImage.sprite = aggress_right;
            //Центр
            else if (!left && !right)
                faceImage.sprite = aggress;
        }
    }

}
