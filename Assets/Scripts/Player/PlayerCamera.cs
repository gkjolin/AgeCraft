using UnityEngine;
using System.Collections;
using RTS;

public class PlayerCamera : MonoBehaviour {

	private Camera playerCam;

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
	

}
