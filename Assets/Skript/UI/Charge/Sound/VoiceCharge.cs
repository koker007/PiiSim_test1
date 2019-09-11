using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceCharge : MonoBehaviour {

    [SerializeField]
    VoiceCtrl Voice;

    [SerializeField]
    AudioClip[] end;

    [SerializeField]
    AudioClip[] process;


    public void playProcess() {
        if (Voice != null && !Voice.is_playing_now() && process != null && process.Length != 0) {
            if (Voice.get_and_play_clip(process[Random.Range(0, process.Length - 1)])) {
                Voice.get_and_play_clip(process[Random.Range(0, process.Length - 1)]);
            }
            Voice.get_and_play_clip(process[Random.Range(0,process.Length -1)]);
        }
    }
    public void playEnd() {
        if (Voice != null && process != null && process.Length != 0) {
            if (Voice.get_and_play_clip(end[Random.Range(0, process.Length - 1)])) {
                Voice.get_and_play_clip(end[Random.Range(0, process.Length - 1)]);
            }
        }
    }
}
