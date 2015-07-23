using UnityEngine;
using System.Collections;

public class WorldObject : MonoBehaviour {

	// Object variables
	public string objectName;

	// Protected variables
	protected string[] actions = {};

	// Object bounds
	protected Bounds selectionBounds;

	protected virtual void Awake() {
		CalculatedBounds ();
	}
	
	protected virtual void Start () {

	}
	
	protected virtual void Update () {
		
	}
	
	protected virtual void OnGUI() {
	}

//	public void SetSelection(bool selected) {
//		currentlySelected = selected;
//	}

	public virtual void ObjectGotRightClicked(Player byPlayer) {
		// Do nothing
	}

	public string[] GetActions() {
		return actions;
	}
	
	public virtual void PerformAction(string actionToPerform) {
		//it is up to children with specific actions to determine what to do with each of those actions
	}

	public void CalculatedBounds () {
		selectionBounds = new Bounds(transform.position, Vector3.zero);
		foreach(Renderer r in transform.FindChild("Model").GetComponentsInChildren< Renderer >()) {
			selectionBounds.Encapsulate(r.bounds);
		}
	}

}
