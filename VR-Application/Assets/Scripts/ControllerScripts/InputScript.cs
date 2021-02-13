using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputScript : MonoBehaviour {
	private DataScript data;
    private Transform cameraTransform;
    private MazeLoader mazeStructure;
	private bool validPosition;
    private int playerX, playerZ, OldPlayerX, OldPlayerZ, mazeSize;
    private int markerX, markerZ;

    private void Awake() {
        GameObject obj = GameObject.Find("Settings");
        data = obj.GetComponent<DataScript>();
        mazeSize = data.mazeSize;
    }

    void Update() {
        if (!MazeLoaderScriptLoaded()) {
			return;
        }
       
        HandleKeyboardInput();

        // If the player stands on the chest in the maze he gets teleported to the treasure room
		if (playerX == mazeSize - 1 && playerZ == mazeSize - 1 && data.foundKey) {
			SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    private bool MazeLoaderScriptLoaded() {
        if (!mazeStructure) {
            GameObject cameraRig = GameObject.Find("[CameraRig]");
            if (cameraRig) {
                cameraTransform = cameraRig.GetComponent<Transform>();
                mazeStructure = cameraRig.GetComponent<MazeLoader>();
                return true;
            } else {
                return false;
            }
        }
        return true;
    }

    private void HandleKeyboardInput() {
		ControlMovement(Input.GetKeyDown(KeyCode.W), Input.GetKeyDown(KeyCode.S), Input.GetKeyDown(KeyCode.A), Input.GetKeyDown(KeyCode.D));
		ApplyChanges (Input.GetKeyDown(KeyCode.Space));
    }

	private void ControlMovement(bool up, bool down, bool left, bool right) {
        OldPlayerX = playerX;
        OldPlayerZ = playerZ;
        if (up) {
			markerZ = markerZ >= mazeSize - 1 ? markerZ : markerZ + 1;
        } else if (down) {
            markerZ = markerZ <= 0 ? 0 : markerZ - 1;
        }
        if (left) {
            markerX = markerX <= 0 ? 0 : markerX - 1;
        } else if (right) {
			markerX = markerX >= mazeSize - 1 ? markerX : markerX + 1;
        }
    }

    private void ApplyChanges(bool confirm) {
        if (confirm && (markerX != playerX || markerZ != playerZ)) {
            TeleportInStraightLine();
        }
    }

    private void TeleportInStraightLine() {
        if (markerX != playerX && markerZ != playerZ) {
            return;
        }

        bool teleportValid = true;
        if (markerX == playerX) {
            teleportValid = TeleportOn_Z_Axis();
        } else {
            teleportValid = TeleportOn_X_Axis();
        }
        if (teleportValid) {
            playerX = markerX;
            playerZ = markerZ;
            validPosition = true;
            RefreshPlayerPosition();
        }
    }

    private bool TeleportOn_Z_Axis() {
        int newPosition, oldPosition;
        int steps = Mathf.Abs(markerZ - playerZ);
        for (int i = 0; i<steps; i++) {
            bool movePlayerUp = markerZ > playerZ;
            if (movePlayerUp) {
				newPosition = playerZ + 1 + i >= mazeSize - 1 ? mazeSize - 1 : playerZ + 1 + i;
            } else {
                newPosition = playerZ - 1 - i< 0 ? 0 : playerZ - 1 - i;
            }
            oldPosition = movePlayerUp? newPosition - 1 : newPosition + 1;
            bool eastWallExists = mazeStructure.EastWallExists(playerX, movePlayerUp ? oldPosition : newPosition);
            bool westWallExists = mazeStructure.WestWallExists(playerX, movePlayerUp ? newPosition : oldPosition);
            if (eastWallExists || westWallExists ) {
                return false;
            }
        }
        return true;
    }

    private bool TeleportOn_X_Axis() {
        int newPosition, oldPosition;
        int steps =  Mathf.Abs(markerX - playerX);
        for (int i = 0; i < steps; i++) {
            bool movePlayerRight = markerX > playerX;
            if (movePlayerRight) {
                newPosition = playerX + 1 + i >= mazeSize - 1 ? mazeSize - 1 : playerX + 1 + i;
            } else {
                newPosition = playerX - 1 - i < 0 ? 0 : playerX - 1 - i;
            }
            oldPosition = movePlayerRight ? newPosition - 1 : newPosition + 1;
            bool northWallExists = mazeStructure.NorthWallExists(movePlayerRight ? newPosition : oldPosition, OldPlayerZ);
            bool southWallExists = mazeStructure.SouthWallExists(movePlayerRight ? oldPosition : newPosition, playerZ);
            if (northWallExists || southWallExists) {
                return false;
            }
        }
        return true;
    }

    private void RefreshPlayerPosition() {
        if (validPosition) {
            Vector3 floorPosition = mazeStructure.GetFloorPosition(playerX, playerZ);
            cameraTransform.position = new Vector3(floorPosition.x, 0f, floorPosition.z);
            validPosition = false;
        } else {
            playerX = OldPlayerX;
            playerZ = OldPlayerZ;
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
}
