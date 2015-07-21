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
			switch(resources[i].name) {
			case "Materials": // From image icon name
				resourceImages.Add(ResourceType.Materials, resources[i]);
				resourceValues.Add(ResourceType.Materials, 0);
				break;
			case "Energy": // From image icon name
				resourceImages.Add(ResourceType.Energy, resources[i]);
				resourceValues.Add(ResourceType.Energy, 0);
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
		
		GUI.BeginGroup (new Rect (Screen.width - 300, 100, 200, 100));
		GUI.Box(new Rect (0, 0, 200, 100),"");

		int topPos = 4, iconLeft = 4, textLeft = 20;
		DrawResourceIcon(ResourceType.Materials, iconLeft, textLeft, topPos);
		iconLeft += 15;
		textLeft += 15;
		DrawResourceIcon(ResourceType.Energy, iconLeft, textLeft, topPos);

		GUI.EndGroup();
		
	}

	private void DrawResourceIcon(ResourceType type, int iconLeft, int textLeft, int topPos) {
		Texture2D icon = resourceImages[type];
		string text = resourceValues[type].ToString();
		GUI.DrawTexture(new Rect(iconLeft, topPos, 15, 15), icon);
		GUI.Label (new Rect(textLeft, topPos, 15, 15), text);
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
