using UnityEngine;
using System.Collections;
using RTS;

public class Worker : Unit {

	// Harvesting variables
	public float capacity = 5;
	private bool harvesting = false, startHarvest = false, emptying = false, startEmpty = false;
	private float currentLoad = 0;
	private ResourceType harvestType;
	private Resource resourceDeposit;
	private Building[] resourceStores;

	private ArrayList ignoredCollisions;

	public float collectionAmount;
	
	// Simple move for workers
	protected bool moving, rotating;
	private Vector3 destination;
	private Quaternion targetRotation;
	
	protected override void Awake() {
		base.Awake();
	}
	
	protected override void Start () {
		base.Start();
		ignoredCollisions = new ArrayList ();;
	}
	
	protected override void Update () {
		base.Update();

		if(rotating) TurnToTarget();
		else if(moving) MakeMove();

		if (harvesting || emptying) {
			if (harvesting && startHarvest) {
				Collect ();
				if (currentLoad >= capacity || resourceDeposit.isEmpty ()) {
					//make sure that we have a whole number to avoid bugs
					//caused by floating point numbers
					currentLoad = Mathf.Floor(currentLoad);
					harvesting = false;
					startHarvest = false;
					emptying = true;
					StartMove (resourceStores[0].transform.position);
				}
			} else if(startEmpty) {
				Deposit ();
				if (currentLoad <= 0) {
					emptying = false;
					startEmpty = false;
					if (!resourceDeposit.isEmpty ()) {
						harvesting = true;
						StartMove (resourceDeposit.transform.position);
					}
				}
			}
		}
	}
	
	protected override void OnGUI() {
		base.OnGUI();
	}

	private void Collect() {
		float collect = collectionAmount * Time.deltaTime;
		//make sure that the harvester cannot collect more than it can carry
		if(currentLoad + collect > capacity) collect = capacity - currentLoad;
		resourceDeposit.Remove(collect);
		currentLoad += collect;
	}
	
	private void Deposit() {
		ResourceType depositType = harvestType;
		if(harvestType == ResourceType.Materials) depositType = ResourceType.Materials;
		player.AddResource(depositType, (int) currentLoad);
		currentLoad = 0;
	}

	

	void OnCollisionEnter(Collision collision) {

		// Rigidbodies colliding with eachother
		if (harvesting && collision.gameObject.tag == "Worker") {
			ignoredCollisions.Add(collision);
			Physics.IgnoreCollision (collision.collider, GetComponent<Collider> (), true);
		}

	}

	void OnTriggerEnter(Collider other) {
		// Triggers colliding with rigidbodies
		if(harvesting && other.gameObject.tag == "Resource") {
			startHarvest = true;
		} else if(emptying && other.gameObject.tag == "Base") {
			startEmpty = true;
		}

		if (harvesting && other.gameObject.tag == "Resource") {
			moving = false; // Stop moving
		} 
	
	}

	public override void ObjectGotRightClicked(Player byPlayer) {
		base.ObjectGotRightClicked (byPlayer);
	}
	
	public override void DoRightClickAction(GameObject hitObject) {
		base.DoRightClickAction (hitObject);
		Resource resource = hitObject.transform.GetComponent< Resource > ();
		if (resource && !resource.isEmpty ()) {
			StartHarvest (resource);
		} else {
			StopHarvest ();
		}

	}
	
	public override void PerformAction(string actionToPerform) {
		base.PerformAction (actionToPerform);
	}
	
	private void StartHarvest(Resource resource) {
		resourceDeposit = resource;
		resourceStores = player.GetComponentsInChildren<Building>();
		MoveToLocation(resource.transform.position);
		//we can only collect one resource at a time, other resources are lost
		if(harvestType != resource.GetResourceType()) {
			harvestType = resource.GetResourceType();
			currentLoad = 0.0f;
		}
		harvesting = true;
		emptying = false;

	}
	
	private void StopHarvest() {
		harvesting = false;
		startHarvest = false;
		startEmpty = false;
		moving = false;
		rotating = false;

		// Re-enable the colliders
		for (int i = 0; i < ignoredCollisions.Count; i++) {
			Collision c = ignoredCollisions[i] as Collision;
			Physics.IgnoreCollision(c.collider, GetComponent<Collider>(), false);
		}
	}

	
	protected void StartMove(Vector3 destination) {
		this.destination = destination;
		targetRotation = Quaternion.LookRotation (destination - transform.position);
		rotating = true;
		moving = false;
	}
	
	private void MakeMove() {
		Vector3 moveVector = (destination - transform.position).normalized * moveSpeed;
		rb.MovePosition (transform.position + moveVector * Time.deltaTime);
//		transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * moveSpeed);
//		if(transform.position == destination) moving = false;
	}
	
	private void TurnToTarget() {
		transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed);
		//sometimes it gets stuck exactly 180 degrees out in the calculation and does nothing, this check fixes that
		Quaternion inverseTargetRotation = new Quaternion(-targetRotation.x, -targetRotation.y, -targetRotation.z, -targetRotation.w);
		if(transform.rotation == targetRotation || transform.rotation == inverseTargetRotation) {
			rotating = false;
			moving = true;
		}
	}

}
