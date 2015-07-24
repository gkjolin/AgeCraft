using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using RTS;

public class PlayerCamera : NetworkBehaviour {
	
	private Camera pCam;

	private Terrain worldTerrain;
	private float worldTerrainPadding = 25f;

	// box limit struct
	public struct BoxLimit
	{
		public float leftLimit;
		public float rightLimit;
		public float topLimit;
		public float bottomLimit;
	}

	// Define camera and mouse limits
	public static BoxLimit cameraLimits = new BoxLimit();
	public static BoxLimit mouseScrollLimits = new BoxLimit();

	void Start() {

		// Player camera
		worldTerrain = (Terrain)FindObjectOfType (typeof(Terrain));
		pCam = GetComponent<Camera> ();

		// Declare camera limits
		cameraLimits.leftLimit = worldTerrain.transform.position.x + worldTerrainPadding;
		cameraLimits.rightLimit = worldTerrain.terrainData.size.x + worldTerrain.transform.position.x - worldTerrainPadding;
		cameraLimits.topLimit = worldTerrain.terrainData.size.z + worldTerrain.transform.position.z - worldTerrainPadding;
		cameraLimits.bottomLimit = worldTerrain.transform.position.z + worldTerrainPadding;
	
		// Declare mouse limits
		mouseScrollLimits.leftLimit = ResourceManager.CameraMoveTriggerPadding;
		mouseScrollLimits.rightLimit = ResourceManager.CameraMoveTriggerPadding;
		mouseScrollLimits.topLimit = ResourceManager.CameraMoveTriggerPadding;
		mouseScrollLimits.bottomLimit = ResourceManager.CameraMoveTriggerPadding;

	}

	public void MoveCamera() {

		if (CheckIfUserCameraInput ()) {
			Vector3 cameraDesiredMove = GetDesiredTranslation ();

			Debug.Log (this);
			if (!IsDesiredPositionOverBoundaries (cameraDesiredMove)) {
				this.transform.Translate (cameraDesiredMove, Space.World);

				Vector3 limitedHeightPosition = this.transform.position;

				// Limit height
				if(this.transform.position.y > ResourceManager.MaxCameraHeight) {
					limitedHeightPosition.y = ResourceManager.MaxCameraHeight;
				} else if(this.transform.position.y < ResourceManager.MinCameraHeight) {
					limitedHeightPosition.y = ResourceManager.MinCameraHeight;
				}
				this.transform.position = limitedHeightPosition;
//				
//				float smooth = 2f;
//				float tiltAngle = 15f;
//				float tiltAroundX = (Input.GetAxis("Mouse ScrollWheel") * tiltAngle) * limitedHeightPosition.y;
//
//				Quaternion target = Quaternion.Euler(tiltAroundX, 0, 0);
//				this.transform.rotation = Quaternion.Slerp(this.transform.rotation, target, Time.deltaTime * smooth);

			} else{

				// Get the current position
				Vector3 currentPosition = this.transform.position;

				// Move the camera to the boundary
				if (
					(Input.GetKey (KeyCode.UpArrow) || Input.mousePosition.y > (Screen.height - mouseScrollLimits.topLimit)) &&
					currentPosition.z > cameraLimits.topLimit
					)
				{
					currentPosition.z = cameraLimits.topLimit;
				}
				if (
					(Input.GetKey (KeyCode.DownArrow) || Input.mousePosition.y < mouseScrollLimits.bottomLimit) &&
					currentPosition.z < cameraLimits.bottomLimit
					)
				{
					currentPosition.z = cameraLimits.bottomLimit;
				}
				if (
					(Input.GetKey (KeyCode.LeftArrow) || Input.mousePosition.x < mouseScrollLimits.leftLimit) &&
					currentPosition.x < cameraLimits.leftLimit
					)
				{
					currentPosition.x = cameraLimits.leftLimit;
				}
				if (
					(Input.GetKey (KeyCode.RightArrow) || Input.mousePosition.x > (Screen.width - mouseScrollLimits.rightLimit)) &&
					currentPosition.x > cameraLimits.rightLimit
					){
					currentPosition.x = cameraLimits.rightLimit;
				}
				
				this.transform.position = currentPosition;

			}
		}
		
	}

	public void MoveCameraToLocation(Vector3 targetPoint) {

		// Keep the height
		targetPoint.y = pCam.transform.position.y;
		targetPoint.z -= 10f;

		// Move the camera
		pCam.transform.position = targetPoint;

	}

