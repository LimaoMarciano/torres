using UnityEngine;
using System.Collections;

public class ScrewCreator : MonoBehaviour {
	public GameObject screwPrefab;
	public GameObject cursorPrefab;
	private GameObject cursor;
	private Vector2 mousePosition;
	private GameObject pieceConnected;

	private bool isConstructionAllowed = false;
	private GameObject[] pieces;

	// Use this for initialization
	void Start () {
		cursor = Instantiate (cursorPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		cursor.transform.SetParent (gameObject.transform);
		cursor.SetActive (false);
	}

	void OnEnable () {
		pieces = GameObject.FindGameObjectsWithTag ("Piece");
	}

	// Update is called once per frame
	void Update () {
		Collider2D[] colliders;

		mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		colliders = Physics2D.OverlapPointAll(mousePosition, LayerMask.GetMask("Pieces"));

		//Reset pieces color
		foreach (GameObject go in pieces) {
			go.GetComponent<SpriteRenderer> ().color = new Color (1, 1, 1, 1);
		}

		if (colliders.Length > 1) {
			isConstructionAllowed = true;
			cursor.SetActive (true);
			cursor.transform.position = mousePosition;

			//Paint target pieces
			foreach (Collider2D col in colliders) {
				col.gameObject.GetComponent<SpriteRenderer> ().color = new Color (0, 1, 0, 1);
			}

		} else {
			isConstructionAllowed = false;
			cursor.SetActive (false);
		}

		if (Input.GetMouseButtonDown (0) && isConstructionAllowed) {
			CreateScrew (mousePosition);
			GameManager.instance.vehicleData.CreatePiece (mousePosition, Vector2.zero, "screw");
		}
	}

	public void CreateScrew (Vector2 position) {
		HingeJoint2D joint;
		HingeJoint2D[] joints;
		Collider2D[] connectedObjs;
		Vector3 positionVector3 = new Vector3 (position.x, position.y, 0);
		connectedObjs = Physics2D.OverlapPointAll(position, LayerMask.GetMask("Pieces"));
		GameObject targetObj = connectedObjs[0].gameObject;

		foreach (Collider2D connectedObj in connectedObjs) {
			if (connectedObj.gameObject != targetObj) {
				targetObj.AddComponent<HingeJoint2D> ();

				joints = targetObj.GetComponents<HingeJoint2D> ();
				joint = joints [joints.Length - 1];
				joint.connectedBody = connectedObj.gameObject.GetComponent<Rigidbody2D> ();

				joint.anchor = targetObj.transform.InverseTransformPoint (positionVector3);
				joint.connectedAnchor = connectedObj.transform.InverseTransformPoint (positionVector3);

				joint.breakForce = 500;
			}
		}

		GameObject sprite = Instantiate (screwPrefab, position, Quaternion.identity) as GameObject;
		sprite.transform.SetParent (targetObj.transform);
	}
}
