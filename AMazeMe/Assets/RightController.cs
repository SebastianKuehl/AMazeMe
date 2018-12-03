using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class RightController : MonoBehaviour {

	public Transform transform;
	public bool PressForMovement = false;
	public bool Teleport = true;
	public bool Running = false;
	public List<Vector2> PlayerPosition;

	private SteamVR_TrackedObject trackedObject;
	private SteamVR_Controller.Device device;
	private List<GameObject> walls;
	private MazeLoader MazeLoaderScript;
	private Vector2 wallPosition;
	private Vector3 playerPosition;
	private bool ButtonHasBeenPressed, ValidPosition, WASD_Keys;
	private int playerX, playerZ, OldPlayerX, OldPlayerZ;

	void Start() {
		PlayerPosition = new List<Vector2> ();
		trackedObject = GetComponent<SteamVR_TrackedObject> ();
		walls = new List<GameObject> ();
		foreach (GameObject gameObject in GameObject.FindObjectsOfType(typeof(GameObject))) {
			if (gameObject.name.Contains ("Wall")) {
				walls.Add (gameObject);
			}
		}
	}

	void FixedUpdate() {
		device = SteamVR_Controller.Input ((int) trackedObject.index);
        WASD_Keys = Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d");

        if ((device.GetTouch (SteamVR_Controller.ButtonMask.Touchpad) && (PressForMovement ? device.GetPress(SteamVR_Controller.ButtonMask.Touchpad) : true)) || WASD_Keys) {
			if(Teleport) {
				TeleportMovement ();
			} else if (Running){
				RunningMovement ();
			}
		}
	}

	private void TeleportMovement() {
		
		if (!device.GetPress (SteamVR_Controller.ButtonMask.Touchpad) || WASD_Keys) {
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
			}
		} else {
			// Touch Input
			OldPlayerX = playerX;
			OldPlayerZ = playerZ;
			MazeCell[,] mazeCells = MazeLoaderScript.mazeCells; 
			// In maze the walls are rotated 90 degrees to the left
			if(device.GetAxis().x >= 0.7f) { // Right
				playerX = playerX >= MazeLoaderScript.mazeRows - 1 ? playerX : playerX + 1;
				ValidPosition = mazeCells [OldPlayerX, OldPlayerZ].southWall == null && mazeCells [playerX, playerZ].northWall == null;
			} else if (device.GetAxis().x <= -0.7f) { // Left
				playerX = playerX <= 0 ? 0 : playerX - 1;
				ValidPosition = mazeCells [OldPlayerX, OldPlayerZ].northWall == null && mazeCells [playerX, playerZ].southWall == null;
			}
			if(device.GetAxis().y >= 0.7f) { // Up
				playerZ = playerZ >= MazeLoaderScript.mazeColumns - 1 ? playerZ : playerZ + 1;
				ValidPosition = mazeCells [OldPlayerX, OldPlayerZ].eastWall == null && mazeCells [playerX, playerZ].westWall == null;
			} else if (device.GetAxis().y <= -0.7f) { // Down
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
		}
	}

	private void RunningMovement() {
		// Calculate new Position
		Vector3 newPlayerPosition = transform.position + transform.right * Time.deltaTime * device.GetAxis ().x;
		newPlayerPosition = (newPlayerPosition + transform.right * Time.deltaTime * device.GetAxis ().x) +  transform.forward * Time.deltaTime * 3 * device.GetAxis ().y;

		bool collision = false;

		// Check Collision for each wall on axis x and z
		foreach (GameObject gameObject in walls) {
			if (checkForCollision(gameObject.transform.position, gameObject.transform.localScale, newPlayerPosition)) {
				collision = true;
				break;
			}
		}

		if (!collision) {
			transform.position = newPlayerPosition;
			PlayerPosition.Add (new Vector2(newPlayerPosition.x, newPlayerPosition.z));
		}
	}

	private bool checkForCollision(Vector3 wallPosition, Vector3 wallScale, Vector3 newPlayerPosition) {
		return (wallPosition.x <= newPlayerPosition.x && newPlayerPosition.x <= wallPosition.x + wallScale.x)
			&& (wallPosition.z <= newPlayerPosition.z && newPlayerPosition.z <= wallPosition.z + wallScale.z);
	}


}