	public Vector3 GetDesiredTranslation() {

		float moveSpeed = 0f;
		Vector3 desiredTranslation = new Vector3 ();

		if (Common.ShiftKeysDown ())
			moveSpeed = (ResourceManager.CameraMoveSpeed + ResourceManager.CameraShiftBonusSpeed) * Time.deltaTime;
		else
			moveSpeed = ResourceManager.CameraMoveSpeed * Time.deltaTime;

		// Move via keyboard or mouse
		if (Input.GetKey (KeyCode.UpArrow) || Input.mousePosition.y > (Screen.height - mouseScrollLimits.topLimit)) {
			desiredTranslation += Vector3.forward * moveSpeed;
		}
		if (Input.GetKey (KeyCode.DownArrow) || Input.mousePosition.y < mouseScrollLimits.bottomLimit) {
			desiredTranslation += Vector3.back * moveSpeed;
		}
		if (Input.GetKey (KeyCode.LeftArrow) || Input.mousePosition.x < mouseScrollLimits.leftLimit) {
			desiredTranslation += Vector3.left * moveSpeed;
		}
		if (Input.GetKey (KeyCode.RightArrow) || Input.mousePosition.x > (Screen.width - mouseScrollLimits.rightLimit)) {
			desiredTranslation += Vector3.right * moveSpeed;
		}

		// Zoom scroll
		if (IsMouseScrollerScrolled ()){
			desiredTranslation.y -= (Input.GetAxis ("Mouse ScrollWheel") * ResourceManager.CameraScrollSpeed);
		}

		return desiredTranslation;
//
//		float xpos = Input.mousePosition.x;
//		float ypos = Input.mousePosition.y;
//		Vector3 movement = new Vector3 (0, 0, 0);
//		
//		//horizontal camera movement
//		if(xpos >= 0 && xpos < ResourceManager.ScrollWidth) {
//			movement.x -= ResourceManager.ScrollSpeed;
//		} else if(xpos <= Screen.width && xpos > Screen.width - ResourceManager.ScrollWidth) {
//			movement.x += ResourceManager.ScrollSpeed;
//		}
//		
//		//vertical camera movement
//		if(ypos >= 0 && ypos < ResourceManager.ScrollWidth) {
//			movement.z -= ResourceManager.ScrollSpeed;
//		} else if(ypos <= Screen.height && ypos > Screen.height - ResourceManager.ScrollWidth) {
//			movement.z += ResourceManager.ScrollSpeed;
//		}
//		
//		//make sure movement is in the direction the camera is pointing
//		//but ignore the vertical tilt of the camera to get sensible scrolling
//		movement = playerCam.transform.TransformDirection(movement);
//		movement.y = 0;
//		
//		//away from ground movement
//		movement.y -= ResourceManager.ScrollSpeed * Input.GetAxis("Mouse ScrollWheel");
//		
//		//calculate desired camera position based on received input
//		Vector3 origin = playerCam.transform.position;
//		Vector3 destination = origin;
//		destination.x += movement.x;
//		destination.y += movement.y;
//		destination.z += movement.z;
//		
//		//limit away from ground movement to be between a minimum and maximum distance
//		if(destination.y > ResourceManager.MaxCameraHeight) {
//			destination.y = ResourceManager.MaxCameraHeight;
//		} else if(destination.y < ResourceManager.MinCameraHeight) {
//			destination.y = ResourceManager.MinCameraHeight;
//		}
//		
//		//if a change in position is detected perform the necessary update
//		if(destination != origin) {
//			playerCam.transform.position = Vector3.MoveTowards(origin, destination, Time.deltaTime * ResourceManager.ScrollSpeed);
//		}

	}

	public bool CheckIfUserCameraInput() {
		bool keyboardMove = false;
		bool mouseMove = false;
		bool zoomMove = false;
		bool canMove = false;

		if (PlayerCamera.AreCameraKeyboardButtonsPressed ()) {
			keyboardMove = true;
		}

		if (PlayerCamera.IsMousePositionWithinBounaries ()) {
			mouseMove = true;
		}
		
		if (PlayerCamera.IsMouseScrollerScrolled ()) {
			zoomMove = true;
		}

		if (keyboardMove || mouseMove || zoomMove) {
			canMove = true;
		}

		return canMove;
	}

	public static bool AreCameraKeyboardButtonsPressed() {
		if (Input.GetKey (KeyCode.UpArrow) || Input.GetKey (KeyCode.LeftArrow) || Input.GetKey (KeyCode.DownArrow) || Input.GetKey (KeyCode.RightArrow))
			return true;
		else
			return false;
	}
	
	public static bool IsMousePositionWithinBounaries() {
		if (
			(Input.mousePosition.x < mouseScrollLimits.leftLimit && Input.mousePosition.x > -5) ||
			(Input.mousePosition.x > (Screen.width - mouseScrollLimits.rightLimit) && Input.mousePosition.x < (Screen.width + 5)) ||
			(Input.mousePosition.y < mouseScrollLimits.bottomLimit && Input.mousePosition.y > -5) ||
			(Input.mousePosition.y > (Screen.height - mouseScrollLimits.topLimit) && Input.mousePosition.y < (Screen.height + 5))
			)
			return true;
		else
			return false;
	}
	
	public static bool IsMouseScrollerScrolled() {
		if (Input.GetAxis("Mouse ScrollWheel") > 0f)
			return true;
		else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
			return true;
		else
			return false;
	}

	public bool IsDesiredPositionOverBoundaries(Vector3 desiredTranslation) {

		Vector3 desiredWorldPosition = this.transform.TransformPoint (desiredTranslation);

		bool overBoundaries = false;

		// Check boundaries
		if (desiredWorldPosition.x < cameraLimits.leftLimit)
			overBoundaries = true;
		if (desiredWorldPosition.x > cameraLimits.rightLimit)
			overBoundaries = true;
		if (desiredWorldPosition.z > cameraLimits.topLimit)
			overBoundaries = true;
		if (desiredWorldPosition.z < cameraLimits.bottomLimit)
			overBoundaries = true;

		return overBoundaries;

	}

}
