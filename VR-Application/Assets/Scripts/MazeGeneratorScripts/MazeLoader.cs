using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MazeLoader : MonoBehaviour {
    private const float SIZE = 3f;

    public GameObject wall;
	public GameObject floor;
	public GameObject corner;
	public GameObject ceiling;
    public GameObject door;

    private int mazeSize;
    private MazeCell[,] mazeCells;

    void Awake () {
        GetNeededData();

        for (int r = 0; r < mazeSize; r++) {
            for (int c = 0; c < mazeSize; c++) {
                PlaceFloorAndWallForCell(r, c);
            }
        }

        for (int r = 0; r <= mazeSize; r++) {
            for (int c = 0; c <= mazeSize; c++) {
                PlaceCorner(r, c);
            }
        }

        PlaceDoorForLeavingMaze();

        new HuntAndKillMazeAlgorithm (mazeCells).CreateMaze();
	}

    private void GetNeededData() {
        GameObject data = GameObject.Find("Settings");
        mazeSize = data.GetComponent<DataScript>().mazeSize;
        mazeCells = new MazeCell[mazeSize, mazeSize];
    }
    
	private void PlaceFloorAndWallForCell(int r, int c) {
		mazeCells [r, c] = new MazeCell() {
            floor = Instantiate(floor, new Vector3(r * SIZE, 0f, c * SIZE), Quaternion.identity) as GameObject,
            ceiling = Instantiate(ceiling, new Vector3(r * SIZE, SIZE, c * SIZE), Quaternion.identity) as GameObject,
            westWall = c == 0  ? Instantiate(wall, new Vector3(r * SIZE, 0f, (c * SIZE) - (SIZE / 2f)), Quaternion.identity) as GameObject : null,
            eastWall = Instantiate(wall, new Vector3(r * SIZE, 0f, (c * SIZE) + (SIZE / 2f)), Quaternion.identity) as GameObject,
            northWall = r == 0 ? Instantiate(wall, new Vector3((r * SIZE) - (SIZE / 2f), 0f, c * SIZE), Quaternion.identity) as GameObject : null,
            southWall = Instantiate(wall, new Vector3((r * SIZE) + (SIZE / 2f), 0f, c * SIZE), Quaternion.identity) as GameObject
        };
                
        Vector3 sizeVector = new Vector3(SIZE / 2f, SIZE / 2f, SIZE / 2f);
        SetParamsForObj(mazeCells[r, c].floor, "Floor " + r + "," + c, false, new Vector3(SIZE, 1f, SIZE));
        SetParamsForObj(mazeCells[r, c].ceiling, "Ceiling " + r + "," + c, false, new Vector3(SIZE / 2f, 1f, SIZE / 2f));
        SetParamsForObj(mazeCells[r, c].westWall, "West Wall " + r + "," + c, false, sizeVector);
        SetParamsForObj(mazeCells[r, c].eastWall, "East Wall " + r + "," + c, false, sizeVector);
        SetParamsForObj(mazeCells[r, c].northWall, "North Wall " + r + "," + c, true, sizeVector);
        SetParamsForObj(mazeCells[r, c].southWall, "South Wall " + r + "," + c, true, sizeVector);
    }

    private void PlaceCorner(int r, int c) {
        GameObject cornerObj = Instantiate(corner, new Vector3(r * SIZE - SIZE / 2f, 0f, c * SIZE - SIZE / 2f), Quaternion.identity) as GameObject;
        SetParamsForObj(cornerObj, "Corner " + r + "," + c, false, new Vector3(SIZE / 2f, SIZE / 2f, SIZE / 2f));
    }

    private void PlaceDoorForLeavingMaze() {
        Destroy(mazeCells[mazeSize - 1, mazeSize - 1].southWall);
        GameObject doorObj = Instantiate(door, new Vector3(((mazeSize - 1) * SIZE) + (SIZE / 2f), 0f, (mazeSize - 1) * SIZE), Quaternion.identity) as GameObject;
        SetParamsForObj(doorObj, "Door", true, new Vector3(SIZE / 2f - 0.1f, 1f, SIZE / 2f));
    }

    private void SetParamsForObj(GameObject obj, string name, bool rotate, Vector3 factor) {
        if (!obj) {
            return;
        }

        obj.name = name;
        if (rotate) {
            obj.transform.Rotate(Vector3.up * 90f);
        }
        Vector3 objScale = obj.transform.localScale;
        obj.transform.localScale = Vector3.Scale(objScale, factor);
    }

    public bool NorthWallExists(int x, int z) {
        return mazeCells[x, z].northWallExists;
    }

    public bool SouthWallExists(int x, int z) {
        return mazeCells[x, z].southWall;
    }

    public bool EastWallExists(int x, int z) {
        return mazeCells[x, z].eastWallExists;
    }

    public bool WestWallExists(int x, int z) {
        return mazeCells[x, z].westWallExists;
    }

    public Vector3 GetFloorPosition(int x, int z) {
        return mazeCells[x, z].floor.transform.position;
    }

	public MazeCell[,] GetMazeCells() {
		return mazeCells;
	}
}
