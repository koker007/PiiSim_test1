using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DownPresure : MonoBehaviour {

    setings seting;
    void iniSetings() {
        if (seting == null) {
            seting = GameObject.FindGameObjectWithTag("setings_game").GetComponent<setings>();
        }
    }

    Player player;
    void iniPlayer() {
        if (player == null) {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }
    }

    Slider PresureSlider;
    void iniPresureSlider() {
        if (PresureSlider == null) {
            PresureSlider = gameObject.GetComponent<Slider>();
        }
    }

    [SerializeField]
    Text MoveText;
    [SerializeField]
    Color Red;
    [SerializeField]
    Color Green;

    float timeToAdd = 0;
    float[] presureHistory = new float[20];
    float presureSrednee = 0;

    [SerializeField]
    Image Attention;
    [SerializeField]
    AudioClip AttentionClip;
    AudioSource AttentionSource;
    void iniAttentionSource() {
        if (seting != null && AttentionSource == null && AttentionClip != null) {
            AttentionSource = gameObject.AddComponent<AudioSource>();
            AttentionSource.volume = 0;
            AttentionSource.spatialBlend = 0;
            AttentionSource.priority = 1;
        }
    }

    [SerializeField]
    RawImage ImageFull;

	// Use this for initialization
	void Start () {
        iniSetings();
        iniPlayer();
        iniPresureSlider();
        iniAttentionSource();
    }
	
	// Update is called once per frame
	void Update () {
        CalcPresure();
        TestTextMove();
        TestAttention();
        TestImageFull();
    }

    void AddNewPresureToHistory(float valueFunc) {
        if (presureHistory.Length != 0) {
            for (int num = 0; num < presureHistory.Length; num++) {
                if (num + 1 != presureHistory.Length)
                {
                    presureHistory[num] = presureHistory[num + 1];
                }
                else {
                    presureHistory[num] = valueFunc;
                }
            }

            presureSrednee = 0;
            for (int num = 0; num < presureHistory.Length; num++) {
                presureSrednee += presureHistory[num];
            }
            presureSrednee /= presureHistory.Length;
        }
    }

    void CalcPresure() {
        if (player != null && PresureSlider != null) {
            PresureSlider.maxValue = player.maxPresure;
            PresureSlider.value = player.bolt.GetComponent<PiiController>().speed_pii;

            timeToAdd += Time.deltaTime;
            if (timeToAdd >= 0.1f) {
                timeToAdd -= 0.1f;

                AddNewPresureToHistory(PresureSlider.value);
            }
        }
    }

    void TestTextMove() {
        if (MoveText != null && presureSrednee > 0) {
            float Smehenie = PresureSlider.value - presureSrednee;
            //Debug.Log("PresureSmehenie" + Smehenie);
            //Сперва настраиваем цвет
            if (Smehenie < 0)
            {
                MoveText.color = Red;

                if (Smehenie < -0.3f)
                {
                    MoveText.text = "<<<";
                }
                else if (Smehenie < -0.1f)
                {
                    MoveText.text = "<<";
                }
                else if (Smehenie < -0.005f)
                {
                    MoveText.text = "<";
                }
                else {
                    MoveText.text = " ";
                }
            }
            else {
                MoveText.color = Green;

                if (Smehenie > 0.3f)
                {
                    MoveText.text = ">>>";
                }
                else if (Smehenie > 0.1f)
                {
                    MoveText.text = ">>";
                }
                else if (Smehenie > 0.005f)
                {
                    MoveText.text = ">";
                }
                else
                {
                    MoveText.text = " ";
                }
            }
        }
    }

    void TestAttention() {
        float Smehenie = PresureSlider.value - presureSrednee;
        if (Attention != null) {
            Color NewColor = Attention.color;
            if (NewColor.a > 0) {
                NewColor.a -= Time.deltaTime;
            }
            if (NewColor.a < 0)
                NewColor.a = 0;

            if (NewColor.a <= 0 && Smehenie < -0.005f) {
                NewColor.a = 1;

                if (AttentionSource != null) {
                    AttentionSource.volume = seting.game.volume_all * seting.game.volume_sound;
                    AttentionSource.PlayOneShot(AttentionClip);
                }
            }

            Attention.color = NewColor;
        }
    }

    void TestImageFull() {
        if (ImageFull != null && PresureSlider != null) {
            Rect NewRect = ImageFull.uvRect;
            NewRect.x -= PresureSlider.value * 0.1f * Time.deltaTime;
            NewRect.width = PresureSlider.value / PresureSlider.maxValue * 4;

            ImageFull.uvRect = NewRect;
        }
    }
}
