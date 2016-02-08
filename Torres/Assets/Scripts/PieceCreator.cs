using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class PieceCreator : MonoBehaviour {

	public GameObject piecePrefab;
	public GameObject cursorPrefab;
	private GameObject cursor;

	private enum State {
		setStartPoint,
		setEndPoint
	};
		
	private State state;
	private Vector2 mousePosition;
	private Vector2 startPoint;
	private Vector2 endPoint;
	private GameObject piece;
	private EventSystem eventSystem;
	private bool isCursorEnabled = false;

	// Use this for initialization
	void Start () {
		//Event system used to detect UI clicks
		eventSystem = EventSystem.current;

		//Create construction guide cursor
		cursor = Instantiate (cursorPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		cursor.transform.SetParent (gameObject.transform);
		cursor.SetActive (false);
	}

	void OnEnable () {
		state = State.setStartPoint;
	}
	
	// Update is called once per frame
	void Update() {
		switch (state) {
		//State to define the piece start point
		case State.setStartPoint:

			if (Input.GetMouseButtonDown (0)) {

				//Check if clicked on UI and do nothing if so
				if (eventSystem.IsPointerOverGameObject ()) {
					return;
				}

				//Set the piece start point
				startPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				state = State.setEndPoint;
			}

			break;
		
		//State to define the piece end position
		case State.setEndPoint:

			mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			isCursorEnabled = true;

			if (Input.GetMouseButtonDown (0)) {

				//Check if clicked on UI and cancel operation if so
				if (eventSystem.IsPointerOverGameObject ()) {
					isCursorEnabled = false;
					state = State.setStartPoint;
					return;
				}

				//Set end point and creates piece
				endPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				isCursorEnabled = false;
				CreatePiece (startPoint, endPoint);

				//Change to initial state
				state = State.setStartPoint;
			}

			break;
		}

		//Update construction guide if enabled
		if (isCursorEnabled) {
			cursor.SetActive (true);
			UpdateCursor (startPoint, mousePosition);
		} else {
			cursor.SetActive (false);
		}
	}

	/// <summary>
	/// Creates a piece between two coordinates.
	/// </summary>
	/// <param name="startPos">Start position.</param>
	/// <param name="endPos">End position.</param>
	public void CreatePiece(Vector2 startPos, Vector2 endPos) {
		piece = Instantiate (piecePrefab, startPoint, Quaternion.identity) as GameObject;
		StrecthSprite (piece, startPos, endPos);
		piece.GetComponent<Rigidbody2D> ().isKinematic = true;

		GameManager.instance.vehicleData.CreatePiece (startPos, endPos);
	}

	public void LoadPiece(Vector2 startPos, Vector2 endPos) {
		piece = Instantiate (piecePrefab, startPoint, Quaternion.identity) as GameObject;
		StrecthSprite (piece, startPos, endPos);
		piece.GetComponent<Rigidbody2D> ().isKinematic = true;
	}

	/// <summary>
	/// Strecths sprite to fit between two coordinates.
	/// </summary>
	/// <param name="sprite">Sprite.</param>
	/// <param name="startPos">Start position.</param>
	/// <param name="endPos">End position.</param>
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

	private void UpdateCursor (Vector2 startPos, Vector2 endPos) {
		StrecthSprite (cursor, startPos, endPos);
	}
}
