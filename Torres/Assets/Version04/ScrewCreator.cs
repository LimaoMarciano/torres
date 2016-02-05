using UnityEngine;
using System.Collections;

public class ScrewCreator : MonoBehaviour {
	public GameObject screwPrefab;
	private Vector2 mousePosition;
	private GameObject pieceTarget;
	private GameObject pieceConnected;

	private bool isConstructionAllowed = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Collider2D[] colliders;

		mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		colliders = Physics2D.OverlapPointAll(mousePosition, LayerMask.GetMask("Pieces"));

		if (colliders.Length > 1) {
			isConstructionAllowed = true;
		} else {
			isConstructionAllowed = false;
		}

		if (Input.GetMouseButtonDown (0) && isConstructionAllowed) {
			pieceTarget = colliders [0].gameObject;
			for (int i = 1; i < colliders.Length; i++) {
				pieceConnected = colliders [i].gameObject;
				CreateScrew (pieceTarget, pieceConnected, mousePosition);
			}
			GameObject sprite = Instantiate (screwPrefab, mousePosition, Quaternion.identity) as GameObject;
			sprite.transform.SetParent (pieceTarget.transform);

		}
	}

	private void CreateScrew (GameObject targetObj, GameObject connectedObj, Vector2 position) {
		HingeJoint2D joint;
		HingeJoint2D[] joints;
		targetObj.AddComponent<HingeJoint2D> ();

		joints = targetObj.GetComponents<HingeJoint2D> ();
		joint = joints[joints.Length - 1];
		joint.connectedBody = connectedObj.GetComponent<Rigidbody2D>();

		Vector3 mousePositionVector3 = new Vector3 (mousePosition.x, mousePosition.y, 0);
		joint.anchor = targetObj.transform.InverseTransformPoint (mousePositionVector3);
		joint.connectedAnchor = connectedObj.transform.InverseTransformPoint (mousePositionVector3);

		joint.breakForce = 500;
	}
}
