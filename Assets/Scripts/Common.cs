using UnityEngine;
using System.Collections;
using RTS;

public class Common : MonoBehaviour {

	private static Terrain worldTerrain = GameObject.Find ("Ground").GetComponent<Terrain> ();

	// Float to bool
	public static bool FloatToBool(float val) {
		if (val < 0f)
			return false;
		else
			return true;
	}
	
	// Unsign a float
	public static float Unsigned (float val) {
		if (val < 0f) val *= -1;
		return val;
	}

	// Are the shift keys held down?
	public static bool ShiftKeysDown() {
		if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) {
			return true;
		} else {
			return false;
		}
	}

	public static Vector2 CalculateMinimapPosFromWorldCoordinate(Vector3 pos){

		float xOffset = (pos.x / worldTerrain.terrainData.size.x) * (Screen.width * ResourceManager.MinimapSizeX);
		float zOffset = (pos.z / worldTerrain.terrainData.size.z) * (Screen.height * ResourceManager.MinimapSizeZ);
		
		float minimapCenterX = (Screen.width * ResourceManager.MinimapOffsetX) + ((Screen.width * ResourceManager.MinimapSizeX) / 2);
		float minimapCenterZ = Screen.height - ((Screen.height * ResourceManager.MinimapOffsetZ) + ((Screen.height * ResourceManager.MinimapSizeZ) / 2));

		Vector2 result = new Vector2 (
			minimapCenterX + xOffset,
			minimapCenterZ - zOffset
		);

		return result;

	}

}
