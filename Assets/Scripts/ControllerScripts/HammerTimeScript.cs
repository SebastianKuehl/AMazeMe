using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerTimeScript : MonoBehaviour {

	public GameObject hammer;

	private ViveControllerScript rightController;

	void Start() {
		rightController = GetComponent<ViveControllerScript> ();
	}

	// Update is called once per frame
	void Update () {
		if (rightController.TriggerDown()) {
			hammer.transform.position = gameObject.transform.position;
			Quaternion controllerPos = gameObject.transform.rotation;
			hammer.transform.rotation = gameObject.transform.rotation;//new Quaternion (controllerPos.x, controllerPos.y, controllerPos.z, 1f );
			hammer.transform.Rotate(new Vector3(90f, 0f, 0f), Space.Self);
		} else {
			hammer.transform.position = new Vector3 (0f, -30f, 0f);
		}
	}
}
