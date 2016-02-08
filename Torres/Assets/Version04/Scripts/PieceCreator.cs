using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class PieceCreator : MonoBehaviour {

	public GameObject piecePrefab;

	private enum State {
		setStartPoint,
		adjustPiece,
		setEndPoint
	};
		
	private State state;
	private Vector2 mousePosition;
	private Vector2 startPoint;
	private Vector2 endPoint;
	private GameObject piece;
	private EventSystem eventSystem;

	// Use this for initialization
	void Start () {
		eventSystem = EventSystem.current;
	}

	void OnEnable () {
		state = State.setStartPoint;
	}
	
	// Update is called once per frame
	void Update() {
		switch (state) {
		case State.setStartPoint:

			if (Input.GetMouseButtonDown (0)) {

				//Check if is clicking on UI
				if (eventSystem.IsPointerOverGameObject ()) {
					return;
				}

				startPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				state = State.setEndPoint;
				CreatePiece ();
			}

			break;

		case State.setEndPoint:

			if (!piece.GetComponent<SpriteRenderer> ().enabled) {
				piece.GetComponent<SpriteRenderer> ().enabled = true;
			}

			mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			StrecthSprite (piece, startPoint, mousePosition);

			if (Input.GetMouseButtonDown (0)) {

				//Check if is clicking on UI
				if (eventSystem.IsPointerOverGameObject ()) {
					GameObject.Destroy (piece);
					state = State.setStartPoint;
					return;
				}
				endPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				state = State.setStartPoint;
			}

			break;
		}
	}

	void CreatePiece() {
		piece = Instantiate (piecePrefab, startPoint, Quaternion.identity) as GameObject;
		piece.GetComponent<SpriteRenderer> ().enabled = false;
		piece.GetComponent<Rigidbody2D> ().isKinematic = true;
	}

	private void StrecthSprite (GameObject sprite, Vector2 startPos, Vector2 endPos) {
		Vector2 centerPos = (startPos + endPos) / 2f;
		sprite.transform.position = centerPos;
		Vector2 direction = startPos - endPos;
		direction = direction.normalized;
		sprite.transform.right = direction;

		if (endPos.x >= startPos.x) {
			sprite.transform.right *= -1f;
		}
		Vector2 scale = new Vector2(1,1);
		scale.x = Vector2.Distance(startPos, endPos);
		sprite.transform.localScale = new Vector3(scale.x, scale.y, 1);

	}
}
