using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class RightController : MonoBehaviour {

	public Transform transform;
	public bool PressForMovement = false;

	private SteamVR_TrackedObject trackedObject;
	private SteamVR_Controller.Device device;

	private List<GameObject> walls;
	private Vector2 wallPosition;
	private Vector3 playerPosition;

	void Start() {
		trackedObject = GetComponent<SteamVR_TrackedObject> ();
		walls = new List<GameObject> ();
		foreach (GameObject gameObject in GameObject.FindObjectsOfType(typeof(GameObject))) {
			if (gameObject.name.Contains ("Wall")) {
				walls.Add (gameObject);
			}
		}
	}

	void Update() {
		device = SteamVR_Controller.Input ((int) trackedObject.index);
		if (device.GetTouch (SteamVR_Controller.ButtonMask.Touchpad) && (PressForMovement ? device.GetPress(SteamVR_Controller.ButtonMask.Touchpad) : true)) {

			// Neue Position berechnen
			Vector3 newPlayerPosition = transform.position + transform.right * Time.deltaTime * device.GetAxis ().x;
			newPlayerPosition = (newPlayerPosition + transform.right * Time.deltaTime * device.GetAxis ().x) +  transform.forward * Time.deltaTime * 3 * device.GetAxis ().y;

			bool collision = false;

			// Für jede Wand Überschneidung überprüfen
			foreach (GameObject gameObject in walls) {
				if (checkForCollision(gameObject.transform.position, newPlayerPosition)) { // Überschneidung prüfen
					collision = true;
					break;
				}
			}

			// Wenn nicht geschnitten: Setze, Sonst: ignoriere
			if (!collision) {
				transform.position = newPlayerPosition;
			}
			// transform.position = transform.position + transform.right * Time.deltaTime * device.GetAxis ().x;
			// transform.position = (transform.position + transform.right * Time.deltaTime * device.GetAxis ().x) +  transform.forward * Time.deltaTime * 3 * device.GetAxis ().y;
		}
	}

	private bool checkForCollision(Vector3 wallPosition, Vector3 newPlayerPosition) {
		return false;
	}
}