using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RTS;

public class Common : MonoBehaviour {

	private static Terrain worldTerrain = GameObject.Find ("Ground").GetComponent<Terrain> ();
	private static Dictionary<Color, Texture2D> staticRectTexture2D = new Dictionary<Color, Texture2D> ();
	private static Dictionary<Color, GUIStyle> staticRectGUIStyle = new Dictionary<Color, GUIStyle> ();

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

		if( ! staticRectTexture2D.ContainsKey(color) )
		{
			// Create a temporary texture
			Texture2D tmpTexture = new Texture2D(1, 1);
			tmpTexture.SetPixel( 0, 0, color );
			tmpTexture.Apply();

			// Add the new texture
			staticRectTexture2D.Add(color, tmpTexture);
		}
		
		if( ! staticRectGUIStyle.ContainsKey(color) )
		{
			GUIStyle tmpStyle = new GUIStyle();
			tmpStyle.normal.background = staticRectTexture2D[color];

			// Add the new GUIStyle
			staticRectGUIStyle.Add(color, tmpStyle);
		}
		
		GUI.Box( position, GUIContent.none, staticRectGUIStyle[color] );
		
	}

}
