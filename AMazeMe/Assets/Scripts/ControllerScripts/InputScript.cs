﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputScript : MonoBehaviour {

    public bool inputByKeyboard = false;
    public bool useMarker = true;
    public bool cheatMode = false;

    private GameObject rightController;
    private ViveControllerScript rightControllerScript;
    private Transform cameraTransform;
    private MazeLoader MazeLoaderScript;
    private Vector2 wallPosition;
    private Vector3 playerPosition;
    private bool validPosition;
    private MazeCell[,] mazeCells;
    private int[,] mazeStructure;
    private int playerX, playerZ, OldPlayerX, OldPlayerZ, mazeRows, mazeColumns, mazeSize;
    private int markerX, markerZ, oldMarkerX, oldMarkerZ;

    void Start() {
        rightController = GameObject.Find("Controller (right)");
    }

    void FixedUpdate() {
        // Load all the needed data if not done before
        if (MazeLoaderScriptLoaded()) {
            // Handle the chosen method of input
            if (inputByKeyboard) {
                HandleKeyboardInput();
            } else {
                HandleControllerInput();
            }
        }
    }

    private bool MazeLoaderScriptLoaded() {
        if (MazeLoaderScript == null) {
            GameObject cameraRig = GameObject.Find("[CameraRig]");
            if (cameraRig != null) {
                cameraTransform = cameraRig.GetComponent<Transform>();
                MazeLoaderScript = cameraRig.GetComponent<MazeLoader>();
                mazeRows = MazeLoaderScript.mazeRows;
                mazeColumns = MazeLoaderScript.mazeColumns;
                mazeCells = MazeLoaderScript.GetMazeCells();
                mazeStructure = MazeLoaderScript.GetMazeStructure();
                mazeSize = MazeLoaderScript.GetMazeSize();
                return true;
            }
            return false;
        }
        return true;
    }

    private void HandleKeyboardInput() {
        ControlMovement(Input.GetKeyDown("w"), Input.GetKeyDown("s"), Input.GetKeyDown("a"), Input.GetKeyDown("d"));
		ApplyChanges (Input.GetKeyDown(KeyCode.Space));
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
			if (rightControllerScript.TriggerDown()) {
				Debug.Log ("Trigger");
			}
			ApplyChanges (rightControllerScript.TriggerDown());
        }
    }

	private void ApplyChanges(bool confirm) {
		if (useMarker) {
			if (confirm && (markerX != playerX || markerZ != playerZ)) {
				// Move Player towars marker
				TeleportInStraightLine ();
			}
		} else {
			RefreshPlayerPosition();
		}
	}

	private void ControlMovement(bool up, bool down, bool left, bool right) {
        OldPlayerX = playerX;
        OldPlayerZ = playerZ;
        oldMarkerX = markerX;
        oldMarkerZ = markerZ;
        if (useMarker) {
            // Move Marker towards direction
            if (up) {
                markerZ = markerZ >= mazeColumns - 1 ? markerZ : markerZ + 1;
            } else if (down) {
                markerZ = markerZ <= 0 ? 0 : markerZ - 1;
            }
            if (left) {
                markerX = markerX <= 0 ? 0 : markerX - 1;
            } else if (right) {
                markerX = markerX >= mazeRows - 1 ? markerX : markerX + 1;
            }
        } else {
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
        }
    }

    private void MovePlayerUp() {
        playerZ = playerZ >= mazeColumns - 1 ? playerZ : playerZ + 1;
        validPosition = !mazeCells[OldPlayerX, OldPlayerZ].eastWallExists && !mazeCells[playerX, playerZ].westWallExists;
    }

    private void MovePlayerDown() {
        playerZ = playerZ <= 0 ? 0 : playerZ - 1;
        validPosition = !mazeCells[OldPlayerX, OldPlayerZ].westWallExists && !mazeCells[playerX, playerZ].eastWallExists;
    }

    private void MovePlayerLeft() {
        playerX = playerX <= 0 ? 0 : playerX - 1;
        validPosition = !mazeCells[OldPlayerX, OldPlayerZ].northWallExists && !mazeCells[playerX, playerZ].southWallExists;
    }

    private void MovePlayerRight() {
        playerX = playerX >= mazeRows - 1 ? playerX : playerX + 1;
        validPosition = !mazeCells[OldPlayerX, OldPlayerZ].southWallExists && !mazeCells[playerX, playerZ].northWallExists;
    }

    private void RefreshPlayerPosition() {
        if (validPosition || cheatMode) {
            Vector3 floorPosition = mazeCells[playerX, playerZ].floor.transform.position;
            cameraTransform.position = new Vector3(floorPosition.x, -1f, floorPosition.z);
            validPosition = false;
        } else {
            playerX = OldPlayerX;
            playerZ = OldPlayerZ;
        }
    }

    private void TeleportInStraightLine() {
        // Both axis are not on line
        if (markerX != playerX && markerZ != playerZ) {
            return;
        }

        if (markerX == playerX) {
            // Teleport on Y-Axis
            int steps = Mathf.Abs(markerZ - playerZ);
            for (int i = 0; i < steps; i++) {
                if (markerZ > playerZ) {
                    MovePlayerUp();
                } else {
                    MovePlayerDown();
                }
                RefreshPlayerPosition();
            }
        } else {
            // Teleport on X-Axis
            int steps = Mathf.Abs(markerX - playerX);
            for (int i = 0; i < steps; i++) {
                if (markerX > playerX) {
                    MovePlayerRight();
                } else {
                    MovePlayerLeft();
                }
                RefreshPlayerPosition();
            }
        }
    }

    public int GetPlayerX() {
		return playerX;
	}

	public int GetPlayerZ() {
		return playerZ;
	}

    public int GetMarkerX() {
        return markerX;
    }

    public int GetMarkerZ() {
        return markerZ;
    }

    public bool UsesMarker() {
        return useMarker;
    }
}
