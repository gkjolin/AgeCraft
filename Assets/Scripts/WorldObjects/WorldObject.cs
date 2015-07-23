using UnityEngine;
using System.Collections;

public class WorldObject : MonoBehaviour {

	// Object variables
	public string objectName;
	public Texture2D buildImage;
	public int cost, hitPoints, maxHitPoints;

	// Protected variables
	protected Player player;
	protected string[] actions = {};
//	protected bool currentlySelected = false;

	// Object bounds
	protected Bounds selectionBounds;

	protected virtual void Awake() {
		CalculatedBounds ();
	}
	
	protected virtual void Start () {
		player = transform.root.GetComponentInChildren< Player >();
	}
	
	protected virtual void Update () {
		
	}
	
	protected virtual void OnGUI() {
	}

//	public void SetSelection(bool selected) {
//		currentlySelected = selected;
//	}

	public string[] GetActions() {
		return actions;
	}
	
	public virtual void PerformAction(string actionToPerform) {
		//it is up to children with specific actions to determine what to do with each of those actions
	}

	public void CalculatedBounds () {
		selectionBounds = new Bounds(transform.position, Vector3.zero);
		foreach(Renderer r in GetComponentsInChildren< Renderer >()) {
			selectionBounds.Encapsulate(r.bounds);
		}
	}

}
