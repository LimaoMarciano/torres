using UnityEngine;
using System.Collections;

public class ConstructionManager : MonoBehaviour {

	public static ConstructionManager Instance {get; private set;}

	public GameObject woodPiece;
	public GameObject connector;
	public float gridSize = 1;

	private GameObject startConnector;
	private GameObject endConnector;
	private Vector2 clickPos;
	private Vector2 constructionPos;

	private GameObject piece;
	private int pieceIndex = 0;
	private int connectorIndex = 0;

	private enum State {
		WaitingInput, 
		Contruction,
		Positioning
	};

	private State state;

	// Use this for initialization
	void Start () {
		state = State.WaitingInput;
	}
	
	// Update is called once per frame
	void Update () {

		switch (state) {
		case State.WaitingInput:

			//Start construction mode if mouse is clicked
			if (InputManager.Instance.isMouseClicked) {
				clickPos = InputManager.Instance.GetMousePositon();
				state = State.Contruction;
			}

			break;

		//This state initializes the construction process, creating the starting connector
		case State.Contruction:

			Collider2D[] col = Physics2D.OverlapPointAll(SnapToGrid(clickPos), LayerMask.GetMask("Connectors"));

			//Check if start position is a connector and create a new one if it's not
			if (col.Length > 0) {
				startConnector = col[0].gameObject;
				Debug.Log ("Clicked in a connection. Setting " + startConnector.name + " as start connector.");
			}
			else
			{
				startConnector = CreateConnector(SnapToGrid(clickPos));
				Debug.Log ("Clicked in a empty space. Creating start connector " + startConnector.name + ".");
			}

			//Create piece
			piece = CreatePiece(Vector2.zero);

			//Change state
			state = State.Positioning;

			break;
		
		//This state positions the piece end point and create the end connector
		case State.Positioning:

			Vector2 connectorSize = startConnector.GetComponent<Collider2D>().bounds.size;
			Vector2 startPos;
			Vector2 endPos;
			Vector2 snappedPos;
			Vector2 pieceStartPos;
			Vector2 pieceEndPos;
			Vector2 direction;
			FixedJoint2D joint;

			piece.SetActive(true);

			startPos = startConnector.transform.position;

			//Update end position and snap it to grid
			endPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			snappedPos = SnapToGrid(endPos);

			//Postioning piece to not overlap with connectors
			direction = snappedPos - startPos;
			pieceStartPos = startPos + direction.normalized * (connectorSize.x / 2f);
			Debug.DrawLine (startPos, direction * 0.1f + startPos,Color.blue);

			direction = startPos - snappedPos;
			pieceEndPos = snappedPos + direction.normalized * (connectorSize.x / 2f);
			Debug.DrawLine (snappedPos, direction * 0.1f + snappedPos,Color.red);

			//Update sprite size
			StrecthSprite(piece, pieceStartPos, pieceEndPos);

			if (InputManager.Instance.isMouseClicked) {

				col = Physics2D.OverlapPointAll(snappedPos, LayerMask.GetMask("Connectors"));

				//Check if end position is a connector and creates a new one if it's not
				if (col.Length > 0) {
					endConnector = col[0].gameObject;
					Debug.Log ("Clicked in a connection. Setting " + endConnector.name + " as end connector.");
				}
				else {
					endConnector = CreateConnector(snappedPos);
					Debug.Log ("Clicked in a empty space. Creating end connector" + endConnector.name + ".");
				}

				//Creating joint between connector and piece start
				Vector2 connectedAnchor;
				Vector2 anchor;

				anchor = startConnector.transform.InverseTransformPoint(pieceStartPos);
				connectedAnchor = piece.transform.InverseTransformPoint(pieceStartPos);
//				joint = CreateDistanceJoint(startConnector, piece, Vector2.zero, connectedAnchor, connectorSize.x / 2f);
				//joint = CreateHingeJoint(startConnector, piece, anchor, connectedAnchor);
				joint = CreateFixedJoint(startConnector, piece, anchor, connectedAnchor);
				piece.GetComponent<PieceBehavior>().SetStartConnector(joint);

				//Creating joint between connector and piece end
				anchor = endConnector.transform.InverseTransformPoint(pieceEndPos);
				connectedAnchor = piece.transform.InverseTransformPoint(pieceEndPos);
//				joint = CreateDistanceJoint(endConnector, piece, Vector2.zero, connectedAnchor, connectorSize.x / 2f);
//				joint = CreateHingeJoint(endConnector, piece, anchor, connectedAnchor);
				joint = CreateFixedJoint(endConnector, piece, anchor, connectedAnchor);
				piece.GetComponent<PieceBehavior>().SetEndConnector(joint);

				//piece.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;

				//Change state
				state = State.WaitingInput;
			}

			break;
		}

//		switch (state) {
//		case State.WaitingInput:
//
//			if (InputManager.Instance.isMouseClicked) {
//				constructionPos = InputManager.Instance.GetMousePositon();
//				state = State.Contruction;
//			}
//			break;
//
//		case State.Contruction:
//
//			if (woodPiece != null) {
//
//				//Piece creation
//				constructionObj = Instantiate(woodPiece, new Vector2(0,0), Quaternion.identity) as GameObject;
//				constructionObj.layer = LayerMask.NameToLayer("Pieces");
//				constructionObj.tag = "Piece";
//				constructionObj.name = "Piece" + pieceIndex;
//				pieceIndex += 1;
//				constructionObj.GetComponent<Rigidbody2D>().isKinematic = true;
//				constructionObj.SetActive(false);
//				Debug.Log ("Created new piece: " + constructionObj.name);
//
//				Collider2D[] col = Physics2D.OverlapPointAll(constructionPos, LayerMask.GetMask("Connections"));
//
//				if (col.Length > 0) {
//					constructionPos = col[0].transform.position;
//					Debug.Log ("Clicked in a connection. Snapping position to connector");
//				}
//				Debug.Log ("Changing to positioning mode");
//				state = State.Positioning;
//			}
//			break;
//		
//		case State.Positioning:
//
//			Collider2D[] col;
//			Vector2 connectorAnchor;
//			Vector2 connectorPos;
//
//			Vector2 endPos = Input.mousePosition;
//			endPos = Camera.main.ScreenToWorldPoint(endPos);
//			Vector2 direction = endPos - constructionPos;
//			Vector2 startPos = constructionPos + direction.normalized * 0.05f;
//			constructionObj.SetActive(true);
//			StrechSprite(constructionObj, startPos, endPos);
//
//			if (InputManager.Instance.isMouseClicked) {
//
//				col = Physics2D.OverlapPointAll(constructionPos, LayerMask.GetMask("Connections"));
//				GameObject newConnector;
//
//				if (col.Length > 0) {
//					Debug.Log ("Start position is a connector. Creating joint");
//					GameObject connector = col[0].gameObject;
//					connectorPos = connector.transform.position;
//					direction = startPos - connectorPos;
//					connectorAnchor = connector.transform.InverseTransformPoint(connectorPos + direction);
//					CreateDistanceJoint(connector, constructionObj, connectorAnchor, new Vector2(0.5f, 0), 0.005f);
//				}
//				else
//				{
//					Debug.Log ("Start position is not a connector. Creating new connector");
//					Vector2 connectionPos = constructionObj.transform.TransformPoint(0.5f, 0, 0);
//					newConnector = CreateConnector(connectionPos, new Vector2(0.5f,0), constructionObj);
//					connectorPos = newConnector.transform.position;
//					direction = startPos - connectorPos;
//					connectorAnchor = newConnector.transform.InverseTransformPoint(connectorPos + direction);
//					CreateDistanceJoint(newConnector, constructionObj, connectorAnchor, new Vector2(0.5f, 0), 0.005f);
//				}
//
//				col = Physics2D.OverlapPointAll(endPos, LayerMask.GetMask("Connections"));
//
//				if (col.Length > 0) {
//					Debug.Log ("End position is a connector. Creating joint");
//					GameObject connector = col[0].gameObject;
//					connectorPos = connector.transform.position;
//					direction = connectorPos - startPos;
//					endPos = connectorPos + direction.normalized * -0.05f;
//					StrechSprite(constructionObj, startPos, endPos);
//
//					connectorPos = connector.transform.position;
//					direction = endPos - connectorPos;
//					connectorAnchor = connector.transform.InverseTransformPoint(connectorPos + direction);
//
//					CreateDistanceJoint(connector, constructionObj, connectorAnchor, new Vector2(-0.5f, 0), 0.005f);
//				}
//				else
//				{
//					Debug.Log ("End position is not a connector. Creating new connector");
//					Vector2 connectionPos = constructionObj.transform.TransformPoint(-0.5f, 0, 0);
//					newConnector = CreateConnector(connectionPos, new Vector2(-0.5f,0), constructionObj);
//					connectorPos = newConnector.transform.position;
//					direction = endPos - connectorPos;
//					connectorAnchor = newConnector.transform.InverseTransformPoint(connectorPos + direction);
//					CreateDistanceJoint(newConnector, constructionObj, connectorAnchor, new Vector2(-0.5f, 0), 0.005f);
//				}
//
//				Debug.Log ("Changing to initial state");
//				state = State.WaitingInput;
//			}
//			break;
//		}

	}

