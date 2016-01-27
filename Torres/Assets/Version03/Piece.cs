using UnityEngine;
using System.Collections;

public class Piece : MonoBehaviour {

	public Transform start;
	public Transform end;
	public float health = 100;
	public float damagePerForce = 5f;
	public DistanceJoint2D joint;

	private SpriteRenderer sRenderer;

	// Use this for initialization
	void Start () {
		Vector2 startPos = new Vector2 (start.position.x, start.position.y);
		Vector2 endPos = new Vector2 (end.position.x, end.position.y);
		StrecthSprite (startPos, endPos);
		sRenderer = GetComponent<SpriteRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (start && end) {
			Vector2 startPos = new Vector2 (start.position.x, start.position.y);
			Vector2 endPos = new Vector2 (end.position.x, end.position.y);
			StrecthSprite (startPos, endPos);
		}

		if (health <= 75) {
			sRenderer.color = new Color(0.75f, 0.75f, 0.75f, 1f);
		}
		if (health <= 50) {
			sRenderer.color = new Color(0.50f, 0.50f, 0.50f, 1f);
		}
		if (health <= 25) {
			sRenderer.color = new Color(0.25f, 0.25f, 0.25f, 1f);
		}

		if (transform.position.y <= -10) {
			Kill();
		}
	}

	private void StrecthSprite (Vector2 startPos, Vector2 endPos) {

		Vector2 centerPos = (startPos + endPos) / 2f;
		gameObject.transform.position = centerPos;
		Vector2 direction = startPos - endPos;
		direction = direction.normalized;
		gameObject.transform.right = direction;

		if (endPos.x >= startPos.x) {
			gameObject.transform.right *= -1f;
		}
		Vector2 scale = new Vector2(1,1);
		scale.x = Vector2.Distance(startPos, endPos);
		gameObject.transform.localScale = scale;

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
				Vector2 reactionForce = col.relativeVelocity * mass * 0.2f;
				Debug.Log(reactionForce);
				col.rigidbody.AddForce(reactionForce,ForceMode2D.Impulse);
				Kill ();
			}
		}
	}

	private void Kill () {
		gameObject.SetActive(false);
		joint.enabled = false;
	}
}
