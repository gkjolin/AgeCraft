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

	public float collectionAmount, depositAmount;
	private float currentDeposit = 0.0f;
	
	// Simple move for workers
	protected bool moving, rotating;
	private Vector3 destination;
	private Quaternion targetRotation;
	
	protected override void Awake() {
		base.Awake();
	}
	
	protected override void Start () {
		base.Start();
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
	
	void OnTriggerEnter(Collider other) {
		
		if (harvesting && other.gameObject.tag == "Worker") {
			Physics.IgnoreCollision (other.GetComponent<Collider>(), GetComponent<Collider> (), true);
		} else if(harvesting && other.gameObject.tag == "Resource") {
			startHarvest = true;
		} else if(emptying && other.gameObject.tag == "Base") {
			startEmpty = true;
		} else if (!harvesting && other.gameObject.tag == "Worker") {
			Physics.IgnoreCollision (other.GetComponent<Collider>(), GetComponent<Collider> (), false);
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
	}

	
	protected void StartMove(Vector3 destination) {
		this.destination = destination;
		targetRotation = Quaternion.LookRotation (destination - transform.position);
		rotating = true;
		moving = false;
	}
	
	private void MakeMove() {
		Vector3 moveVector = (destination - transform.position).normalized * moveSpeed;
		rigidbody.MovePosition (transform.position + moveVector * Time.deltaTime);
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
