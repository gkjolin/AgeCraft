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

	private WorldObject lastSelection;
	private float sliderValue;

	// HUD static variables
	private const int BUILD_IMAGE_HEIGHT = 64;
	private const int BUILD_IMAGE_WIDTH = 64;
	private const int BUTTON_SPACING = 6;
	private const int SCROLL_BAR_WIDTH = 22;
	private const int BUILD_IMAGE_PADDING = 8;

	public Texture2D buildFrame, buildMask;

	// Use this for initialization
	void Start () {
		player = transform.root.GetComponent<Player> ();

		resourceImages = new Dictionary< ResourceType, Texture2D >();
		for(int i = 0; i < resources.Length; i++) {
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
		if (Mouse.currentlySelectedUnits.Count > 0) {
			GameObject curSel = Mouse.currentlySelectedUnits[0] as GameObject;
			Building selectedBuilding = curSel.GetComponent< Building >();
			if(selectedBuilding) {
				DrawBuildQueue(selectedBuilding.getBuildQueueValues(), selectedBuilding.getBuildPercentage());
			}
			DrawActions (curSel.GetComponent<PlayerObject>().GetActions ());
		} else {
			//reset slider value if the selected object has changed
			sliderValue = 0.0f;
		}
		GUI.EndGroup();

	}

	public void SetResourceValues(Dictionary< ResourceType, int > resourceValues) {
		this.resourceValues = resourceValues;
	}

	private void DrawActions(string[] actions) {
//		GUIStyle buttons = new GUIStyle();
//		buttons.hover.background = buttonHover;
//		buttons.active.background = buttonClick;

		int buildAreaHeight = 200;
//		GUI.skin.button = buttons;
		int numActions = actions.Length;
		//define the area to draw the actions inside
		GUI.BeginGroup(new Rect(300, 0, ResourceManager.HudHeight, buildAreaHeight));
		//draw scroll bar for the list of actions if need be
		if(numActions >= MaxNumRows(buildAreaHeight)) DrawSlider(buildAreaHeight, numActions / 2.0f);
		//display possible actions as buttons and handle the button click for each
		for(int i = 0; i < numActions; i++) {
			int column = i % 2;
			int row = i / 2;
			Rect pos = GetButtonPos(row, column);
			Texture2D action = ResourceManager.GetBuildImage(actions[i]);
			if(action) {
				//create the button and handle the click of that button
				if(GUI.Button(pos, action)) {
					if(Mouse.currentlySelectedUnits.Count > 0) {
						GameObject curSel = Mouse.currentlySelectedUnits[0] as GameObject;
						curSel.GetComponent<PlayerObject>().PerformAction(actions[i]);
					}
				}
			}
		}
		GUI.EndGroup();
	}

	private void DrawBuildQueue(string[] buildQueue, float buildPercentage) {
		for(int i = 0; i < buildQueue.Length; i++) {
			float topPos = i * BUILD_IMAGE_HEIGHT - (i+1) * BUILD_IMAGE_PADDING;
			Rect buildPos = new Rect(BUILD_IMAGE_PADDING, topPos, BUILD_IMAGE_WIDTH, BUILD_IMAGE_HEIGHT);
			GUI.DrawTexture(buildPos, ResourceManager.GetBuildImage(buildQueue[i]));
			GUI.DrawTexture(buildPos, buildFrame);
			topPos += BUILD_IMAGE_PADDING;
			float width = BUILD_IMAGE_WIDTH - 2 * BUILD_IMAGE_PADDING;
			float height = BUILD_IMAGE_HEIGHT - 2 * BUILD_IMAGE_PADDING;
			if(i==0) {
				//shrink the build mask on the item currently being built to give an idea of progress
				topPos += height * buildPercentage;
				height *= (1 - buildPercentage);
			}
			GUI.DrawTexture(new Rect(2 * BUILD_IMAGE_PADDING, topPos, width, height), buildMask);
		}
	}

	private int MaxNumRows(int areaHeight) {
		return areaHeight / BUILD_IMAGE_HEIGHT;
	}
	
	private Rect GetButtonPos(int row, int column) {
		int left = SCROLL_BAR_WIDTH + column * BUILD_IMAGE_WIDTH;
		float top = row * BUILD_IMAGE_HEIGHT - sliderValue * BUILD_IMAGE_HEIGHT;
		return new Rect(left, top, BUILD_IMAGE_WIDTH, BUILD_IMAGE_HEIGHT);
	}
	
	private void DrawSlider(int groupHeight, float numRows) {
		//slider goes from 0 to the number of rows that do not fit on screen
		sliderValue = GUI.VerticalSlider(GetScrollPos(groupHeight), sliderValue, 0.0f, numRows - MaxNumRows(groupHeight));
	}

	private Rect GetScrollPos(int groupHeight) {
		return new Rect(BUTTON_SPACING, BUTTON_SPACING, SCROLL_BAR_WIDTH, groupHeight - 2 * BUTTON_SPACING);
	}

}
