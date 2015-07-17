using UnityEngine;
using System.Collections;
using RTS;

public class UserInput : MonoBehaviour {

	private Player player;
	private PlayerCamera playerCam;

	// Use this for initialization
	void Start () {
		player = transform.root.GetComponent<Player>();
		playerCam = transform.root.FindChild ("Camera").GetComponent<PlayerCamera> ();
	}
	
	// Update is called once per frame
	void Update () {
		if(player.isHuman) {
			playerCam.MoveCamera();
			playerCam.MouseTracker();
		}
	}

}
