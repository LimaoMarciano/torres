using UnityEngine;
using System.Collections;

public class PieceBehaviour : MonoBehaviour {

	public float forceResistence = 30;
	public float damagePerForce = 5;
	private float health = 100;
	private SpriteRenderer spriteRenderer;


	// Use this for initialization
	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter2D (Collision2D collision) {
		float colVelocity;
		float colMass;
		float impactForce;
		float damage;

		colVelocity = collision.relativeVelocity.magnitude;
		colMass = collision.rigidbody.mass;
		impactForce = colVelocity * colMass;
		damage = Mathf.Clamp(impactForce - forceResistence, 0, 100) * damagePerForce;

		Damage(damage);
		Debug.Log ("Impact force: " + impactForce);

	}

	void Damage (float damage) {
		health -= damage;

		if (health <= 75) {
			spriteRenderer.color = new Color (0.25f, 0.25f, 0.25f, 1);
		}

		if (health <= 50) {
			spriteRenderer.color = new Color (0.5f, 0.5f, 0.5f, 1);
		}

		if (health <= 25) {
			spriteRenderer.color = new Color (0.75f, 0.75f, 0.75f, 1);
		}

		if (health <= 0) {
			Destroy(gameObject);
		}
	}
}
