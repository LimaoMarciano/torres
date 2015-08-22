using UnityEngine;
using System.Collections;

public class Strecth : MonoBehaviour {

	public Vector2 startPos;
	public Vector2 endPos;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		StrechSprite(gameObject, startPos, endPos);
	}

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
}
