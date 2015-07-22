using UnityEngine;
using System.Collections;
using RTS;

public class Common : MonoBehaviour {

	private static Terrain worldTerrain = GameObject.Find ("Ground").GetComponent<Terrain> ();
	private static Texture2D _staticRectTexture;
	private static GUIStyle _staticRectStyle;

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


	// Note that this function is only meant to be called from OnGUI() functions.
	public static void GUIDrawRect( Rect position, Color color )
	{
		if( _staticRectTexture == null )
		{
			_staticRectTexture = new Texture2D( 1, 1 );
			_staticRectTexture.SetPixel( 0, 0, color );
			_staticRectTexture.Apply();
		}
		
		if( _staticRectStyle == null )
		{
			_staticRectStyle = new GUIStyle();
			_staticRectStyle.normal.background = _staticRectTexture;
		}
		
		GUI.Box( position, GUIContent.none, _staticRectStyle );
		
		
	}

}
