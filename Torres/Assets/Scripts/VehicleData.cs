using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[Serializable]
public class VehicleData {

	private List<Piece> pieces = new List<Piece>();

	public void CreatePiece (Vector2 startPos, Vector2 endPos, string type) {
		Piece piece = new Piece ();
		piece.aX = startPos.x;
		piece.aY = startPos.y;
		piece.bX = endPos.x;
		piece.bY = endPos.y;
		piece.type = type;
		pieces.Add (piece);
	}

	public void ClearData () {
		pieces.Clear ();
	}

	public List<Piece> GetData () {
		return pieces;
	}
}

[Serializable]
public class Piece {
	public string type;
	public float aX;
	public float aY;
	public float bX;
	public float bY;
	//	public string material;
}
