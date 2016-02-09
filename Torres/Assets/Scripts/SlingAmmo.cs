using UnityEngine;
using System.Collections;

public class SlingAmmo : MonoBehaviour {

	public WeaponSling weapon;
	public bool isReleased = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseDown () {
		if (!isReleased) {
			weapon.HoldingAmmo ();
		}
	}

	void OnMouseUp () {
		if (!isReleased) {
			weapon.NotHoldingAmmo ();
		}
	}
}
