using UnityEngine;
using System.Collections;

public class SlingDragable : MonoBehaviour {
	
	public float maxStretch = 1.5f;

	private SpringJoint2D spring;
	private Rigidbody2D rb;
	private Transform catapult;
	private bool clickedOn = false;
	private Ray rayToMouse;
	private float maxStretchSqr;
	private Vector2 prevVelocity;

	void Awake () {
		spring = GetComponent<SpringJoint2D> ();
		rb = GetComponent<Rigidbody2D> ();
		catapult = spring.connectedBody.transform;
		maxStretchSqr = maxStretch * maxStretch;
	}

	// Use this for initialization
	void Start () {
		rayToMouse = new Ray (catapult.position, Vector3.zero);
	}
	
	// Update is called once per frame
	void Update () {
		if (clickedOn) {
			Dragging ();
		}

		if (spring != null) {
			if (!rb.isKinematic && prevVelocity.sqrMagnitude > rb.velocity.sqrMagnitude) {
				Destroy (spring);
				rb.velocity = prevVelocity;
			}

			if (!clickedOn) {
				prevVelocity = rb.velocity;
			}

		} else {

		}
	}

	void OnMouseDown () {
		spring.enabled = false;
		clickedOn = true;
	}

	void OnMouseUp () {
		spring.enabled = true;
		rb.isKinematic = false;
		clickedOn = false;
	}

	void Dragging() {
		Vector3 mouseWorldPoint = Camera.main.ScreenToWorldPoint (Input.mousePosition);


		Vector2 catapultToMouse = mouseWorldPoint - catapult.position;

		if (catapultToMouse.sqrMagnitude > maxStretchSqr) {
			rayToMouse.direction = catapultToMouse;
			mouseWorldPoint = rayToMouse.GetPoint (maxStretch);
		}

		mouseWorldPoint.z = 0;
		transform.position = mouseWorldPoint;

	}
		
}
