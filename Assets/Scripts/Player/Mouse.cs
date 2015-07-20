using UnityEngine;
using System.Collections;
using RTS;

public class Mouse : MonoBehaviour {

	// Private variables
	private Camera playerCam;

	// Ray cast mouse tracker
	RaycastHit hit;
	public GameObject target;
	private float rayLength = 500;

	public static Vector3 rightClickPoint;

	//	public static GameObject currentlySelectedUnit;
	public static ArrayList currentlySelectedUnits = new ArrayList (); // of GameObject
	public static ArrayList unitsOnScreen = new ArrayList (); // of GameObject
	public static ArrayList unitsInDrag = new ArrayList (); // of GameObject
	private bool finishedDragOnThisFrame;
	private bool startedDrag;

	// private layermask to enable mouse moving to any point on terrain
	private LayerMask allowTerrainMouseClick = (1 << 10);
//	private LayerMask ignoreLayersForMouseMove = ~((1 << 8) | (1 << 9) | (1 << 11));

	
	// Dragging variables
	public static bool userIsDragging;
	private static float timeLimitBeforeDeclareDrag = 1f;
	private static float timeLeftBeforeDeclareDrag;
	private static Vector2 mouseDragStart;

	
	private static Vector3 mouseDownPoint;
	private static Vector3 currentMousePoint; // world space
	
	// GUI
	public GUIStyle MouseDragSkin;

	private float boxWidth;
	private float boxHeight;
	private float boxTop;
	private float boxLeft;
	private static Vector2 boxStart;
	private static Vector2 boxFinish;

	// Initialize camera
	void Start() {
		playerCam = transform.root.FindChild ("Camera").GetComponent<Camera> ();
	}

	#region mouse
	public void MouseTracker () {
		
		// Run selection methods
		Ray ray = playerCam.ScreenPointToRay(Input.mousePosition);

		// Allow the user to click anywhere on the terrain to move objects
		if(Physics.Raycast(ray, out hit, rayLength, allowTerrainMouseClick)){
			
			// Temporary store the current mouse point
			currentMousePoint = hit.point;
			
			// Store point at mouse button down
			if(Input.GetMouseButtonDown(0)) {
				
				mouseDownPoint = hit.point;
				timeLeftBeforeDeclareDrag = timeLimitBeforeDeclareDrag;
				mouseDragStart = Input.mousePosition;
				startedDrag = true;

			} else if(Input.GetMouseButton(0)) {
				// if the user is not dragging, lets do the tests
				if(!userIsDragging) {
					timeLeftBeforeDeclareDrag -= Time.deltaTime;
					if(timeLeftBeforeDeclareDrag <= 0f || UserDraggingByPosition(mouseDragStart, Input.mousePosition)) {
						userIsDragging = true;
					}
				}
				
			} else if(Input.GetMouseButtonUp(0)){
				
				if(userIsDragging) {
					finishedDragOnThisFrame = true;
				}
				userIsDragging = false;
			}
			
			
			// Mouse click
			if(!userIsDragging) {
				
				if(hit.collider.name == "Ground") {
					// Hitting the terrain
					if(Input.GetMouseButtonDown(1)) {
						GameObject TargetObj = Instantiate(target, hit.point, Quaternion.identity) as GameObject;
						TargetObj.name = "target";

						// Store the rightclick point
						rightClickPoint = hit.point;

					} else if(Input.GetMouseButtonUp(0) && DidUserClickLeftMouse(mouseDownPoint)){
						if(!Common.ShiftKeysDown())
							DeselectGameobjectsIfSelected();
					}
				} else{
					
					// Hitting other objects
					if(Input.GetMouseButtonUp(0) && DidUserClickLeftMouse(mouseDownPoint)){

						// Is the user hitting a unit?
						if(hit.collider.transform.gameObject.GetComponent<Unit>()) {
							
							// Found a selectable object
							if(!UnitAlreadyInCurrentlySelectedUnits(hit.collider.transform.gameObject)) {
								
								// If the shift key is not down, start selecting anew
								if (!Common.ShiftKeysDown()) {
									DeselectGameobjectsIfSelected();
								}
								
								// Set the selected projection
								GameObject selectedObj = hit.collider.transform.FindChild("Selected").gameObject;
								selectedObj.SetActive(true);
								
								// Add the unit to the arraylist
								currentlySelectedUnits.Add (hit.collider.transform.gameObject);

								// Change the unit selected value to true
								hit.collider.transform.gameObject.GetComponent<Unit>().selected = true;
								
							} else {
								// Unit is already in the currently selected units arraylist, remove the unit when shift is held down
								if(Common.ShiftKeysDown()){
									RemoveUnitFromCurrentlySelectedUnits(hit.collider.transform.gameObject);
								} else {
									DeselectGameobjectsIfSelected();
									
									// Set the selected projection
									GameObject selectedObj = hit.collider.transform.FindChild("Selected").gameObject;
									selectedObj.SetActive(true);
									
									// Add the unit to the arraylist
									currentlySelectedUnits.Add (hit.collider.transform.gameObject);

									// Change the unit selected value to true
									hit.collider.transform.gameObject.GetComponent<Unit>().selected = true;
								}
								
							}
							
						}
						else {
							// If this object is not a unit
							if(!Common.ShiftKeysDown())
								DeselectGameobjectsIfSelected();
						}
						
					}
				}
			}
			
			
		}
		else if(Input.GetMouseButtonUp(0) && DidUserClickLeftMouse(mouseDownPoint)){
			if(!Common.ShiftKeysDown())
				DeselectGameobjectsIfSelected();
		}


		// Erase the selection if the user is dragging
		if (!Common.ShiftKeysDown() && startedDrag && userIsDragging) {
			DeselectGameobjectsIfSelected();
			startedDrag = false;
		}

		Debug.DrawRay(ray.origin, ray.direction * rayLength, Color.yellow);
		
		
		// GUI variables
		if (userIsDragging) {
			boxWidth = playerCam.WorldToScreenPoint(mouseDownPoint).x - playerCam.WorldToScreenPoint(currentMousePoint).x;
			boxHeight = playerCam.WorldToScreenPoint(mouseDownPoint).y - playerCam.WorldToScreenPoint(currentMousePoint).y;
			boxLeft = Input.mousePosition.x;
			boxTop = (Screen.height - Input.mousePosition.y) - boxHeight;
			
			if(Common.FloatToBool(boxWidth)) {
				if(Common.FloatToBool(boxHeight)) {
					boxStart = new Vector2(Input.mousePosition.x, Input.mousePosition.y + boxHeight);
				} else{
					boxStart = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
				}
			} else{
				if(Common.FloatToBool(boxHeight)) {
					boxStart = new Vector2(Input.mousePosition.x + boxWidth, Input.mousePosition.y + boxHeight);
				} else{
					boxStart = new Vector2(Input.mousePosition.x + boxWidth, Input.mousePosition.y);
				}
			}
			
			boxFinish = new Vector2 (
				boxStart.x + Common.Unsigned(boxWidth),
				boxStart.y - Common.Unsigned(boxHeight)
				);
			
		}
		
		
	}
	
