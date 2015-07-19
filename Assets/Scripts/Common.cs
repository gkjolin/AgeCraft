using UnityEngine;
using System.Collections;

public class Common : MonoBehaviour {
	
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

}
