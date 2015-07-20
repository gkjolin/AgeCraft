using UnityEngine;
using System.Collections;
using RTS;

public class HUD : MonoBehaviour {

	public GUISkin hudSkin;
	private Player player;

	// Use this for initialization
	void Start () {
		player = transform.root.GetComponent<Player> ();
	}

	void OnGUI () {
		if (player && player.isHuman) {
			DrawHUD();
		}
	}

	private void DrawHUD() {
		GUI.skin = hudSkin;

		GUI.BeginGroup (new Rect (Screen.width / 4f, Screen.height - ResourceManager.HudHeight, Screen.width, ResourceManager.HudHeight));
		GUI.Box(new Rect (0, 0, Screen.width, ResourceManager.HudHeight),"");
		GUI.EndGroup();

	}
}
