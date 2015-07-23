using UnityEngine;
using System.Collections;
using RTS;

public class Worker : Unit {

	public float capacity;
	
	private bool harvesting = false, emptying = false;
	private float currentLoad = 0.0f;
	private ResourceType harvestType;
	
	protected override void Awake() {
		base.Awake();
	}
	
	protected override void Start () {
		base.Start();
	}
	
	protected override void Update () {
		base.Update();
	}
	
	protected override void OnGUI() {
		base.OnGUI();
	}
	
	public override void PerformAction(string actionToPerform) {
		base.PerformAction (actionToPerform);
	}
	
	private void StartHarvest(Resource resource) {
		
	}
	
	private void StopHarvest() {
		
	}

}
