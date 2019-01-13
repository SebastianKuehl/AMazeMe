using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputScript : MonoBehaviour {
    
    public Transform treasureroomPosition;
    public bool inputByKeyboard = false;
    public bool useMarker = true;
    public bool cheatMode = false;

    private bool wonTheGame = false;

    private GameObject rightController;
    private ViveControllerScript rightControllerScript;
    private Transform cameraTransform;
    private MazeLoader MazeLoaderScript;
    private bool validPosition;
    private MazeCell[,] mazeCells;
    private List<TreasureBag> bagList;
    private GameObject[] lootObjects;
    private int lootcounter;
    private int playerX, playerZ, OldPlayerX, OldPlayerZ, mazeRows, mazeColumns, mazeSize;
    private int markerX, markerZ, oldMarkerX, oldMarkerZ;

    void Awake() {
        rightController = GameObject.Find("Controller (right)");
        lootObjects = GameObject.FindGameObjectsWithTag("Loot");
        foreach (GameObject obj in lootObjects) {
            Component[] items = obj.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in items) {
                renderer.enabled = false;
            }
        }
    }

    void FixedUpdate() {
        
        if (wonTheGame || !MazeLoaderScriptLoaded()) {
            return;
        }

        // Handle the chosen method of input
        if (inputByKeyboard) {
            HandleKeyboardInput();
        } else {
            HandleControllerInput();
        }

        // Collect breadcrumbs if player stands on one
        Vector2 currentPos = new Vector2(playerX, playerZ);
        foreach (TreasureBag container in bagList) {
            if (container.position != currentPos) {
                continue;
            }
            lootcounter++;
            Component[] items = lootObjects[lootcounter].GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in items) {
                renderer.enabled = true;
            }

            items = container.bag.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in items) {
                renderer.enabled = false;
            }
            bagList.Remove(container);
            break;
        }

        // If the player stands on the chest in the maze he gets teleported to the treasure room
        if (playerX == mazeRows - 1 && playerZ == mazeColumns - 1) {
            wonTheGame = true;
            Vector3 targetPos = treasureroomPosition.transform.position;
            cameraTransform.position = new Vector3(targetPos.x, cameraTransform.position.y, targetPos.z);
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
                bagList = MazeLoaderScript.GetBags();
                mazeSize = MazeLoaderScript.GetMazeSize();
                return true;
            }
            return false;
        }
        return true;
    }

    private void HandleKeyboardInput() {
        ControlMovement(Input.GetKeyUp("w"), Input.GetKeyUp("s"), Input.GetKeyUp("a"), Input.GetKeyUp("d"));
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

    private void ApplyChanges(bool confirm) {
        if (useMarker) {
            if (confirm && (markerX != playerX || markerZ != playerZ)) {
                // Move Player towars marker
                TeleportInStraightLine();
            }
        } else {
            RefreshPlayerPosition();
        }
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
        bool teleport = true;
        int newPosition, oldPosition;
        if (markerX == playerX) {
            // Teleport on Y-Axis
            int steps = Mathf.Abs(markerZ - playerZ);
            for (int i = 0; i < steps; i++) {
                bool movePlayerUp = markerZ > playerZ;
                if (movePlayerUp) {
                    newPosition = playerZ + 1 + i >= mazeColumns - 1 ? mazeColumns - 1 : playerZ + 1 + i;
                } else {
                    newPosition = playerZ - 1 - i < 0 ? 0 : playerZ - 1 - i;
                }
                oldPosition = movePlayerUp ? newPosition - 1 : newPosition + 1;
                if (mazeCells[playerX, movePlayerUp ? oldPosition : newPosition].eastWallExists || mazeCells[playerX, movePlayerUp ? newPosition : oldPosition].westWallExists) {
                    teleport = false;
                    break;
                }
            }
        } else {
            // Teleport on X-Axis
            int steps = Mathf.Abs(markerX - playerX);
            for (int i = 0; i < steps; i++) {
                bool movePlayerRight = markerX > playerX;
                if (movePlayerRight) {
                    newPosition = playerX + 1 + i >= mazeRows - 1 ? mazeRows - 1 : playerX + 1 + i;
                } else {
                    newPosition = playerX - 1 - i < 0 ? 0 : playerX - 1 - i;
                }
                oldPosition = movePlayerRight ? newPosition - 1 : newPosition + 1;
                if (mazeCells[movePlayerRight ? newPosition : oldPosition, OldPlayerZ].northWallExists || mazeCells[movePlayerRight ? oldPosition : newPosition, playerZ].southWallExists) {
                    teleport = false;
                    break;
                }
            }
        }
        if (teleport) {
            playerX = markerX;
            playerZ = markerZ;
            validPosition = true;
            RefreshPlayerPosition();
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

    public bool WonTheGame() {
        return wonTheGame;
    }
}
