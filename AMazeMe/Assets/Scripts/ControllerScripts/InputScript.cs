using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputScript : MonoBehaviour {

    public bool inputByKeyboard = true;
   
    private GameObject rightController;
    private ViveControllerScript rightControllerScript;
    private Transform cameraTransform;
    private MazeLoader MazeLoaderScript;
    private Vector2 wallPosition;
    private Vector3 playerPosition;
    private bool validPosition;
    private MazeCell[,] mazeCells;
	private int[,] mazeStructure;
	private int playerX, playerZ, OldPlayerX, OldPlayerZ, mazeRows, mazeColumns;

    void Start() {
        rightController = GameObject.Find("Controller (right)");
    }

    void FixedUpdate() {
        // Load all the needed data if not done before
        if (MazeLoaderScript == null) {
            GameObject cameraRig = GameObject.Find("[CameraRig]");
            if (cameraRig != null) {
                cameraTransform = cameraRig.GetComponent<Transform>();
                MazeLoaderScript = cameraRig.GetComponent<MazeLoader>();
                mazeRows = MazeLoaderScript.mazeRows;
                mazeColumns = MazeLoaderScript.mazeColumns;
				mazeCells = MazeLoaderScript.GetMazeCells();
				mazeStructure = MazeLoaderScript.GetMazeStructure ();
            }
        } else {
            // Handle the chosen method of input
            if (inputByKeyboard) {
                HandleKeyboardInput();
            } else {
                HandleControllerInput();
            }
        }
    }

    private void HandleKeyboardInput() {
        ControlMovement(Input.GetKeyDown("w"), Input.GetKeyDown("s"), Input.GetKeyDown("a"), Input.GetKeyDown("d"));
    }

    private void HandleControllerInput() {
        if (rightController == null) {
            rightController = GameObject.Find("Controller (right)");
        } else if (rightControllerScript == null) {
            rightControllerScript = rightController.GetComponent<ViveControllerScript>();
        } else {
            if (rightControllerScript.TouchpadPressed()) {
				ControlMovement(rightControllerScript.TouchpadTouchUp(),
					rightControllerScript.TouchpadTouchDown(),
					rightControllerScript.TouchpadTouchLeft(),
					rightControllerScript.TouchpadTouchRight());
            }
        }
    }

    private void ControlMovement(bool up, bool down, bool left, bool right) {
        OldPlayerX = playerX;
        OldPlayerZ = playerZ;

        // The walls within the maze are rotated by a 90 degree angel
        if (up) {
            MovePlayerUp();
        } else if (down) {
            MovePlayerDown();
        }
        if (left) {
            MovePlayerLeft();
        } else if (right) {
            MovePlayerRight();
        }

        if (validPosition) {
            Vector3 floorPosition = mazeCells[playerX, playerZ].floor.transform.position;
            cameraTransform.position = new Vector3(floorPosition.x, -1f, floorPosition.z);
            validPosition = false;
        } else {
            playerX = OldPlayerX;
            playerZ = OldPlayerZ;
        }
    }

    private void MovePlayerUp() {
        playerZ = playerZ >= mazeColumns - 1 ? playerZ : playerZ + 1;
        validPosition = mazeCells[OldPlayerX, OldPlayerZ].eastWall == null && mazeCells[playerX, playerZ].westWall == null;
    }

    private void MovePlayerDown() {
        playerZ = playerZ <= 0 ? 0 : playerZ - 1;
        validPosition = mazeCells[OldPlayerX, OldPlayerZ].westWall == null && mazeCells[playerX, playerZ].eastWall == null;
    }

    private void MovePlayerLeft() {
        playerX = playerX <= 0 ? 0 : playerX - 1;
        validPosition = mazeCells[OldPlayerX, OldPlayerZ].northWall == null && mazeCells[playerX, playerZ].southWall == null;
    }

    private void MovePlayerRight() {
        playerX = playerX >= mazeRows - 1 ? playerX : playerX + 1;
        validPosition = mazeCells[OldPlayerX, OldPlayerZ].southWall == null && mazeCells[playerX, playerZ].northWall == null;
    }

	public int GetPlayerX() {
		return playerX;
	}

	public int GetPlayerZ() {
		return playerZ;
	}
}
