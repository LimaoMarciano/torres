using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PrototypeUI : MonoBehaviour {

	public GameObject[] tools;
	public Button[] toolButtons;
	private int activeTool = 0;

	// Use this for initialization
	void Start () {
		ActivateTool (0);
	}
	
	// Update is called once per frame
	void Update () {

		for (int i = 0; i < toolButtons.Length; i++) {
			if (i == activeTool) {
				toolButtons [i].image.color = new Color (1, 1, 1, 1);
			} else {
				toolButtons [i].image.color = new Color (1, 1, 1, 0.3f);
			}
		}
			
	}

	public void StartGame () {
		Debug.Log("Activating physics");

		foreach (GameObject tool in tools) {
			tool.SetActive (false);
		}

		GameObject[] pieceList;
		pieceList = GameObject.FindGameObjectsWithTag("Piece");
		foreach (GameObject piece in pieceList) {
			piece.GetComponent<Rigidbody2D>().isKinematic = false;
		}
	}

	public void ActivateTool (int toolNumber) {
		activeTool = toolNumber;
		foreach (GameObject tool in tools) {
			tool.SetActive (false);
		}
		tools [toolNumber].SetActive (true);
	}

	public void Restart () {
		SceneManager.LoadScene ("teste");
	}

	public void Quit () {
		Application.Quit ();
	}
}
