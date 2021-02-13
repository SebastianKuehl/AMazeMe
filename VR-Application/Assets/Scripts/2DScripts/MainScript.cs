using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainScript : MonoBehaviour {

	public TextMeshProUGUI textlabel;

    public Tilemap floorTilemap;
    public Tilemap playerMarkerTilemap;
    public Tile player;
    public Tile marker;
	public Tile treasureTile;
    public Tile cornerSouthEast;
    public Tile cornerWestNorth;
    public Tile cornerNorthEast;
    public Tile cornerWestSouth;
    public Tile tCornerNorth;
    public Tile tCornerEast;
    public Tile tCornerSouth;
    public Tile tCornerWest;
    public Tile endWest;
    public Tile endEast;
    public Tile endSouth;
    public Tile endNorth;
    public Tile pathNorthSouth;
    public Tile pathWestEast;
    public Tile start;
    public Tile fourWayCenter;

    private GameObject cameraRig;
	private InputScript inputScript;

	private int mazeSize;
    private int[,] mazeStructure;
    private List<Vector2> playerPositionList;
	private List<TreasureBag> treasurePositionList;
    private Vector2 newPosition, lastPosition;
	private int playerRotation;

	private Transform viveCameraTransform;
	private GameObject cameraObj;

	void Start () {
		cameraObj = GameObject.Find ("Camera (eye)");
		cameraRig = GameObject.Find("[CameraRig]");
		inputScript = cameraRig.GetComponent<InputScript> ();

        playerPositionList = new List<Vector2>();

		newPosition = new Vector2(0, 0);
       
		HandleMazeData();
	}

	void Update () {

		UpdateTextlabel ();

		HandlePlayerMovement();

		UpdateArrowData ();

        UpdateMap();
    }

	private void UpdateTextlabel() {
		int ones = 0;
		for (int i = 0; i < mazeSize; i++) {
			for (int j = 0; j < mazeSize; j++) {
				ones += mazeStructure [i * 3 + 1, j * 3 + 1];
			}
		}
		float percentage = (float) ones / (mazeSize * mazeSize) * 100f;
		textlabel.text = "Discovered: " + (int) percentage + "%";
	}

    private void HandleMazeData() {
		GameObject data = GameObject.Find ("Settings");

		mazeSize = data.GetComponent<DataScript> ().mazeSize;
		mazeStructure = new int[mazeSize * 3, mazeSize * 3];
        mazeStructure[1, 1] = 1;
    }

    private void HandlePlayerMovement() {
        // Add the latest position to the list if not done before
		newPosition.x = inputScript.GetPlayerX();
		newPosition.y = inputScript.GetPlayerZ();; // playerZ because the PC application is in 3D and Y is up/down

        bool pathNeedsUpdate = false;

        if (playerPositionList.Count <= 0) {
            lastPosition = newPosition;
            playerPositionList.Add(newPosition);
            pathNeedsUpdate = true;
        } else {
            lastPosition = playerPositionList[playerPositionList.Count - 1];
            if (lastPosition.x != newPosition.x || lastPosition.y != newPosition.y) {
                playerPositionList.Add(newPosition);
                pathNeedsUpdate = true;
            }
        }

        if (pathNeedsUpdate) {
            // Update the path inbetween the last and current position
            UpdatePath(lastPosition, newPosition);
        }
    }

    private void UpdatePath(Vector2 lastPos, Vector2 newPos) {
		if (lastPos.x >= mazeSize || lastPos.y >= mazeSize || newPos.x >= mazeSize || newPos.y >= mazeSize) {
            Debug.LogError("Values of some positions are larger than existing matrix.");
            return;
        }

        // If positions are unequal then update path depending on direction
        if (lastPos.x != newPos.x || lastPos.y != newPos.y) {
			// Player went north or south
            if (lastPos.x == newPos.x && lastPos.y != newPos.y) {
                int steps = Mathf.Abs ((int) lastPos.y - (int) newPos.y) * 3;
                for(int i = 0; i < steps; i++) {
					int x = (int)lastPos.x * 3 + 1;
					int y = (int)lastPos.y * 3 + (lastPos.y < newPos.y ? 2 + i : -i);
					mazeStructure[x, y] = 1;
                }
            // Player went east or west
            } else if (lastPos.x != newPos.x && lastPos.y == newPos.y) {
                int steps = Mathf.Abs((int)lastPos.x - (int)newPos.x) * 3;
                for (int i = 0; i < steps; i++) {
					int x = (int)lastPos.x * 3 + (lastPos.x < newPos.x ? 2 + i : -i);
					int y = (int)lastPos.y * 3 + 1;
                    mazeStructure[x, y] = 1;
                }
            }
        }
    }

	private void UpdateArrowData () {
		if (viveCameraTransform == null) {
			viveCameraTransform = cameraObj.GetComponent<Transform>();
		} else {
			int newValue = (int) (viveCameraTransform.eulerAngles.y);
			if (newValue % 5 == 0) {
				newValue = 90 - newValue;
				playerRotation = newValue;
			}
		}
	}
		
    private void UpdateMap() {
        // Go through mazeStructure and place needed tile for each point
		for (int i = 0; i < mazeSize; i++) {
			for (int j = 0; j < mazeSize; j++) {
                // Marker- and playermap
				if (i == inputScript.GetPlayerX() && j == inputScript.GetPlayerZ()){
                    playerMarkerTilemap.SetTile(new Vector3Int(i, j, 0), player);

					// Rotate the player arrow according to playerRotation
					Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, playerRotation), Vector3.one);
					playerMarkerTilemap.SetTransformMatrix (new Vector3Int(i, j, 0), matrix);
					playerMarkerTilemap.RefreshTile (new Vector3Int(i, j, 0));
				} else if (i == inputScript.GetMarkerX() && j == inputScript.GetMarkerZ()) {
					playerMarkerTilemap.SetTile(new Vector3Int(i, j, 0), marker);
				} else {
                    playerMarkerTilemap.SetTile(new Vector3Int(i, j, 0), null);
                }

                // Floormap
                if (mazeStructure[i * 3 + 1, j * 3 + 1] == 1) {
                    floorTilemap.SetTile(new Vector3Int(i, j, 0), GetTile(i * 3 + 1, j * 3 + 1));
				} 
            }
        }
    }

    private Tile GetTile(int x, int y) {
        string northVisited = VisitedNorth(x, y) ? "North " : "No ";
        string southVisited = VisitedSouth(x, y) ? "South " : "No ";
        string eastVisited = VisitedEast(x, y) ? "East " : "No ";
        string westVisited = VisitedWest(x, y) ? "West" : "No";
        string result = northVisited + southVisited + eastVisited + westVisited;
        switch(result) {
            // North South East West
            case "North South East West":
                return fourWayCenter;
            case "North South East No":
                return tCornerWest;
            case "North South No West":
                return tCornerEast;
            case "North South No No":
                return pathNorthSouth;
            case "North No East West":
                return tCornerSouth;
            case "North No East No":
                return cornerNorthEast;
            case "North No No West":
                return cornerWestNorth;
            case "North No No No":
                return endSouth;
            case "No South East West":
                return tCornerNorth;
            case "No South East No":
                return cornerSouthEast;
            case "No South No West":
                return cornerWestSouth;
            case "No South No No":
                return endNorth;
            case "No No East West":
                return pathWestEast;
            case "No No East No":
                return endWest;
            case "No No No West":
                return endEast;
            case "No No No No":
                return start;
            default:
                return null;
        }
    }

    private bool VisitedNorth(int x, int y) {
        return mazeStructure[x, y + 1] == 1;
    }

    private bool VisitedSouth(int x, int y) {
        return mazeStructure[x, y - 1] == 1;
    }

    private bool VisitedEast(int x, int y) {
        return mazeStructure[x + 1, y] == 1;
    }

    private bool VisitedWest(int x, int y) {
        return mazeStructure[x - 1, y] == 1;
    }
}
