using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RTS;

public class Player : MonoBehaviour {

	// Player initialization
	private MinimapController minimapControl;
	private HUD hud;

	// Player materials
	public int startMaterials;
	public int startEnergy;
	private Dictionary< ResourceType, int > resources;


	public string username;
	public bool isHuman;

	void Awake() {
		resources = InitResourceList();
	}

	void Start() {
		minimapControl = transform.root.GetComponent<MinimapController>();
		hud = transform.root.GetComponent<HUD>();

		AddStartResources();
	}
	
	// Update is called once per frame
	void Update () {
		if (this.isHuman) {
			minimapControl.ShowMinimap();
			hud.SetResourceValues(resources);
		}
	}

	void OnGUI() {
		if (this.isHuman) {
			minimapControl.RenderViewportBox();
		}
	}

	private Dictionary< ResourceType, int > InitResourceList() {
		Dictionary< ResourceType, int > list = new Dictionary< ResourceType, int >();
		list.Add(ResourceType.Materials, 0);
		list.Add(ResourceType.Energy, 0);
		return list;
	}

	private void AddStartResources() {
		AddResource(ResourceType.Materials, startMaterials);
		AddResource(ResourceType.Energy, startEnergy);
	}

	public void AddResource(ResourceType type, int amount) {
		resources[type] += amount;
	}

}
