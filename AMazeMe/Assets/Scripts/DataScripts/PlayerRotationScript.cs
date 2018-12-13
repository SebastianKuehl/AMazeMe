using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotationScript : MonoBehaviour {

	public int playerYRotation;

	private Transform viveCameraTransform;
	private GameObject camera;

	void Start() {
		camera = GameObject.Find ("Camera (eye)");
	}

	void FixedUpdate() {
		viveCameraTransform = camera.GetComponent<Transform>();
		if (viveCameraTransform != null) {
			this.playerYRotation = (int) (viveCameraTransform.rotation.y * 100);
		}
	}
}
