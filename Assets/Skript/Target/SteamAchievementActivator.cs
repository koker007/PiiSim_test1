using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Применяется на цели и активинует ачивки с их смертью
public class SteamAchievementActivator : MonoBehaviour {

    steam_achievement steam_Achievement;
    PiiTarget piiTarget;
    bool dead_old = false;

    public bool compromising_material = false;
    public bool fighter = false;
    public bool scoolbus = false;
    public bool doggyStyle = false;

    // Use this for initialization
    void Start()
    {
        setSteamAchievent();
        setPiiTarget();
    }

        // Update is called once per frame
    void Update () {
        test_acievent();
    }

    void setSteamAchievent() {
        if (steam_Achievement == null) {
            steam_Achievement = GameObject.FindWithTag("steam_manager").GetComponent<steam_achievement>();
        }
    }
    void setPiiTarget() {
        if (piiTarget == null) {
            piiTarget = gameObject.GetComponent<PiiTarget>();
        }
    }

    void test_acievent() {
        if (steam_Achievement != null && piiTarget != null && piiTarget.heath <= 0 && !dead_old) {
            dead_old = true;

            if (compromising_material)
                steam_Achievement.compromising_material();

            if (fighter)
                steam_Achievement.destroy_fighter();

            if (scoolbus)
                steam_Achievement.scoolbus();

            if (doggyStyle)
                steam_Achievement.doggyStyle();

        }
    }

}
