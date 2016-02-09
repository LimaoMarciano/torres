using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameManager : MonoBehaviour {

	public static GameManager instance;
	public VehicleData vehicleData;
	public PieceCreator pieceCreator;
	public ScrewCreator screwCreator;

	void Awake () {
		if (instance == null) {
			DontDestroyOnLoad (gameObject);
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Save() {
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + "/vehicleInfo.dat");
		bf.Serialize (file, vehicleData);
		file.Close ();
		Debug.Log ("Vehicle data saved");
	}

	public void Load() {
		if (File.Exists (Application.persistentDataPath + "/vehicleInfo.dat")) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/vehicleInfo.dat", FileMode.Open);
			vehicleData = (VehicleData)bf.Deserialize (file);
			file.Close ();
			Debug.Log ("Vehicle data loaded");

			List<Piece> piecesData = vehicleData.GetData ();
			foreach (Piece piece in piecesData) {
				if (piece.type == "piece") {
					Vector2 startPoint = new Vector2 (piece.aX, piece.aY);
					Vector2 endPoint = new Vector2 (piece.bX, piece.bY);
					pieceCreator.LoadPiece (startPoint, endPoint);
				}
				if (piece.type == "screw") {
					Vector2 position = new Vector2 (piece.aX, piece.aY);
					screwCreator.CreateScrew (position);
				}
			}
			Debug.Log ("Vehicle reconstructed");
		} else {
			Debug.Log ("Vehicle data not found");
		}
	}
}
