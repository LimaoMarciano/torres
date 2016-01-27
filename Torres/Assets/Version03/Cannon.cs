using UnityEngine;
using System.Collections;

public class Cannon : MonoBehaviour {

	public Vector2 direction;
	public float power;
	private Rigidbody2D rb;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		rb.isKinematic = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			rb.isKinematic = false;
			rb.AddForce (direction * power, ForceMode2D.Impulse);
		}

	}
}
