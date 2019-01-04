using UnityEngine;
using System.Collections;

public class MazeLoader : MonoBehaviour {
	public int mazeRows; // x-Axis
	public int mazeColumns; // z-Axis
    public GameObject wall1;
    public GameObject wall2;
    public GameObject wall3;
    public GameObject wall4;
    public GameObject wall5;
    public GameObject wall6;
    public GameObject wall7;
    public GameObject wall8;
    public GameObject wall9;
    public GameObject wall10;
    public GameObject wall11;
    public GameObject wall12;
	public GameObject floor;
	public GameObject corner;
	public GameObject chest;
	public float size = 2f;

	private MazeCell[,] mazeCells;
	private Vector3 objectScale;
	private float width; // x-Axis
	private float height; // z-Axis
	private int[,] mazeStructure;

	void Start () {
		InitializeMaze ();

		MazeAlgorithm ma = new HuntAndKillMazeAlgorithm (mazeCells);
		ma.CreateMaze ();

		Vector3 chestLocation = mazeCells[mazeRows - 1, mazeColumns - 1].floor.transform.localPosition;
		chest = Instantiate (chest, new Vector3 (chestLocation.x, -1.35f, chestLocation.z), Quaternion.identity) as GameObject;

		// Place the cest
		float tiltAroundY = 0f;
		bool southWall = mazeCells[mazeRows-2, mazeColumns-1].southWallExists;
		bool eastWall = mazeCells[mazeRows - 1, mazeColumns-2].eastWallExists;
		if (!eastWall && !southWall) {
			tiltAroundY = -130f;
		} else if (southWall) {
			tiltAroundY = -180f;
		} else {
			tiltAroundY = -90f;
		}
		Quaternion target = Quaternion.Euler (0, tiltAroundY, 0);
		chest.transform.rotation = target;
	}

	private void InitializeMaze() {
		mazeCells = new MazeCell[mazeRows, mazeColumns];

		// TODO Set position of second camera to center of plane
		for (int r = 0; r < mazeRows; r++) {
			for (int c = 0; c < mazeColumns; c++) {
				mazeCells [r, c] = new MazeCell ();

				mazeCells [r, c] .floor = Instantiate (floor, new Vector3 (r*size, -1.1f, c*size), Quaternion.identity) as GameObject;
				mazeCells [r, c] .floor.name = "Floor " + r + "," + c;
				objectScale = mazeCells [r, c].floor.transform.localScale;
				mazeCells [r, c].floor.transform.localScale = new Vector3 (objectScale.x * size, objectScale.y, objectScale.z * size);

				if (c == 0) {
					mazeCells [r, c].westWall = Instantiate (GetRandomWall(), new Vector3 (r * size, 0, (c * size) - (size / 2f)), Quaternion.identity) as GameObject;
					mazeCells [r, c].westWall.name = "West Wall " + r + "," + c;
					objectScale = mazeCells [r, c].westWall.transform.localScale;
					mazeCells [r, c].westWall.transform.localScale = new Vector3 (objectScale.x * size, objectScale.y, objectScale.z * size);
				}

				mazeCells [r, c].eastWall = Instantiate (GetRandomWall(), new Vector3 (r*size, 0, (c*size) + (size/2f)), Quaternion.identity) as GameObject;
				mazeCells [r, c].eastWall.name = "East Wall " + r + "," + c;
				objectScale = mazeCells [r, c].eastWall.transform.localScale;
				mazeCells [r, c].eastWall.transform.localScale = new Vector3 (objectScale.x * size, objectScale.y, objectScale.z * size);

				if (r == 0) {
					mazeCells [r, c].northWall = Instantiate (GetRandomWall(), new Vector3 ((r*size) - (size/2f), 0, c*size), Quaternion.identity) as GameObject;
					mazeCells [r, c].northWall.name = "North Wall " + r + "," + c;
					mazeCells [r, c].northWall.transform.Rotate (Vector3.up * 90f);
					objectScale = mazeCells [r, c].northWall.transform.localScale;
					mazeCells [r, c].northWall.transform.localScale = new Vector3 (objectScale.x * size, objectScale.y, objectScale.z * size);
				}

				mazeCells[r,c].southWall = Instantiate (GetRandomWall(), new Vector3 ((r*size) + (size/2f), 0, c*size), Quaternion.identity) as GameObject;
				mazeCells [r, c].southWall.name = "South Wall " + r + "," + c;
				mazeCells [r, c].southWall.transform.Rotate (Vector3.up * 90f);
				objectScale = mazeCells [r, c].southWall.transform.localScale;
				mazeCells [r, c].southWall.transform.localScale = new Vector3 (objectScale.x * size, objectScale.y, objectScale.z * size);

			}
		}

		for (int r = 0; r <= mazeRows; r++) {
			for (int c = 0; c <= mazeColumns; c++) {
				GameObject dump = Instantiate (corner, new Vector3 (r*size - size/2f, 0, c*size - size/2f), Quaternion.identity) as GameObject;
				objectScale = dump.transform.localScale;
				dump.transform.localScale = new Vector3 (objectScale.x * size, objectScale.y, objectScale.z * size);
			}
		}
	}

    private GameObject GetRandomWall() {
        float randomNumber = Random.value;
        if (randomNumber < 0.085) {
            return wall1;
        } else if (randomNumber < 0.17) {
            return wall2;
        } else if (randomNumber < 0.255) {
            return wall2;
        } else if (randomNumber < 0.34) {
            return wall2;
        } else if (randomNumber < 0.425) {
            return wall2;
        } else if (randomNumber < 0.51) {
            return wall2;
        } else if (randomNumber < 0.595) {
            return wall2;
        } else if (randomNumber < 0.68) {
            return wall2;
        } else if (randomNumber < 0.765) {
            return wall2;
        } else if (randomNumber < 0.85) {
            return wall2;
        } else if (randomNumber < 0.935) {
            return wall2;
        } else {
            return wall12;
        }
    }

    public int[,] GetMazeStructure() {
        return mazeStructure;
    }

    public MazeCell[,] GetMazeCells() {
        return mazeCells;
    }

    public int GetMazeSize() {
        return (int)size;
    }
}
