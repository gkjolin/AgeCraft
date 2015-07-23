﻿using UnityEngine;
using System.Collections;
using Pathfinding;

public class UnitPath : MonoBehaviour {

	private Seeker seeker;
	private CharacterController controller;
	public Path path;
	private Unit unit;
	
	// The max distance from the AI to a waypoint for it to continue to the next waypoint
	public float nextWaypointDistance = 5;
	
	// Current waypoint (always starts at index 0)
	private int currentWaypoint = 0;
	
	public void Awake() {
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

	public void MoveToLocation(Vector3 newPosition) {
		Debug.Log (unit);
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
		Vector3 dir = (path.vectorPath [currentWaypoint] - transform.position).normalized;
		dir *= unit.moveSpeed * Time.fixedDeltaTime;

		// Have unit face forwards
		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z)), unit.rotationSpeed * Time.fixedDeltaTime);

		// Have unit move forwards
		controller.SimpleMove (dir);
		
		// Check if close enough to the current waypoint, if we are, proceed to next waypoint
		if (Vector3.Distance (transform.position, path.vectorPath [currentWaypoint]) < nextWaypointDistance) {
			currentWaypoint++;
			return;
		}
	}
}
