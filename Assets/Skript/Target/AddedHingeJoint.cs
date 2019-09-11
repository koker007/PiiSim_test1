using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddedHingeJoint : MonoBehaviour {

    bool Added = false;
    [SerializeField]
    Vector3 axis;
	
	// Update is called once per frame
	void Update () {
        if (!Added) {
            Added = true;

            HingeJoint hingeJoint = gameObject.GetComponent<HingeJoint>();
            if (hingeJoint == null) {
                hingeJoint = gameObject.AddComponent<HingeJoint>();
            }
            if (hingeJoint != null) {
                hingeJoint.axis = axis;

                Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
                if (rigidbody != null) {
                    rigidbody.constraints = RigidbodyConstraints.None;
                }
            }

            AddedHingeJoint addedHingeJoint = gameObject.GetComponent<AddedHingeJoint>();
            GameObject.Destroy(addedHingeJoint);
        }	
	}
}
