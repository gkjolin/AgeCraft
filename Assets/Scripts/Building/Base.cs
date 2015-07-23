using UnityEngine;
using System.Collections;

public class Base : Building {
	
	protected override void Awake() {
		base.Awake();
	}
	
	protected override void Start () {
		base.Start();
		actions = new string[] { "Worker" };
	}
	
	protected override void Update () {
		base.Update();
	}
	
	protected override void OnGUI() {
		base.OnGUI();
	}

	public override void PerformAction(string actionToPerform) {
		base.PerformAction(actionToPerform);
		CreateUnit(actionToPerform);
	}
}
