using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {

	public static InputManager Instance {get; private set;}

	private Vector2 clickMousePos;
	public bool isMouseClicked = false;

	void Awake() {
		Instance = this;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		isMouseClicked = false;

		if(Input.GetMouseButtonDown(0)) {
			clickMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			isMouseClicked = true;
			Debug.Log ("Cliked at " + clickMousePos);
		}

		if (Input.GetKeyDown(KeyCode.Space)) {

			Debug.Log("Activating physics");
			GameObject[] piecesList;
			piecesList = GameObject.FindGameObjectsWithTag("Piece");

			foreach (GameObject go in piecesList) {
				go.GetComponent<Rigidbody2D>().isKinematic = false;
			}


//			GameObject[] connectorsList;
//			connectorsList = GameObject.FindGameObjectsWithTag("Connector");
//			foreach (GameObject go in connectorsList) {
//				go.SetActive(false);
//			}
		}

	}

	public Vector2 GetMousePositon () {
		return clickMousePos;
	}



}