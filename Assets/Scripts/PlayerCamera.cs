using UnityEngine;
using System.Collections;
using RTS;

public class PlayerCamera : MonoBehaviour {

	private Camera playerCam;
	RaycastHit hit;
	public GameObject target;
	private float rayLength = 500;

	public static GameObject currentlySelectedUnit;
	public static ArrayList currentlySelectedUnits = new ArrayList ();

	private static Vector3 mouseDownPoint;

	// Dragging variables
	public static bool userIsDragging;
	private static float timeLimitBeforeDeclareDrag = 1f;
	private static float timeLeftBeforeDeclareDrag;
	private static Vector2 mouseDragStart;

	void Awake() {
		mouseDownPoint = Vector3.zero;
	}

	void Start() {
		playerCam = transform.GetComponent<Camera> ();
	}

	#region camera
	public void MoveCamera() {
		float xpos = Input.mousePosition.x;
		float ypos = Input.mousePosition.y;
		Vector3 movement = new Vector3 (0, 0, 0);
		
		//horizontal camera movement
		if(xpos >= 0 && xpos < ResourceManager.ScrollWidth) {
			movement.x -= ResourceManager.ScrollSpeed;
		} else if(xpos <= Screen.width && xpos > Screen.width - ResourceManager.ScrollWidth) {
			movement.x += ResourceManager.ScrollSpeed;
		}
		
		//vertical camera movement
		if(ypos >= 0 && ypos < ResourceManager.ScrollWidth) {
			movement.z -= ResourceManager.ScrollSpeed;
		} else if(ypos <= Screen.height && ypos > Screen.height - ResourceManager.ScrollWidth) {
			movement.z += ResourceManager.ScrollSpeed;
		}
		
		//make sure movement is in the direction the camera is pointing
		//but ignore the vertical tilt of the camera to get sensible scrolling
		movement = playerCam.transform.TransformDirection(movement);
		movement.y = 0;
		
		//away from ground movement
		movement.y -= ResourceManager.ScrollSpeed * Input.GetAxis("Mouse ScrollWheel");
		
		//calculate desired camera position based on received input
		Vector3 origin = playerCam.transform.position;
		Vector3 destination = origin;
		destination.x += movement.x;
		destination.y += movement.y;
		destination.z += movement.z;
		
		//limit away from ground movement to be between a minimum and maximum distance
		if(destination.y > ResourceManager.MaxCameraHeight) {
			destination.y = ResourceManager.MaxCameraHeight;
		} else if(destination.y < ResourceManager.MinCameraHeight) {
			destination.y = ResourceManager.MinCameraHeight;
		}
		
		//if a change in position is detected perform the necessary update
		if(destination != origin) {
			playerCam.transform.position = Vector3.MoveTowards(origin, destination, Time.deltaTime * ResourceManager.ScrollSpeed);
		}
		
	}
	#endregion
	
	
	#region mouse
	public void MouseTracker () {
		
		// Run selection methods
		Ray ray = playerCam.ScreenPointToRay(Input.mousePosition);
		
		if(Physics.Raycast(ray, out hit, rayLength)){

			// Store point at mouse button down
			if(Input.GetMouseButtonDown(0)) {
				mouseDownPoint = hit.point;
			}

			// Mouse drag
			if(Input.GetMouseButtonDown(0)) {
				timeLeftBeforeDeclareDrag = timeLimitBeforeDeclareDrag;
				mouseDragStart = Input.mousePosition;

			} else if(Input.GetMouseButton(0)) {
				// if the user is not dragging, lets do the tests
				if(!userIsDragging) {
					timeLeftBeforeDeclareDrag -= Time.deltaTime;
					if(timeLeftBeforeDeclareDrag <= 0f || UserDraggingByPosition(mouseDragStart, Input.mousePosition)) {
						userIsDragging = true;
					}
				}

				if(userIsDragging){
					// OK, user is dragging, lets compute (GUI...)
					Debug.Log ("USER IS DRAGGIGN");
				}

			} else if(Input.GetMouseButtonUp(0)){
				Debug.Log ("NO DRAGGIGN");
				timeLeftBeforeDeclareDrag = 0f;
				userIsDragging = false;
			}


			// Mouse click
			if(!userIsDragging) {

				if(hit.collider.name == "Ground") {
					// Hitting the terrain
					if(Input.GetMouseButtonDown(1)) {
						GameObject TargetObj = Instantiate(target, hit.point, Quaternion.identity) as GameObject;
						TargetObj.name = "target";
					} else if(Input.GetMouseButtonUp(0) && DidUserClickLeftMouse(mouseDownPoint)){
						if(!ShiftKeysDown())
							DeselectGameobjectsIfSelected();
					}
				} else{
					
					// Hitting other objects
					if(Input.GetMouseButtonUp(0) && DidUserClickLeftMouse(mouseDownPoint)){
						
						// Is the user hitting a unit?
						if(hit.collider.transform.parent.FindChild("Selected")) {
							
							
							// Found a selectable object
							if(!UnisAlreadyInCurrentlySelectedUnits(hit.collider.transform.parent.gameObject)) {
								
								// If the shift key is not down, start selecting anew
								if (!ShiftKeysDown()) {
									DeselectGameobjectsIfSelected();
								}
								
								// Set the selected projection
								GameObject selectedObj = hit.collider.transform.parent.FindChild("Selected").gameObject;
								selectedObj.GetComponent<Projector>().enabled = true;
								
								// Add the unit to the arraylist
								currentlySelectedUnits.Add (hit.collider.transform.parent.gameObject);
								
							} else {
								// Unit is already in the currently selected units arraylist, remove the unit when shift is held down
								if(ShiftKeysDown()){
									RemoveUnitFromCurrentlySelectedUnits(hit.collider.transform.parent.gameObject);
								} else {
									DeselectGameobjectsIfSelected();
									
									// Set the selected projection
									GameObject selectedObj = hit.collider.transform.parent.FindChild("Selected").gameObject;
									selectedObj.GetComponent<Projector>().enabled = true;
									
									// Add the unit to the arraylist
									currentlySelectedUnits.Add (hit.collider.transform.parent.gameObject);
								}
								
							}
							
						}
						else {
							// If this object is not a unit
							if(!ShiftKeysDown())
								DeselectGameobjectsIfSelected();
						}
						
					}
				}
			}

			
		}
		else if(Input.GetMouseButtonUp(0) && DidUserClickLeftMouse(mouseDownPoint)){
			if(!ShiftKeysDown())
				DeselectGameobjectsIfSelected();
		}
		
		Debug.DrawRay(ray.origin, ray.direction * rayLength, Color.yellow);
		
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
				arrayListUnit.transform.FindChild ("Selected").GetComponent<Projector>().enabled = false;
			}
			currentlySelectedUnits.Clear ();
		}
	}

	// Check if a user is already in the currently selected units arraylist
	public static bool UnisAlreadyInCurrentlySelectedUnits(GameObject unit) {
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
	public void RemoveUnitFromCurrentlySelectedUnits(GameObject unit) {
		if (currentlySelectedUnits.Count > 0) {
			for (int i = 0; i < currentlySelectedUnits.Count; i++) {
				GameObject arrayListUnit = currentlySelectedUnits[i] as GameObject;
				if(arrayListUnit == unit) {
					currentlySelectedUnits.RemoveAt(i);
					arrayListUnit.transform.FindChild("Selected").GetComponent<Projector>().enabled = false;
				}
			}
		}
	}

	// Are the shift keys held down?
	public static bool ShiftKeysDown() {
		if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) {
			return true;
		} else {
			return false;
		}
	}

	#endregion
}
