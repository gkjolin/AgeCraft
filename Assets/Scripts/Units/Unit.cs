﻿using UnityEngine;
using System.Collections;
using RTS;

/*
 * This Script should be attached to all controllable units in the game, wether they are walkable or not
 * */

public class Unit : MonoBehaviour {

	// Attached player camera
	private Camera pCam;

	// For UserInput.cs
	public Vector2 screenPos;
	public bool onScreen;
	public bool selected = false;

	public bool isWalkable = true;

	// For minimap
	public int mapPixelSize = 4;

	// GUI
	public Texture unitTexture;

	void Awake() {
		// Physics.IgnoreLayerCollision (9, 9, true);
	}

	// Use this for initialization
	void Start () {
		pCam = transform.root.FindChild ("Camera").GetComponent<Camera> ();
	}
	
	// Update is called once per frame
	void Update () {
		// if unit not selected, get screen space
		if(!selected) {
			// track the screen position
			screenPos = pCam.WorldToScreenPoint(this.transform.position);

			// if withing the screen space
			if(Mouse.UnitsWithinScreenSpace(screenPos)) {
				// and not already added to unitsOnScreen, add it!
				if(!onScreen) {
					Mouse.unitsOnScreen.Add (this.gameObject);
					onScreen = true;
				}
			} else {
				// Unit is not in screen space

				// remove if previously on the screen
				if(onScreen) {
					Mouse.RemoveFromOnScreenUnits(this.gameObject);
				}

			}
		}
	}
	
	
	void OnGUI (){
		if (!unitTexture) {
			Debug.LogError("Assign a Texture in the inspector.");
			return;
		}

		// Calculate the screen position of the unit
		Vector2 xyPos = Common.CalculateMinimapPosFromWorldCoordinate (transform.position);

		// Place a small gui box over the minimap
		Rect minimapPosition = new Rect (
            xyPos.x,
            xyPos.y,
			mapPixelSize,
			mapPixelSize
			);

		GUI.DrawTexture(minimapPosition, unitTexture);
	}

}
