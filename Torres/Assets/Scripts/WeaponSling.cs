using UnityEngine;
using System.Collections;

public class WeaponSling : MonoBehaviour {

	public GameObject ammoPrefab;
	private GameObject ammoObj;
	public Transform initialPos;
	public float maxStrecht = 1.5f;
	private SlingAmmo ammo;
	private Rigidbody2D ammoRb;
	private SpringJoint2D spring;
	private bool clickedOn;
	private bool isLoaded;
	private Ray rayToMouse;
	private float maxStrechtSqr;
	private Vector2 prevVelocity;

	void Awake() {
		
		spring = GetComponent<SpringJoint2D> ();
		maxStrechtSqr = maxStrecht * maxStrecht;
		Reload ();
	}

	// Use this for initialization
	void Start () {
		rayToMouse = new Ray (transform.position, Vector3.zero);
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.R)) {
			Reload ();
		}

		if (clickedOn) {
			Dragging ();
		}

		if (spring.enabled) {
			if (!ammoRb.isKinematic && prevVelocity.sqrMagnitude > ammoRb.velocity.sqrMagnitude) {
				spring.enabled = false;
				ammoRb.velocity = prevVelocity;
				ammo.isReleased = true;
				isLoaded = false;
			}

			if (!clickedOn) {
				prevVelocity = ammoRb.velocity;
			}

		}
	
	}

	public void HoldingAmmo () {
		spring.enabled = false;
		clickedOn = true;
	}

	public void NotHoldingAmmo () {
		spring.enabled = true;
		ammoRb.isKinematic = false;
		clickedOn = false;
	}

	private void Dragging () {
		Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		Vector2 mouseToCatapult = mouseWorldPosition - transform.position;

		if (mouseToCatapult.sqrMagnitude > maxStrechtSqr) {
			rayToMouse.direction = mouseToCatapult;
			mouseWorldPosition = rayToMouse.GetPoint (maxStrecht);
		}

		mouseWorldPosition.z = 0;
		ammoRb.transform.position = mouseWorldPosition;
	}

	private void Reload () {
		if (!isLoaded) {
			ammoObj = Instantiate (ammoPrefab, initialPos.position, Quaternion.identity) as GameObject;
			ammoRb = ammoObj.GetComponent<Rigidbody2D> ();
			ammo = ammoObj.GetComponent<SlingAmmo> ();
			ammo.weapon = gameObject.GetComponent<WeaponSling> ();

			ammoRb.isKinematic = true;
			spring.enabled = true;
			spring.connectedBody = ammoRb;
			isLoaded = true;
		} else {
			Debug.Log ("Catapult is already loaded");
		}
	}
}
