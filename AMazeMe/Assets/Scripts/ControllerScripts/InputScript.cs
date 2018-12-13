using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputScript : MonoBehaviour {

    public bool inputByKeyboard = true;
    public bool PressForMovement = false;
    public bool Teleport = true;
    public bool Running = false;
    public List<Vector2> playerPositionList;
    public int playerX, playerZ, OldPlayerX, OldPlayerZ, mazeRows, mazeColumns;

    private GameObject rightController;
    private ViveControllerScript rightControllerScript;
    private Transform cameraTransform;
    private List<GameObject> walls;
    private MazeLoader MazeLoaderScript;
    private Vector2 wallPosition;
    private Vector3 playerPosition;
    private bool ButtonHasBeenPressed, validPosition;
    private MazeCell[,] mazeCells;

    void Start() {
        rightController = GameObject.Find("Controller (right)");
        playerPositionList = new List<Vector2>();
        walls = new List<GameObject>();
        foreach (GameObject gameObject in GameObject.FindObjectsOfType(typeof(GameObject))) {
            if (gameObject.name.Contains("Wall")) {
                walls.Add(gameObject);
            }
        }
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
                mazeCells = MazeLoaderScript.mazeCells;
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
            if (rightControllerScript.TouchpadTouched() && (PressForMovement ? rightControllerScript.TouchpadPressed() : true)) {
                if (Teleport) {
                    TeleportMovement();
                } else if (Running) {
                    RunningMovement();
                }
            }
        }
    }

    private void TeleportMovement() {
        if (rightControllerScript.TouchpadPressed()) {
            ControlMovement(rightControllerScript.TouchpadTouchUp(),
                rightControllerScript.TouchpadTouchDown(),
                rightControllerScript.TouchpadTouchLeft(),
                rightControllerScript.TouchpadTouchRight());
        }
    }

    private void ControlMovement(bool up, bool down, bool left, bool right) {
        // Debug.Log(up + " " + down + " " + left + " " + right);
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
            playerPositionList.Add(new Vector2(floorPosition.x, floorPosition.z));
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

    private void RunningMovement() {
        // Neue Position berechnen
        Vector3 newPlayerPosition = cameraTransform.position + cameraTransform.right * Time.deltaTime * rightControllerScript.TouchPosition().x;
        newPlayerPosition = (newPlayerPosition + transform.right * Time.deltaTime * rightControllerScript.TouchPosition().x) + transform.forward * Time.deltaTime * 3 * rightControllerScript.TouchPosition().y;

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
            cameraTransform.position = newPlayerPosition;
            playerPositionList.Add(new Vector2(newPlayerPosition.x, newPlayerPosition.z));
        }
    }

    private bool checkForCollision(Vector3 wallPosition, Vector3 wallScale, Vector3 newPlayerPosition) {
        // Debug.Log(wallPosition.x +" "+ newPlayerPosition.x+" "+ (wallPosition.x + wallScale.x));
        return (wallPosition.x <= newPlayerPosition.x && newPlayerPosition.x <= wallPosition.x + wallScale.x)
            && (wallPosition.z <= newPlayerPosition.z && newPlayerPosition.z <= wallPosition.z + wallScale.z);
    }



}
