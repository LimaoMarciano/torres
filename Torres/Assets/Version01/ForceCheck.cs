using UnityEngine;
using System.Collections;

public class ForceCheck : MonoBehaviour {

	DistanceJoint2D joint;

	// Use this for initialization
	void Start () {
		joint = gameObject.GetComponent<DistanceJoint2D>();
	}
	
	// Update is called once per frame
	void Update () {
		print ("Joint force: " + joint.GetReactionForce(0.03f));
		Vector2 force = joint.GetReactionForce(0.03f);
		if (force.x > 20 || force.y > 20) {
			joint.enabled = false;
		}
	}
}
