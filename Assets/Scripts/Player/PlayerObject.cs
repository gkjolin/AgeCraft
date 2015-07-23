using UnityEngine;
using System.Collections;

public class PlayerObject : WorldObject {
	
	// Attached player camera
	private Camera pCam;
	
	// Protected variables
	protected Player player;
	
	public Texture2D buildImage;
	public int cost, hitPoints, maxHitPoints;
	
	// For UserInput.cs
	public Vector2 screenPos;
	public bool onScreen;
	public bool selected = false;
	
	// For minimap
	public int mapPixelSize = 4;
	
	// GUI
	public Texture minimapUnitTexture;
	
	protected override void Awake() {
		base.Awake();
	}
	
	protected override void Start () {
		base.Start();
		pCam = transform.root.FindChild ("Camera").GetComponent<Camera> ();
		player = transform.root.GetComponentInChildren< Player >();
	}
	
	protected override void Update () {
		base.Update();
		
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
	
	protected override void OnGUI() {
		base.OnGUI();

		// Calculate the screen position of the unit
		Vector2 xyPos = Common.CalculateMinimapPosFromWorldCoordinate (transform.position);
		
		// Place a small gui box over the minimap
		Rect minimapPosition = new Rect (
			xyPos.x,
			xyPos.y,
			mapPixelSize,
			mapPixelSize
			);
		
		if (!minimapUnitTexture) {
			Common.GUIDrawRect (minimapPosition, Color.green);
		} else {
			GUI.DrawTexture(minimapPosition, minimapUnitTexture);
		}
	}
	
	public override void ObjectGotRightClicked(Player byPlayer) {
		base.ObjectGotRightClicked (byPlayer);
		if (player.Equals (byPlayer)) {
			// A player right-clicked his own unit
			Debug.Log("A player right-clicked his own unit");
		} else {
			// Another player right-clicked this unit
			Debug.Log("Another player right-clicked this unit");
		}
	}
	
	public virtual void DoRightClickAction(GameObject hitObject) {

	}
	
	public override void PerformAction(string actionToPerform) {
		base.PerformAction (actionToPerform);
	}
	

}
