using UnityEngine;
using System.Collections;

public class PieceBehavior : MonoBehaviour {

	public float health = 100;
	public float damagePerForce = 5f;
	public float startConHealh = 100;
	public float endConHealth = 100;
	public float damageConPerForce = 5f;
	public float damageConThreshold = 50;
	private DistanceJoint2D startConnector;
	private DistanceJoint2D endConnector;
	private Rigidbody2D rb;
	private SpriteRenderer sRenderer;

	// Use this for initialization
	void Start () {
		rb = gameObject.GetComponent<Rigidbody2D>();
		sRenderer = gameObject.GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		if (health <= 75) {
			sRenderer.color = new Color(0.75f, 0.75f, 0.75f, 1f);
		}
		if (health <= 50) {
			sRenderer.color = new Color(0.50f, 0.50f, 0.50f, 1f);
		}
		if (health <= 25) {
			sRenderer.color = new Color(0.25f, 0.25f, 0.25f, 1f);
		}
	}

	void FixedUpdate () {
		if (rb) {
			if (!rb.isKinematic)
				CheckConnectorsForce();
		}

	}	

	void OnCollisionEnter2D (Collision2D col) {

		if (col != null) {
			// Piece Damage
			float mass = col.rigidbody.mass;
			float colVelocity = col.relativeVelocity.magnitude;
			float colForce = mass * colVelocity;
			float damage = damagePerForce * colForce;
			health -= damage;

			Debug.Log ("Piece damage: " + damage);

			if (health <= 0) {
				Vector2 reactionForce = col.relativeVelocity * mass * 0.4f;
				Debug.Log(reactionForce);
				col.rigidbody.AddForce(reactionForce,ForceMode2D.Impulse);
				Kill ();
			}
		}
	}

	void CheckConnectorsForce () {
		float startConReactionForce ;
		float endConReactionForce;
		float damage;

		//Start connector damage
		if (startConnector.enabled == true) {
			startConReactionForce = startConnector.GetReactionForce(Time.fixedDeltaTime).magnitude;
			damage = startConReactionForce * damageConPerForce;
			if (damage >= damageConThreshold) {
				startConHealh -= damage;
				Debug.Log ("Connector damage: " + damage);
			}

			if (startConHealh <= 0) {
				startConnector.enabled = false;
				Debug.Log ("Connector destroyed");
			}
		}

		//End connector damage
		if (endConnector == true) {
			endConReactionForce = endConnector.GetReactionForce(Time.fixedDeltaTime).magnitude;
			damage = endConReactionForce * damageConPerForce;
			if (damage >= damageConThreshold) {
				endConHealth -= damage;
				Debug.Log ("Connector damage: " + damage);
			}

			if (endConHealth <= 0) {
				endConnector.enabled = false;
				Debug.Log ("Connector destroyed");
			}
		}
	}

	private void Kill () {

		if (startConnector != null)
			startConnector.enabled = false;
		if (endConnector != null)
			endConnector.enabled = false;
		gameObject.SetActive(false);
	}

	public void SetStartConnector (DistanceJoint2D joint) {
		startConnector = joint;
	}

	public void SetEndConnector (DistanceJoint2D joint) {
		endConnector = joint;
	}

}