	void LateUpdate() {
		unitsInDrag.Clear ();
		
		// If user is dragging or finished this frame, AND there are units to select on the screen.
		if ((userIsDragging || finishedDragOnThisFrame) && unitsOnScreen.Count > 0) {
			// loop through those units
			for(int i = 0; i < unitsOnScreen.Count; i++) {
				GameObject unitObj = unitsOnScreen[i] as GameObject;
				Unit unitScript = unitObj.GetComponent<Unit>();
				GameObject selectedObj = unitObj.transform.FindChild("Selected").gameObject;
				
				if(!UnitAlreadyInDraggedUnits(unitObj)) {
					if(UnitInsideDrag(unitScript.screenPos)) {
						selectedObj.SetActive(true);
						unitsInDrag.Add (unitObj);
					} else {
						// Unit is not in drag, remove the selecte graphic
						if(!UnitAlreadyInCurrentlySelectedUnits(unitObj)){
							selectedObj.SetActive(false);
						}
					}
				}
			}
		}
		
		if (finishedDragOnThisFrame) {
			finishedDragOnThisFrame = false;
			PutDraggedUnitsInCurrentlySelectedUnits();
		}
	}
	
	void OnGUI() {
		// box width, height, top, left
		if (userIsDragging) {
			GUI.Box (new Rect (boxLeft, boxTop, boxWidth, boxHeight), "", MouseDragSkin);
		}
	}
	
	#endregion
	
	
	#region helper functions

	
	// Is the user dragging relative to the mouse start point?
	public bool UserDraggingByPosition(Vector2 dragStartPoint, Vector2 newPoint) {
		if(
			(newPoint.x > dragStartPoint.x + ResourceManager.ClickTolerance || newPoint.x < dragStartPoint.x - ResourceManager.ClickTolerance) ||
			(newPoint.y > dragStartPoint.y + ResourceManager.ClickTolerance || newPoint.y < dragStartPoint.y - ResourceManager.ClickTolerance)
			)
			return true;
		else 
			return false;
	}
	
