using UnityEngine;
using System.Collections;

public class DestroyRootGameObject : MonoBehaviour {

	void DestroyObject () {
		Destroy (this.gameObject.transform.root.gameObject);
	}
}
