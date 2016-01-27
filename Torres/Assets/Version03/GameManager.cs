using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public GameObject jointPrefab;
	public GameObject piecePrefab;
	public float gridSize = 1;


	private enum State {
		waitingInput,
		creatingPiece,
		finishingPiece
	};

	private State state;
	private Vector2 clickPosition;
	private Vector2 snappedPosition;

	private StructureJoint startJoint;
	private StructureJoint endJoint;

	// Use this for initialization
	void Start () {
		state = State.waitingInput;
	}
	
	// Update is called once per frame
	void Update () {
	
		switch(state) {
		case State.waitingInput:
			if (Input.GetMouseButtonDown(0)) {

				clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				snappedPosition = SnapToGrid(clickPosition);

				Collider2D[] col = Physics2D.OverlapPointAll(snappedPosition, LayerMask.GetMask("Connectors"));

				if (col.Length != 0) {
					startJoint = col[0].gameObject.GetComponent<StructureJoint>();
					Debug.Log("Selected " + col[0].gameObject.name + " as start point");
				}
				else {
					Debug.Log("Nothing was selected");
				}
				state = State.creatingPiece;
			}
			break;

		case State.creatingPiece:
			if (Input.GetMouseButtonDown(0)) {

				clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				snappedPosition = SnapToGrid(clickPosition);
				Collider2D[] col = Physics2D.OverlapPointAll(snappedPosition, LayerMask.GetMask("Connectors"));

				if (col.Length != 0) {
					endJoint = col[0].gameObject.GetComponent<StructureJoint>();
					AddDistanceJoint(startJoint, endJoint);
				}
				else {
					GameObject connector = CreateConnector(ConvertToVector3(snappedPosition));
					endJoint = connector.GetComponent<StructureJoint>();
					AddDistanceJoint(startJoint, endJoint);
				}

				CreatePiece(startJoint, endJoint);

				state = State.waitingInput;
			}
			break;

		}

		if (Input.GetKeyDown(KeyCode.Space)) {

			Debug.Log("Activating physics");

			GameObject[] connectorList;
			connectorList = GameObject.FindGameObjectsWithTag("Connector");
			foreach (GameObject connector in connectorList) {
				connector.GetComponent<Rigidbody2D>().isKinematic = false;
			}
		}

	}


	private Vector2 ConvertToVector2 (Vector3 vector) {
		Vector2 convertedVector;
		convertedVector = new Vector2 (vector.x, vector.y);
		return convertedVector;
	}

	private Vector3 ConvertToVector3 (Vector2 vector) {
		Vector3 convertedVector;
		convertedVector = new Vector3 (vector.x, vector.y, 0);
		return convertedVector;
	}

	private GameObject CreateConnector (Vector3 position) {
		GameObject connector;
		connector = Instantiate(jointPrefab, position, Quaternion.identity) as GameObject;
		return connector;
	}

	private void CreatePiece (StructureJoint start, StructureJoint end) {
		GameObject piece = Instantiate(piecePrefab, Vector3.zero, Quaternion.identity) as GameObject;
		Piece pieceC = piece.GetComponent<Piece>();
		pieceC.start = start.transform;
		pieceC.end = end.transform;
	}

	private void AddFixedJoints (StructureJoint start, StructureJoint end) {
		start.gameObject.AddComponent<FixedJoint2D>();
		FixedJoint2D[] jointList = start.gameObject.GetComponents<FixedJoint2D>();
		//JointAngleLimits2D limits;

		FixedJoint2D joint = jointList[jointList.Length - 1];

		joint.autoConfigureConnectedAnchor = false;
		joint.connectedBody = end.gameObject.GetComponent<Rigidbody2D>();
		joint.anchor = Vector2.zero;
		joint.dampingRatio = 1;
		joint.frequency = 1;

		Vector3 endPosition = start.transform.InverseTransformPoint(end.transform.position) * -1;
		joint.connectedAnchor = new Vector2 (endPosition.x, endPosition.y);
	}

	private void AddDistanceJoint (StructureJoint start, StructureJoint end) {
		start.gameObject.AddComponent<DistanceJoint2D>();
		DistanceJoint2D[] jointList = start.gameObject.GetComponents<DistanceJoint2D>();
		DistanceJoint2D joint = jointList[jointList.Length - 1];

		joint.autoConfigureConnectedAnchor = false;
		joint.connectedBody = end.gameObject.GetComponent<Rigidbody2D>();

		joint.anchor = Vector2.zero;
		joint.connectedAnchor = Vector2.zero;

		joint.distance = Vector3.Distance(start.gameObject.transform.position, end.gameObject.transform.position);

	}

	private Vector2 SnapToGrid (Vector2 pos) {

		Vector2 snappedPos;

		snappedPos.x = Mathf.Round (pos.x / gridSize);
		snappedPos.y = Mathf.Round (pos.y / gridSize);

		snappedPos.x *= gridSize;
		snappedPos.y *= gridSize;

		return snappedPos;
	}
}
