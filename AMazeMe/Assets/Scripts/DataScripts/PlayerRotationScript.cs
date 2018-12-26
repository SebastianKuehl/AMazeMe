﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotationScript : MonoBehaviour {

	public int playerYRotation;

	private Transform viveCameraTransform;
	private GameObject cameraObj;

	void Start() {
        cameraObj = GameObject.Find ("Camera (eye)");
	}

	void FixedUpdate() {
		viveCameraTransform = cameraObj.GetComponent<Transform>();
		if (viveCameraTransform != null) {
            int newValue = (int) (viveCameraTransform.eulerAngles.y);
            if (newValue % 10 == 0 && newValue != playerYRotation) {
                playerYRotation = newValue;
            }
        }
	}
}
