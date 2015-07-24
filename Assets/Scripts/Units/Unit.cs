using UnityEngine;
using System.Collections;
using Pathfinding;
using RTS;

/*
 * This Script should be attached to all controllable units in the game, wether they are walkable or not
 * */

public class Unit : PlayerObject {
	
	private Seeker seeker;
	public Path path;
	protected Unit unit;
	protected Rigidbody rb;

	// Movement-related stuff
	public bool isWalkable = true;
	public float moveSpeed;
	public float rotationSpeed;
	
	// The max distance from the AI to a waypoint for it to continue to the next waypoint
	public float nextWaypointDistance = 2;
	
	// Current waypoint (always starts at index 0)
	private int currentWaypoint = 0;

	protected override void Awake(){
		base.Awake();
		seeker = GetComponent<Seeker> ();
		unit = GetComponent<Unit> ();
		rb = GetComponent<Rigidbody>();
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
	
	public override void ObjectGotRightClicked(Player byPlayer) {
		base.ObjectGotRightClicked (byPlayer);
	}
	
	public override void DoRightClickAction(GameObject hitObject) {
		base.DoRightClickAction (hitObject);

		if (hitObject.name == "Ground") {
			MoveToLocation(player.mouse.rightClickPoint);
		} else {
			Player objectOwner = hitObject.transform.root.gameObject.GetComponent<Player> ();
			if (player.Equals (objectOwner)) {
				// You right-clicked another unit you own
				Debug.Log ("You right-clicked another unit you own");
				//			unitPath.MoveToLocation(hitObject.transform.position);
			} else {
				// You right-clicked something you don't own
				Debug.Log ("You right-clicked something you don't own");
				if (objectOwner) {
					// Someone else owns this object
					Debug.Log ("Someone else owns this object");
				} else {
					// None owns this object
					Debug.Log ("None owns this object");
				}
				
			}
		}
	}
	
	public override void PerformAction(string actionToPerform) {
		base.PerformAction (actionToPerform);
	}
	
	public void MoveToLocation(Vector3 newPosition) {
		if (unit.isWalkable) {
			// Set path
			seeker.StartPath (transform.position, newPosition, OnPathComplete);
		}
	}

	
	// Pathfinding logic
	public void OnPathComplete(Path p) {
		if (!p.error) {
			path = p;
			// Reset waypoint counter
			currentWaypoint = 0;
		}
	}
	
	public void FixedUpdate() {
		if (!unit.isWalkable)
			return;
		
		if (path == null)
			return;
		
		if (currentWaypoint >= path.vectorPath.Count)
			return;
		
		// Calculate direction of unit
		Vector3 dir = (path.vectorPath [currentWaypoint] - transform.position).normalized * unit.moveSpeed;
//		float step = unit.moveSpeed * Time.fixedDeltaTime;
		
		// Have unit face forwards
//		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z)), unit.rotationSpeed * Time.fixedDeltaTime);
		rb.MoveRotation (Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z)), unit.rotationSpeed * Time.fixedDeltaTime));
		
		// Have unit move forwards
//		Debug.Log (dir);
//		controller.SimpleMove (dir);
//		transform.position = Vector3.MoveTowards(transform.position, path.vectorPath [currentWaypoint], step);
//		transform.Translate (dir * Time.fixedDeltaTime);
		rb.MovePosition (transform.position + dir * Time.fixedDeltaTime);

		
		// Check if close enough to the current waypoint, if we are, proceed to next waypoint
		if (Vector3.Distance (transform.position, path.vectorPath [currentWaypoint]) < nextWaypointDistance) {
			currentWaypoint++;
			return;
		}
	}
	
}
