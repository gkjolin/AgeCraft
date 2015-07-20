using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	// Player initialization
	private MinimapController minimapControl;

	// Player variables
	public int materials;
	public int energy;
	public string username;
	public bool isHuman;


	void Start() {
		minimapControl = transform.root.GetComponent<MinimapController>();
	}
	
	// Update is called once per frame
	void Update () {
		if (this.isHuman) {
			minimapControl.ShowMinimap();
		}
	}
}
