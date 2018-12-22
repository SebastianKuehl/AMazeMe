using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class MainScript : MonoBehaviour {

    public Tilemap tilemap;
	public Tile wall;
	public Tile floor;
	public Tile possiblePosition;

	public Tilemap lineOfSightTilemap;
	public Tile north;
	public Tile south;
	public Tile east;
	public Tile west;
	public Tile northeast;
	public Tile northwest;
	public Tile southeast;
	public Tile southwest;

    private GameObject gameObj;
    private MazeDataScript mazeDataScript;
    private PlayerPositionScript playerPositionScript;
    private PlayerRotationScript playerRotationScript;

    private int mazeRows, mazeColumns;
    private int[,] mazeStructure;
    private List<Vector2> playerPositionList;
    private Vector2 newPosition, lastPosition;

	private Tile lastSightDirectionTile;

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
        HandlePlayerRotation();

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
            for (int i = 0; i < mazeRows * 3; i++) {
                for (int j = 0; j < mazeColumns * 3; j++) {
					tilemap.SetTile(new Vector3Int(i, j, 0), wall);
                }
            }
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
        if (playerPositionList.Count <= 0) {
            lastPosition = newPosition;
            playerPositionList.Add(newPosition); // TODO Check that its not referenced and the values are kept
        } else {
            lastPosition = playerPositionList[playerPositionList.Count - 1];
            if (lastPosition.x != newPosition.x || lastPosition.y != newPosition.y) {
                playerPositionList.Add(newPosition);
            }
        }

        // Update the path inbetween the last and current position
        UpdatePath(lastPosition, newPosition);
    }

    private void UpdatePath(Vector2 lastPos, Vector2 newPos) {
        if (lastPos.x >= mazeRows || lastPos.y >= mazeColumns || newPos.x >= mazeRows || newPos.y >= mazeColumns) {
            Debug.LogError("Values of some positions are larger than existing matrix.");
            return;
        }
        // Make new position show in map
        mazeStructure[(int)newPos.x * 3 + 1, (int)newPos.y * 3 + 1] = 1;

        // If positions are unequal then update path depending on direction
        if (lastPos.x != newPos.x || lastPos.y != newPos.y) {
            // Player went north
            if (lastPos.x == newPos.x && lastPos.y < newPos.y) {
                mazeStructure[(int)lastPos.x * 3 + 1, (int)lastPos.y * 3 + 2] = 1;
                mazeStructure[(int)lastPos.x * 3 + 1, (int)lastPos.y * 3 + 3] = 1;
            }
            // Player went south
            if (lastPos.x == newPos.x && lastPos.y > newPos.y) {
                mazeStructure[(int)lastPos.x * 3 + 1, (int)lastPos.y * 3] = 1;
                mazeStructure[(int)lastPos.x * 3 + 1, (int)lastPos.y * 3 - 1] = 1;
            }
            // Player went east
            if (lastPos.x < newPos.x && lastPos.y == newPos.y) {
                mazeStructure[(int)lastPos.x * 3 + 2, (int)lastPos.y * 3 + 1] = 1;
                mazeStructure[(int)lastPos.x * 3 + 3, (int)lastPos.y * 3 + 1] = 1;
            }
            // Player went west
            if (lastPos.x > newPos.x && lastPos.y == newPos.y) {
                mazeStructure[(int)lastPos.x * 3, (int)lastPos.y * 3 + 1] = 1;
                mazeStructure[(int)lastPos.x * 3 - 1, (int)lastPos.y * 3 + 1] = 1;
            }
        }
    }

    private void HandlePlayerRotation() {
        if (!playerRotationScript) {
            playerRotationScript = gameObj.GetComponentInChildren<PlayerRotationScript>();
        }

    }

    private void UpdateMap() {
        // Go through mazeStructure and place needed tile for each point
		Tile whiteTile = new Tile();
		whiteTile.color = Color.white;

        for (int i = 1; i < mazeRows * 3 - 1; i++) {
            for (int j = 1; j < mazeColumns * 3 - 1; j++) {
				if ((i - 1) % 3 == 0 && (j - 1) % 3 == 0) {
					tilemap.SetTile (new Vector3Int (i, j, 0), possiblePosition);
				} else if (mazeStructure[i, j] == 1) {
					tilemap.SetTile (new Vector3Int (i, j, 0),floor);
                }
            }
        }
		lineOfSightTilemap.SetTile (new Vector3Int((int)lastPosition.x * 3 + 1, (int)lastPosition.y * 3 + 1, 0), null);
		lineOfSightTilemap.SetTile (new Vector3Int((int)newPosition.x * 3 + 1, (int)newPosition.y * 3 + 1, 0), GetLineOfSightTile());
    }
	/*
    private Tile GetTile(int x, int y) {
        string northVisited = VisitedNorth(x, y) ? "North " : "No ";
        string southVisited = VisitedSouth(x, y) ? "South " : "No ";
        string eastVisited = VisitedEast(x, y) ? "East " : "No ";
        string westVisited = VisitedWest(x, y) ? "West" : "No";
        string result = northVisited + southVisited + eastVisited + westVisited;
        switch(result) {
            case "North South East West":
                return allPillarTile;
            case "North South East No":
                return tCornerWestTile;
            case "North South No West":
                return tCornerEastTile;
            case "North South No No":
                return twoWallSouthNorthTile;
            case "North No East West":
                return tCornerSouthTile;
            case "North No East No":
                return cornerNorthEastTile;
            case "North No No West":
                return cornerWestNorthTile;
            case "North No No No":
                return threeWallNorthTile;
            case "No South East West":
                return tCornerNorthTile;
            case "No South East No":
                return cornerSouthEastTile;
            case "No South No West":
                return cornerWestSouthTile;
            case "No South No No":
                return threeWallSouthTile;
            case "No No East West":
                return twoWallWestEastTile;
            case "No No East No":
                return threeWallEastTile;
            case "No No No West":
                return threeWallWestTile;
            case "No No No No":
                return allWallTile;
            default:
                return allWallTile;
        }
    }
*/
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

	private Tile GetLineOfSightTile() {
		int direction = playerRotationScript.playerYRotation;
		if (direction >= -10 && direction <= 10) {
			lastSightDirectionTile = north;
			return north;
		}
		if (direction >= 55 && direction <= 75) {
			lastSightDirectionTile = east;
			return east;
		}
		if (direction < -10 && direction >= -80) {
			lastSightDirectionTile = west;
			return west;
		}
		if (direction >= 85 && direction <= 99) {
			lastSightDirectionTile = south;
			return south;
		}

		return lastSightDirectionTile;
	}
}
