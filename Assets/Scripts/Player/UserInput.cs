using UnityEngine;
using System.Collections;
using RTS;

public class UserInput : MonoBehaviour {

	private Player player;
	private PlayerCamera pCam;
	private Mouse mouse;

	// Use this for initialization
	void Start () {
		player = transform.root.GetComponent<Player>();
		mouse = transform.root.GetComponent<Mouse>();
		pCam = transform.root.FindChild ("Camera").GetComponent<PlayerCamera> ();
	}
	
	// Update is called once per frame
	void Update () {
		if(player.isHuman) {
			pCam.MoveCamera();
			mouse.MouseTracker();
		}
	}

}
