using UnityEngine;
using System.Collections;

public class WeaponCreator : MonoBehaviour {

	public GameObject weaponPrefab;
	public GameObject cursorPrefab;

	private Vector2 mousePosition;
	private bool isConstructionEnabled;
	private GameObject cursor;


	// Use this for initialization
	void Start () {
		cursor = Instantiate(cursorPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		cursor.transform.SetParent(gameObject.transform);
		cursor.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		Collider2D[] colliders;

		mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		colliders = Physics2D.OverlapPointAll(mousePosition, LayerMask.GetMask("Pieces"));

		if (colliders.Length > 0) {
			isConstructionEnabled = true;
		} else {
			isConstructionEnabled = false;
		}

		if (isConstructionEnabled) {
			cursor.SetActive(true);
			cursor.transform.position = mousePosition;

			if (Input.GetMouseButtonDown(0)) {
				CreateWeapon(mousePosition, colliders[0].gameObject);
			}

		} else {
			cursor.SetActive(false);
		}
	}

	private void CreateWeapon (Vector2 position, GameObject parent) {
		GameObject weapon = Instantiate(weaponPrefab, position, Quaternion.identity) as GameObject;
		weapon.transform.SetParent(parent.transform);
	}
}
