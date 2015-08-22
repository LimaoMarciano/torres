using UnityEngine;
using System.Collections;

public class ConstructionManager : MonoBehaviour {

	public GameObject woodPiece;
	public GameObject connection;

	private Vector2 constructionPos;
	private GameObject constructionObj;
	private int pieceIndex = 0;

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

			if (InputManager.Instance.isMouseClicked) {
				constructionPos = InputManager.Instance.GetMousePositon();
				state = State.Contruction;
			}
			break;

		case State.Contruction:



			if (woodPiece != null) {

				//Piece creation
				constructionObj = Instantiate(woodPiece, new Vector2(0,0), Quaternion.identity) as GameObject;
				constructionObj.layer = LayerMask.NameToLayer("Pieces");
				constructionObj.tag = "Piece";
				constructionObj.name = "Piece" + pieceIndex;
				pieceIndex += 1;
				constructionObj.GetComponent<Rigidbody2D>().isKinematic = true;
				constructionObj.SetActive(false);
				Debug.Log ("Created new piece: " + constructionObj.name);

				Collider2D[] col = Physics2D.OverlapPointAll(constructionPos, LayerMask.GetMask("Connections"));

				if (col.Length > 0) {
					constructionPos = col[0].transform.position;
					Debug.Log ("Clicked in a connection. Snapping position to connector");
				}
				Debug.Log ("Changing to positioning mode");
				state = State.Positioning;
			}
			break;
		
		case State.Positioning:

			Collider2D[] col;
			Vector2 connectorAnchor;
			Vector2 connectorPos;

			Vector2 endPos = Input.mousePosition;
			endPos = Camera.main.ScreenToWorldPoint(endPos);
			Vector2 direction = endPos - constructionPos;
			Vector2 startPos = constructionPos + direction.normalized * 0.05f;
			constructionObj.SetActive(true);
			StrechSprite(constructionObj, startPos, endPos);

			if (InputManager.Instance.isMouseClicked) {

				col = Physics2D.OverlapPointAll(constructionPos, LayerMask.GetMask("Connections"));
				GameObject newConnector;

				if (col.Length > 0) {
					Debug.Log ("Start position is a connector. Creating joint");
					GameObject connector = col[0].gameObject;
					connectorPos = connector.transform.position;
					direction = startPos - connectorPos;
					connectorAnchor = connector.transform.InverseTransformPoint(connectorPos + direction);
					CreateDistanceJoint(connector, constructionObj, connectorAnchor, new Vector2(0.5f, 0), 0.005f);
				}
				else
				{
					Debug.Log ("Start position is not a connector. Creating new connector");
					Vector2 connectionPos = constructionObj.transform.TransformPoint(0.5f, 0, 0);
					newConnector = CreateConnector(connectionPos, new Vector2(0.5f,0), constructionObj);
					connectorPos = newConnector.transform.position;
					direction = startPos - connectorPos;
					connectorAnchor = newConnector.transform.InverseTransformPoint(connectorPos + direction);
					CreateDistanceJoint(newConnector, constructionObj, connectorAnchor, new Vector2(0.5f, 0), 0.005f);
				}

				col = Physics2D.OverlapPointAll(endPos, LayerMask.GetMask("Connections"));

				if (col.Length > 0) {
					Debug.Log ("End position is a connector. Creating joint");
					GameObject connector = col[0].gameObject;
					connectorPos = connector.transform.position;
					direction = connectorPos - startPos;
					endPos = connectorPos + direction.normalized * -0.05f;
					StrechSprite(constructionObj, startPos, endPos);

					connectorPos = connector.transform.position;
					direction = endPos - connectorPos;
					connectorAnchor = connector.transform.InverseTransformPoint(connectorPos + direction);

					CreateDistanceJoint(connector, constructionObj, connectorAnchor, new Vector2(-0.5f, 0), 0.005f);
				}
				else
				{
					Debug.Log ("End position is not a connector. Creating new connector");
					Vector2 connectionPos = constructionObj.transform.TransformPoint(-0.5f, 0, 0);
					newConnector = CreateConnector(connectionPos, new Vector2(-0.5f,0), constructionObj);
					connectorPos = newConnector.transform.position;
					direction = endPos - connectorPos;
					connectorAnchor = newConnector.transform.InverseTransformPoint(connectorPos + direction);
					CreateDistanceJoint(newConnector, constructionObj, connectorAnchor, new Vector2(-0.5f, 0), 0.005f);
				}

				Debug.Log ("Changing to initial state");
				state = State.WaitingInput;
			}
			break;
		}

	}

	/// <summary>
	/// Strechs the sprite.
	/// </summary>
	/// <param name="sprite">Sprite.</param>
	/// <param name="startPos">Start position.</param>
	/// <param name="endPos">End position.</param>
	void StrechSprite (GameObject sprite, Vector2 startPos, Vector2 endPos) {

		Vector2 centerPos = (startPos + endPos) / 2f;
		sprite.transform.position = centerPos;
		Vector2 direction = startPos - endPos;
		direction = direction.normalized;
		sprite.transform.right = direction;
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
	private GameObject CreateConnector (Vector2 pos, Vector2 anchor, GameObject obj) {
		GameObject connector = Instantiate(connection, pos, obj.transform.rotation) as GameObject;
		if (anchor.x >= 0)
			connector.transform.position += connector.transform.TransformDirection(Vector3.right) * 0.05f;
		else
			connector.transform.position += connector.transform.TransformDirection(Vector3.right) * -0.05f;
		connector.layer = LayerMask.NameToLayer("Connections");
		connector.tag = "Piece";
		connector.GetComponent<Connection>().connectedObj = constructionObj;
		connector.GetComponent<Connection>().anchor = anchor;

		Debug.Log("Created connector linked to " + connector.GetComponent<Connection>().connectedObj.name);
		return connector;
	}

	/// <summary>
	/// Creates the distance joint.
	/// </summary>
	/// <param name="obj">Object.</param>
	/// <param name="connectedObj">Connected object.</param>
	/// <param name="anchor">Anchor.</param>
	/// <param name="connectedAnchor">Connected anchor.</param>
	/// <param name="distance">Distance.</param>
	private void CreateDistanceJoint(GameObject obj, GameObject connectedObj, Vector2 anchor, Vector2 connectedAnchor, float distance) {

		obj.AddComponent<DistanceJoint2D>();
		DistanceJoint2D[] jointList = obj.GetComponents<DistanceJoint2D>();

		DistanceJoint2D joint = jointList[jointList.Length - 1];
		joint.connectedBody = connectedObj.GetComponent<Rigidbody2D>();
		joint.anchor = anchor;
		joint.connectedAnchor = connectedAnchor;
		joint.distance = distance;
		joint.enableCollision = true;
	}
}
