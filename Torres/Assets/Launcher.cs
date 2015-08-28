using UnityEngine;
using System.Collections;

public class Launcher : MonoBehaviour {

	public Vector2 angle;
	public float power = 500;
	private bool isShoot = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate () {
		if (!isShoot && !gameObject.GetComponent<Rigidbody2D>().isKinematic) {
			gameObject.GetComponent<Rigidbody2D>().AddForce(angle * power);
			isShoot = true;
		}
	}
}