	/// <summary>
	/// Snaps position to grid.
	/// </summary>
	/// <returns>The to grid.</returns>
	/// <param name="pos">Position.</param>
	private Vector2 SnapToGrid (Vector2 pos) {

		Vector2 snappedPos;

		snappedPos.x = Mathf.Round (pos.x / gridSize);
		snappedPos.y = Mathf.Round (pos.y / gridSize);

		snappedPos.x *= gridSize;
		snappedPos.y *= gridSize;

		return snappedPos;
	}

	/// <summary>
	/// Strechs the sprite.
	/// </summary>
	/// <param name="sprite">Sprite.</param>
	/// <param name="startPos">Start position.</param>
	/// <param name="endPos">End position.</param>
	void StrecthSprite (GameObject sprite, Vector2 startPos, Vector2 endPos) {

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
		sprite.transform.localScale = scale;

	}

	/// <summary>
	/// Creates the connector.
	/// </summary>
	/// <param name="pos">Position.</param>
	/// <param name="anchor">Anchor.</param>
	/// <param name="obj">Object.</param>
	private GameObject CreateConnector (Vector2 pos) {
		GameObject go = Instantiate(connector, pos, Quaternion.identity) as GameObject;
		go.GetComponent<Rigidbody2D>().isKinematic = true;
		go.layer = LayerMask.NameToLayer("Connectors");
		go.tag = "Connector";
		go.name = "Connector" + pieceIndex;
		connectorIndex += 1;

		Debug.Log("Created connector at" + go.transform.position);
		return go;
	}