	// Check if a user clicked mouse (with a small tolerance)
	public bool DidUserClickLeftMouse (Vector3 hitPoint) {
		
		if (
			(mouseDownPoint.x < hitPoint.x + ResourceManager.ClickTolerance && mouseDownPoint.x > hitPoint.x - ResourceManager.ClickTolerance) &&
			(mouseDownPoint.y < hitPoint.y + ResourceManager.ClickTolerance && mouseDownPoint.y > hitPoint.y - ResourceManager.ClickTolerance) &&
			(mouseDownPoint.z < hitPoint.z + ResourceManager.ClickTolerance && mouseDownPoint.z > hitPoint.z - ResourceManager.ClickTolerance)
			)
			return true;
		else
			return false;
		
	}
	
	
	public static void DeselectGameobjectsIfSelected() {
		if (currentlySelectedUnits.Count > 0) {
			for (int i = 0; i < currentlySelectedUnits.Count; i++) {
				GameObject arrayListUnit = currentlySelectedUnits[i] as GameObject;
				arrayListUnit.transform.FindChild ("Selected").gameObject.SetActive(false);
				arrayListUnit.GetComponent<Unit>().selected = false;
			}
			currentlySelectedUnits.Clear ();
		}
	}
	
	// Check if a user is already in the currently selected units arraylist
	public static bool UnitAlreadyInCurrentlySelectedUnits(GameObject unit) {
		if (currentlySelectedUnits.Count > 0) {
			for (int i = 0; i < currentlySelectedUnits.Count; i++) {
				GameObject arrayListUnit = currentlySelectedUnits[i] as GameObject;
				if(arrayListUnit == unit)
					return true;
			}
		}
		return false;
	}
	
	// Remote a unit from the currently selected units arraylist
	public static void RemoveUnitFromCurrentlySelectedUnits(GameObject unit) {
		if (currentlySelectedUnits.Count > 0) {
			for (int i = 0; i < currentlySelectedUnits.Count; i++) {
				GameObject arrayListUnit = currentlySelectedUnits[i] as GameObject;
				if(arrayListUnit == unit) {
					currentlySelectedUnits.RemoveAt(i);
					arrayListUnit.transform.FindChild("Selected").gameObject.SetActive(false);
				}
			}
		}
	}
	
	// Check if a unit is in withing the screen space to deal with mouse drag selecting
	public static bool UnitsWithinScreenSpace(Vector2 unitScreenPos) {
		if(
			(unitScreenPos.x < Screen.width && unitScreenPos.y < Screen.height) &&
			(unitScreenPos.x > 0f && unitScreenPos.y > 0f)
			)
			return true;
		else 
			return false;
	}
	
	// Remove a unit from screen units unitsOnScreen ArrayList
	public static void RemoveFromOnScreenUnits (GameObject unit) {
		for (int i = 0; i < unitsOnScreen.Count; i++) {
			GameObject unitObj = unitsOnScreen[i] as GameObject;
			if (unit == unitObj) {
				unitsOnScreen.RemoveAt(i);
				unitObj.GetComponent<Unit>().onScreen = false;
				return;
			}
		}
		return;
	}
	
	// Is unit inside the drag?
	public static bool UnitInsideDrag(Vector2 unitScreenPos) {
		if(
			(unitScreenPos.x > boxStart.x && unitScreenPos.y < boxStart.y) &&
			(unitScreenPos.x < boxFinish.x && unitScreenPos.y > boxFinish.y)
			)
			return true;
		else 
			return false;
	}
	
	// Check if a unit is in unitsInDrag array list
	public static bool UnitAlreadyInDraggedUnits(GameObject unit) {
		if (unitsInDrag.Count > 0) {
			for (int i = 0; i < unitsInDrag.Count; i++) {
				GameObject arrayListUnit = unitsInDrag[i] as GameObject;
				if(arrayListUnit == unit)
					return true;
			}
		}
		return false;
	}
	
	// take all units from unitsInDrag, into currentlySelectedUnits
	public static void PutDraggedUnitsInCurrentlySelectedUnits() {
		if (unitsInDrag.Count > 0) {
			for (int i = 0; i < unitsInDrag.Count; i++) {
				GameObject unitObj = unitsInDrag[i] as GameObject;
				
				// if unit is not already in currentlySelectedUnits, add it!
				if(!UnitAlreadyInCurrentlySelectedUnits(unitObj)){
					currentlySelectedUnits.Add (unitObj);
					unitObj.GetComponent<Unit>().selected = true;
				} else if (Common.ShiftKeysDown() && UnitAlreadyInCurrentlySelectedUnits(unitObj)) {
					// Remove the already selected unit from the selection set
					RemoveUnitFromCurrentlySelectedUnits(unitObj);
				}
			}
			
			unitsInDrag.Clear ();
		}
	}
	
	#endregion
}
