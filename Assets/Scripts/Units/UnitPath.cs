using UnityEngine;
using System.Collections;
using Pathfinding;

public class UnitPath : MonoBehaviour {

	private Seeker seeker;
	private CharacterController controller;
	public Path path;
	private Unit unit;
	
	public float speed;
	
	// The max distance from the AI to a waypoint for it to continue to the next waypoint
	public float nextWaypointDistance = 5;
	
	// Current waypoint (always starts at index 0)
	private int currentWaypoint = 0;

	public void Start() {
		seeker = GetComponent<Seeker> ();
		controller = GetComponent<CharacterController> ();
		unit = GetComponent<Unit> ();
	}


	public void LateUpdate() {
		if (unit.selected && unit.isWalkable) {
			if(Input.GetMouseButtonDown(1)){
				// Set path
				seeker.StartPath (transform.position, Mouse.rightClickPoint, OnPathComplete);
			}
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
		Vector3 dir = (path.vectorPath [currentWaypoint] - transform.position).normalized;
		dir *= speed * Time.fixedDeltaTime;
		
		controller.SimpleMove (dir); // Unit moves here!
		
		// Check if close enough to the current waypoint, if we are, proceed to next waypoint
		if (Vector3.Distance (transform.position, path.vectorPath [currentWaypoint]) < nextWaypointDistance) {
			currentWaypoint++;
			return;
		}
	}
}