	/// <summary>
	/// Creates the piece.
	/// </summary>
	/// <returns>The piece.</returns>
	/// <param name="pos">Position.</param>
	private GameObject CreatePiece (Vector2 pos) {

		GameObject go;

		go = Instantiate(woodPiece, new Vector2(0,0), Quaternion.identity) as GameObject;
		go.layer = LayerMask.NameToLayer("Pieces");
		go.tag = "Piece";
		go.name = "Piece" + pieceIndex;
		pieceIndex += 1;
		go.GetComponent<Rigidbody2D>().isKinematic = true;
		go.SetActive(false);

		return go;
	}

	/// <summary>
	/// Creates the distance joint.
	/// </summary>
	/// <param name="obj">Object.</param>
	/// <param name="connectedObj">Connected object.</param>
	/// <param name="anchor">Anchor.</param>
	/// <param name="connectedAnchor">Connected anchor.</param>
	/// <param name="distance">Distance.</param>
	private DistanceJoint2D CreateDistanceJoint(GameObject obj, GameObject connectedObj, Vector2 anchor, Vector2 connectedAnchor, float distance) {

		obj.AddComponent<DistanceJoint2D>();
		DistanceJoint2D[] jointList = obj.GetComponents<DistanceJoint2D>();

		DistanceJoint2D joint = jointList[jointList.Length - 1];
		joint.connectedBody = connectedObj.GetComponent<Rigidbody2D>();
		joint.anchor = anchor;
		joint.connectedAnchor = connectedAnchor;
		joint.distance = distance;
		joint.enableCollision = true;

		return joint;
	}

	private HingeJoint2D CreateHingeJoint(GameObject obj, GameObject connectedObj, Vector2 anchor, Vector2 connectedAnchor) {
		
		obj.AddComponent<HingeJoint2D>();
		HingeJoint2D[] jointList = obj.GetComponents<HingeJoint2D>();
		JointAngleLimits2D limits;
		
		HingeJoint2D joint = jointList[jointList.Length - 1];
		limits = joint.limits;
		limits.max = 0;
		joint.limits = limits;

		joint.connectedBody = connectedObj.GetComponent<Rigidbody2D>();
		joint.anchor = anchor;
		joint.connectedAnchor = connectedAnchor;
		joint.enableCollision = true;
		
		return joint;
	}

	private FixedJoint2D CreateFixedJoint(GameObject obj, GameObject connectedObj, Vector2 anchor, Vector2 connectedAnchor) {

		obj.AddComponent<FixedJoint2D>();
		FixedJoint2D[] jointList = obj.GetComponents<FixedJoint2D>();
		//JointAngleLimits2D limits;

		FixedJoint2D joint = jointList[jointList.Length - 1];
		joint.frequency = 1;
		joint.dampingRatio = 0.5f;
		joint.autoConfigureConnectedAnchor = true;
		joint.enableCollision = false;
		//limits = joint.limits;
		//limits.max = 0;
		//joint.limits = limits;

		joint.connectedBody = connectedObj.GetComponent<Rigidbody2D>();
		joint.anchor = anchor;
		joint.connectedAnchor = connectedAnchor;
		joint.enableCollision = true;

		return joint;
	}
}
