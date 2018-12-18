using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ViveControllerScript : MonoBehaviour {
    
	private SteamVR_TrackedObject trackedObject;
	private SteamVR_Controller.Device device;
	private Vector2 wallPosition;
	private Vector3 playerPosition;
	private bool ValidPosition;

	void Start() {
		trackedObject = GetComponent<SteamVR_TrackedObject> ();
        device = SteamVR_Controller.Input((int)trackedObject.index);
    }

    public bool TouchpadTouched() {
        return device.GetTouch(SteamVR_Controller.ButtonMask.Touchpad);
    }

    public bool TouchpadPressed() {
        return device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad);
    }

    public bool TouchpadTouchRight() {
        return device.GetAxis().x >= 0.7f;
    }

    public bool TouchpadTouchLeft() {
        return device.GetAxis().x <= -0.7f;
    }

    public bool TouchpadTouchUp() {
        return device.GetAxis().y >= 0.7f;
    }

    public bool TouchpadTouchDown() {
        return device.GetAxis().y <= -0.7f;
    }

    public Vector2 TouchPosition() {
        return device.GetAxis();
    }
}