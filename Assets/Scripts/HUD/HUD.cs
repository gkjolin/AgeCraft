using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RTS;

public class HUD : MonoBehaviour {

	public GUISkin hudSkin, resourceSkin;
	public Texture2D[] resources;

	private Player player;
	private Dictionary< ResourceType, int > resourceValues;
	private Dictionary< ResourceType, Texture2D > resourceImages;

	// Use this for initialization
	void Start () {
		player = transform.root.GetComponent<Player> ();

		resourceImages = new Dictionary< ResourceType, Texture2D >();
		for(int i = 0; i < resources.Length; i++) {
			Debug.Log(resources[i].name);
			switch(resources[i].name) {
			case "Materials": // From image icon name
				resourceImages.Add(ResourceType.Materials, resources[i]);
				break;
			case "Energy": // From image icon name
				resourceImages.Add(ResourceType.Energy, resources[i]);
				break;
			default: break;
			}
		}

	}

	void OnGUI () {
		if (player && player.isHuman) {
			DrawResourceBars();
			DrawHUD();
		}
	}
	
	private void DrawResourceBars() {
		GUI.skin = resourceSkin;
		
		GUI.BeginGroup (new Rect (Screen.width - 150, 20, 150, 80));
		GUI.Box(new Rect (0, 0, 130, 45),"");

		int topPos = 5, iconLeft = 4, textLeft = 40;
		DrawResourceIcon(ResourceType.Materials, iconLeft, textLeft, topPos);
		iconLeft += 32 + textLeft;
		textLeft += 32 + textLeft;
		DrawResourceIcon(ResourceType.Energy, iconLeft, textLeft, topPos);

		GUI.EndGroup();
		
	}

	private void DrawResourceIcon(ResourceType type, int iconLeft, int textLeft, int topPos) {
		Texture2D icon = resourceImages[type];
		string text = resourceValues[type].ToString();
		GUI.DrawTexture(new Rect(iconLeft, topPos, 32, 32), icon);
		GUI.Label (new Rect(textLeft, topPos, 32, 32), text);
	}

	private void DrawHUD() {
		GUI.skin = hudSkin;

		GUI.BeginGroup (new Rect (Screen.width / 4f, Screen.height - ResourceManager.HudHeight, Screen.width, ResourceManager.HudHeight));
		GUI.Box(new Rect (0, 0, Screen.width, ResourceManager.HudHeight),"");
		GUI.EndGroup();

	}

	public void SetResourceValues(Dictionary< ResourceType, int > resourceValues) {
		this.resourceValues = resourceValues;
	}
}
