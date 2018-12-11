using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Valve.VR;

public class RightController : MonoBehaviour {

	public Transform transform;
	public bool PressForMovement = false;
	public bool Teleport = true;
	public bool Running = false;
	public List<Vector2> PlayerPosition;
	public int playerX, playerZ, OldPlayerX, OldPlayerZ, mazeRows, mazeColumns;

//	private SteamVR_TrackedObject trackedObject;
//	private SteamVR_Controller.Device device;
	private List<GameObject> walls;
//	private MazeLoader MazeLoaderScript;
	private Vector2 wallPosition;
	private Vector3 playerPosition;
	private bool ButtonHasBeenPressed, ValidPosition;

	void Start() {
		PlayerPosition = new List<Vector2> ();
//		trackedObject = GetComponent<SteamVR_TrackedObject> ();
		walls = new List<GameObject> ();
		foreach (GameObject gameObject in GameObject.FindObjectsOfType(typeof(GameObject))) {
			if (gameObject.name.Contains ("Wall")) {
				walls.Add (gameObject);
			}
		}
	}

	void FixedUpdate() {/*
		device = SteamVR_Controller.Input ((int) trackedObject.index);
		if (device.GetTouch (SteamVR_Controller.ButtonMask.Touchpad) && (PressForMovement ? device.GetPress(SteamVR_Controller.ButtonMask.Touchpad) : true)) {
			if(Teleport) {
				TeleportMovement ();
			} else if (Running){
				RunningMovement ();
			}
		}*/
	}

	private void TeleportMovement() {/*
		
		if (!device.GetPress (SteamVR_Controller.ButtonMask.Touchpad)) {
			ButtonHasBeenPressed = false;
			return;
		}

		if (ButtonHasBeenPressed) {
			return;
		}
		ButtonHasBeenPressed = true;

		if (MazeLoaderScript == null) {
			if (GameObject.Find ("[CameraRig]") != null) {
				MazeLoaderScript = GameObject.Find ("[CameraRig]").GetComponent<MazeLoader> ();
				mazeRows = MazeLoaderScript.mazeRows;
				mazeColumns = MazeLoaderScript.mazeColumns;
			}
		} else {
			// Touch Input
			OldPlayerX = playerX;
			OldPlayerZ = playerZ;
			MazeCell[,] mazeCells = MazeLoaderScript.mazeCells; 
			// In maze the walls are rotate 90 degrees left
			if(device.GetAxis().x >= 0.7f) { // Rechts
				playerX = playerX >= MazeLoaderScript.mazeRows - 1 ? playerX : playerX + 1;
				ValidPosition = mazeCells [OldPlayerX, OldPlayerZ].southWall == null && mazeCells [playerX, playerZ].northWall == null;
			} else if (device.GetAxis().x <= -0.7f) { // Links
				playerX = playerX <= 0 ? 0 : playerX - 1;
				ValidPosition = mazeCells [OldPlayerX, OldPlayerZ].northWall == null && mazeCells [playerX, playerZ].southWall == null;
			}
			if(device.GetAxis().y >= 0.7f) { // Oben
				playerZ = playerZ >= MazeLoaderScript.mazeColumns - 1 ? playerZ : playerZ + 1;
				ValidPosition = mazeCells [OldPlayerX, OldPlayerZ].eastWall == null && mazeCells [playerX, playerZ].westWall == null;
			} else if (device.GetAxis().y <= -0.7f) { // Unten
				playerZ = playerZ <= 0 ? 0 : playerZ - 1;
				ValidPosition = mazeCells [OldPlayerX, OldPlayerZ].westWall == null && mazeCells [playerX, playerZ].eastWall == null;
			}
			if (ValidPosition) {
				Vector3 floorPosition = MazeLoaderScript.mazeCells [playerX, playerZ].floor.transform.position;
				transform.position = new Vector3 (floorPosition.x, -1f, floorPosition.z);
				PlayerPosition.Add (new Vector2(floorPosition.x, floorPosition.z));
				ValidPosition = false;
			} else {
				playerX = OldPlayerX;
				playerZ = OldPlayerZ;
			}
		}*/
	}

	private void RunningMovement() {/*
		// Neue Position berechnen
		Vector3 newPlayerPosition = transform.position + transform.right * Time.deltaTime * device.GetAxis ().x;
		newPlayerPosition = (newPlayerPosition + transform.right * Time.deltaTime * device.GetAxis ().x) +  transform.forward * Time.deltaTime * 3 * device.GetAxis ().y;

		bool collision = false;

		// Für jede Wand Überschneidung überprüfen
		foreach (GameObject gameObject in walls) {
			if (checkForCollision(gameObject.transform.position, gameObject.transform.localScale, newPlayerPosition)) { // Überschneidung prüfen
				collision = true;
				break;
			}
		}

		// Wenn nicht geschnitten: Setze, Sonst: ignoriere
		if (!collision) {
			transform.position = newPlayerPosition;
			PlayerPosition.Add (new Vector2(newPlayerPosition.x, newPlayerPosition.z));
		}
		// transform.position = transform.position + transform.right * Time.deltaTime * device.GetAxis ().x;
		// transform.position = (transform.position + transform.right * Time.deltaTime * device.GetAxis ().x) +  transform.forward * Time.deltaTime * 3 * device.GetAxis ().y;
		*/
	}

	private bool checkForCollision(Vector3 wallPosition, Vector3 wallScale, Vector3 newPlayerPosition) {
		// Debug.Log(wallPosition.x +" "+ newPlayerPosition.x+" "+ (wallPosition.x + wallScale.x));
		return (wallPosition.x <= newPlayerPosition.x && newPlayerPosition.x <= wallPosition.x + wallScale.x)
			&& (wallPosition.z <= newPlayerPosition.z && newPlayerPosition.z <= wallPosition.z + wallScale.z);
	}
}