using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RTS;

public class Building : PlayerObject {
	
	public float maxBuildProgress;
	public bool hasSpawnPoint;

	protected Queue< string > buildQueue;
	private float currentBuildProgress = 0.0f;
	private Vector3 spawnPoint;

	protected Vector3 rallyPoint;

	protected override void Awake() {
		base.Awake();
		
		// Initialize a build queue
		buildQueue = new Queue< string >();
		float spawnX = selectionBounds.center.x - transform.forward.x * selectionBounds.extents.x - transform.forward.x * 1;
		float spawnZ = selectionBounds.center.z - transform.forward.z * selectionBounds.extents.z - transform.forward.z * 1;
		spawnPoint = new Vector3(spawnX, 0.0f, spawnZ);
		rallyPoint = new Vector3(spawnX - transform.forward.x * 3, 0.0f, spawnZ - transform.forward.z * 3);
		
		Debug.Log (selectionBounds.center);
		Debug.Log (selectionBounds.extents);
		Debug.Log (transform.forward);
		Debug.Log (spawnPoint);
		Debug.Log (rallyPoint);
	}
	
	protected override void Start () {
		base.Start();
		if (hasSpawnPoint) {
			SetRallyPoint (rallyPoint);
		}
	}
	
	protected override void Update () {
		base.Update();

		if(player && hasSpawnPoint) {
			RallyPoint rallyPt = player.GetComponentInChildren< RallyPoint >();
			if(selected) {
				if(rallyPt && player.isHuman && spawnPoint != null && rallyPoint != null) {
//					rallyPt.transform.position = rallyPoint;
//					rallyPt.transform.forward = transform.forward;
					rallyPt.Enable();
				}
			} else {
				if(rallyPt && player.isHuman) rallyPt.Disable();
			}
		}

		ProcessBuildQueue();
	}
	
	public void LateUpdate() {
		if (hasSpawnPoint && selected) {
			if(Input.GetMouseButtonDown(1)){
				// Set rally point on click position
				SetRallyPoint (Mouse.rightClickPoint);
			}
		}
	}
	
	protected override void OnGUI() {
		base.OnGUI();
	}
	
	public override void PerformAction(string actionToPerform) {
		base.PerformAction (actionToPerform);
	}

	protected void CreateUnit(string unitName) {
		buildQueue.Enqueue(unitName);
	}

	protected void ProcessBuildQueue() {
		if(buildQueue.Count > 0) {
			currentBuildProgress += Time.deltaTime * ResourceManager.BuildSpeed;
			if(currentBuildProgress > maxBuildProgress) {
				if(player) player.AddUnit(buildQueue.Dequeue(), spawnPoint, rallyPoint, transform.rotation);
				currentBuildProgress = 0.0f;
			}
		}
	}
	
	public string[] getBuildQueueValues() {
		string[] values = new string[buildQueue.Count];
		int pos = 0;
		foreach(string unit in buildQueue) values[pos++] = unit;
		return values;
	}
	
	public float getBuildPercentage() {
		return currentBuildProgress / maxBuildProgress;
	}

	public void SetRallyPoint(Vector3 position) {
		rallyPoint = position;
		if(player && player.isHuman) {
			RallyPoint rallyPt = player.GetComponentInChildren< RallyPoint >();
			if(rallyPt) {
				rallyPt.transform.position = rallyPoint;
				LineRenderer line = rallyPt.GetComponentInChildren<LineRenderer>();
				line.SetPosition(0, new Vector3(transform.position.x, 5f, transform.position.z));
				line.SetPosition(1, position);
			}
		}
	}

}
