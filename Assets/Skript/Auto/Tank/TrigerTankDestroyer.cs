using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrigerTankDestroyer : MonoBehaviour {
    

    private void OnTriggerEnter(Collider other)
    {
        PiiTarget target = other.GetComponent<PiiTarget>();
        PiiTarget piiTarget = gameObject.GetComponentInParent<PiiTarget>();
        if (target != null && piiTarget != null && piiTarget != target && target.heath > 0 && target.MinPresure < (piiTarget.MinPresure))
        {
            //target.TrigerPii(Presure, faceController, piiController, score_Text_3D.gameObject);
            target.heath -= piiTarget.MinPresure * 500 * Time.deltaTime;
            if (target.heath <= 0)
            {
                target.DeathTarget();
            }
        }
    }
}
