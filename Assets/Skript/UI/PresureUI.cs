using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PresureUI : MonoBehaviour {

    [SerializeField]
    Player player;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<Text>().text = System.Convert.ToString(player.ScoreTime);
    }
}
