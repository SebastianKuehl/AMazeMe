using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightScript : MonoBehaviour {

	public Transform controllerTransform;
	public Transform cameraTransform;

	private DataScript data;
	private ViveControllerScript rightController;

	void Start() {
		rightController = GetComponent<ViveControllerScript> ();
		GameObject dataObj = GameObject.Find ("Settings");
		data = dataObj.GetComponent<DataScript> ();
	}

	// Update is called once per frame
	void Update () {
        if (!data) {
            return;
        }

		if (data.useController) {
			useFlashLight ();
		} else {
			useHeadLight ();
		}
	}

	private void useHeadLight() {
        if(!gameObject || !cameraTransform) {
            return;
        }

		gameObject.transform.position = cameraTransform.position;
		gameObject.transform.rotation = cameraTransform.rotation;
	}

	private void useFlashLight() {
        if (!rightController) {
            return;
        }

		if (rightController.TouchpadTouched()) {
			gameObject.transform.position = controllerTransform.position;
			gameObject.transform.rotation = controllerTransform.rotation;
		} else {
			gameObject.transform.position = new Vector3 (0f, -30f, 0f);
		}
	}
}
