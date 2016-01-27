using UnityEngine;
using System.Collections;

public class Piece : MonoBehaviour {

	public Transform start;
	public Transform end;

	// Use this for initialization
	void Start () {
		Vector2 startPos = new Vector2 (start.position.x, start.position.y);
		Vector2 endPos = new Vector2 (end.position.x, end.position.y);
		StrecthSprite (startPos, endPos);
	}
	
	// Update is called once per frame
	void Update () {
		if (start && end) {
			Vector2 startPos = new Vector2 (start.position.x, start.position.y);
			Vector2 endPos = new Vector2 (end.position.x, end.position.y);
			StrecthSprite (startPos, endPos);
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
}
