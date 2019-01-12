using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class MainScript : MonoBehaviour {

    public Tilemap floorTilemap;
    public Tilemap playerMarkerTilemap;
    public Tile player;
    public Tile marker;
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

    private GameObject gameObj;
    private MazeDataScript mazeDataScript;
    private PlayerPositionScript playerPositionScript;
    private PlayerRotationScript playerRotationScript;
    private MarkerScript markerScript;

    private int mazeRows, mazeColumns;
    private int[,] mazeStructure;
    private List<Vector2> playerPositionList;
    private Vector2 newPosition, lastPosition;
    
    void Start () {
        gameObj = GameObject.Find("GameObject");
        playerPositionList = new List<Vector2>();
        newPosition = new Vector2(0, 0);
	}
	
	void Update () {
        // Handle the maze data before the others in order to generate the main floor first
        if (!HandleMazeData()) {
            return;
        }
        HandlePlayerMovement();

        UpdateMap();
    }

    private bool HandleMazeData() {
        if (!mazeDataScript) {
            mazeDataScript = gameObj.GetComponentInChildren<MazeDataScript>();
            if (!mazeDataScript) {
                return false;
            }
            mazeRows = mazeDataScript.mazeRows;
            mazeColumns = mazeDataScript.mazeColumns;
            mazeStructure = new int[mazeRows * 3, mazeColumns * 3];
            mazeStructure[1, 1] = 1;
        }
        return true;
    }

    private void HandlePlayerMovement() {
        if (!playerPositionScript) {
            playerPositionScript = gameObj.GetComponentInChildren<PlayerPositionScript>();
            if (!playerPositionScript) {
                return;
            }
        }

        // Add the latest position to the list if not done before
        newPosition.x = playerPositionScript.playerX;
        newPosition.y = playerPositionScript.playerZ; // playerZ because the PC application is in 3D and Y is up/down
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
        if (lastPos.x >= mazeRows || lastPos.y >= mazeColumns || newPos.x >= mazeRows || newPos.y >= mazeColumns) {
            Debug.LogError("Values of some positions are larger than existing matrix.");
            return;
        }

        // If positions are unequal then update path depending on direction
        if (lastPos.x != newPos.x || lastPos.y != newPos.y) {
            // Player went north or south
            if (lastPos.x == newPos.x && lastPos.y != newPos.y) {
                int steps = Mathf.Abs ((int) lastPos.y - (int) newPos.y) * 3;
                Debug.Log(steps);
                for(int i = 0; i < steps; i++) {
                    mazeStructure[(int)lastPos.x * 3 + 1, (int)lastPos.y * 3 + (lastPos.y < newPos.y ? 2 + i : -i)] = 1;
                }
            // Player went east or west
            } else if (lastPos.x != newPos.x && lastPos.y == newPos.y) {
                int steps = Mathf.Abs((int)lastPos.x - (int)newPos.x) * 3;
                Debug.Log(steps);
                for (int i = 0; i < steps; i++) {
                    mazeStructure[(int)lastPos.x * 3 + (lastPos.x < newPos.x ? 2 + i : -i), (int)lastPos.y * 3 + 1] = 1;
                }
            }
        }
    }

    private void UpdateMap() {
        if (!markerScript) {
            markerScript = gameObj.GetComponentInChildren<MarkerScript>();
        }

        // Go through mazeStructure and place needed tile for each point
        for (int i = 0; i < mazeRows; i++) {
            for (int j = 0; j < mazeColumns; j++) {
                // Marker- and playermap
                if (markerScript && markerScript.usesMarker && i == markerScript.markerX && j == markerScript.markerZ) {
                    playerMarkerTilemap.SetTile(new Vector3Int(i, j, 0), marker);
                } else if (i == newPosition.x && j == newPosition.y){
                    playerMarkerTilemap.SetTile(new Vector3Int(i, j, 0), player);
                } else {
                    playerMarkerTilemap.SetTile(new Vector3Int(i, j, 0), null);
                }
                // Floormap
                if (mazeStructure[i * 3 + 1, j * 3 + 1] == 1) {
                    floorTilemap.SetTile(new Vector3Int(i, j, 0), GetTile(i * 3 + 1, j * 3 + 1));
                } else {
                    floorTilemap.SetTile(new Vector3Int(i, j, 0), null);
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
