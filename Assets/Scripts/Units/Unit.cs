using UnityEngine;
using System.Collections;
using RTS;

/*
 * This Script should be attached to all controllable units in the game, wether they are walkable or not
 * */

public class Unit : PlayerObject {

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
	
}
