using UnityEngine;
using System.Collections;

public class MazeLoader : MonoBehaviour {
	public int mazeRows; // x-Axis
	public int mazeColumns; // z-Axis
	public GameObject wall;
	public float size = 2f;

	private MazeCell[,] mazeCells;
	private float width; // x-Axis
	private float height; // z-Axis
	private int[,] mazeStructure;

	void Start () {
		InitializeMaze ();

		MazeAlgorithm ma = new HuntAndKillMazeAlgorithm (mazeCells);
		ma.CreateMaze ();

		CopyMazeToIntArray ();
	}

	private void InitializeMaze() {

		mazeCells = new MazeCell[mazeRows, mazeColumns];

		// TODO Set position of second camera to center of plane

		for (int r = 0; r < mazeRows; r++) {
			for (int c = 0; c < mazeColumns; c++) {
				mazeCells [r, c] = new MazeCell ();

				// For now, use the same wall object for the floor!
				mazeCells [r, c] .floor = Instantiate (wall, new Vector3 (r*size, -1.5f, c*size), Quaternion.identity) as GameObject;
				mazeCells [r, c] .floor.name = "Floor " + r + "," + c;
				mazeCells [r, c] .floor.transform.Rotate (Vector3.right, 90f);
				mazeCells [r, c].floor.transform.localScale = new Vector3 (mazeCells [r, c] .floor.transform.localScale.x, mazeCells [r, c] .floor.transform.localScale.x, mazeCells [r, c] .floor.transform.localScale.z);

				if (c == 0) {
					mazeCells [r, c].westWall = Instantiate (wall, new Vector3 (r * size, 0, (c * size) - (size / 2f)), Quaternion.identity) as GameObject;
					mazeCells [r, c].westWall.name = "West Wall " + r + "," + c;
				}

				mazeCells [r, c].eastWall = Instantiate (wall, new Vector3 (r*size, 0, (c*size) + (size/2f)), Quaternion.identity) as GameObject;
				mazeCells [r, c].eastWall.name = "East Wall " + r + "," + c;

				if (r == 0) {
					mazeCells [r, c].northWall = Instantiate (wall, new Vector3 ((r*size) - (size/2f), 0, c*size), Quaternion.identity) as GameObject;
					mazeCells [r, c].northWall.name = "North Wall " + r + "," + c;
					mazeCells [r, c].northWall.transform.Rotate (Vector3.up * 90f);
				}

				mazeCells[r,c].southWall = Instantiate (wall, new Vector3 ((r*size) + (size/2f), 0, c*size), Quaternion.identity) as GameObject;
				mazeCells [r, c].southWall.name = "South Wall " + r + "," + c;
				mazeCells [r, c].southWall.transform.Rotate (Vector3.up * 90f);
			}
		}
	}

	private void CopyMazeToIntArray() {
		mazeStructure = new int[mazeRows * 3, mazeColumns * 3];

		int curRow, curCol;

		for (int row = 0; row < mazeRows; row++) {
			for (int column = 0; column < mazeColumns; column++) {
				curRow = row * 3 + 1;
				curCol = column * 3 + 1;

				// Center of 3x3 is always walkable
				if ((curRow - 1) % 3 == 0 && (curCol - 1) % 3 == 0) {
					mazeStructure[curRow, curCol] = 2;
					// The corners of the 3x3 are never walkable
					mazeStructure[curRow - 1, curCol - 1] = 1;
					mazeStructure[curRow - 1, curCol + 1] = 1;
					mazeStructure[curRow + 1, curCol - 1] = 1;
					mazeStructure[curRow + 1, curCol + 1] = 1;
				}

				// Check all walls
				if (mazeCells[row, column].northWallExists) {
					mazeStructure [curRow - 1, curCol - 1] = 1;
					mazeStructure [curRow - 1, curCol] = 1;
					mazeStructure [curRow - 1, curCol + 1] = 1;

				}
				if (mazeCells[row, column].southWallExists) {
					mazeStructure [curRow + 1, curCol - 1] = 1;
					mazeStructure [curRow + 1, curCol] = 1;
					mazeStructure [curRow + 1, curCol + 1] = 1;
				
				}
				if (mazeCells[row, column].westWallExists) {
					mazeStructure [curRow - 1, curCol - 1] = 1;
					mazeStructure [curRow, curCol - 1] = 1;
					mazeStructure [curRow + 1, curCol - 1] = 1;
				
				}
				if (mazeCells[row, column].eastWallExists) {
					mazeStructure [curRow - 1, curCol + 1] = 1;
					mazeStructure [curRow, curCol + 1] = 1;
					mazeStructure [curRow + 1, curCol + 1] = 1;
				}
			}
		}

		// Turn array 90 degree left due to rotation of walls (lowkey annoying)
		int[,] placeholder = new int[mazeRows * 3, mazeColumns * 3];
		for (int row = 0; row < mazeRows * 3; row++) {
			for (int column = 0; column < mazeColumns * 3; column++) {
				placeholder [mazeRows * 3 - 1 - column, row] = mazeStructure [row, column];
			}
		}
		mazeStructure = placeholder;

		// For debugging purpose
		/*
		string output = "";
		for (int row = 0; row < mazeRows * 3; row++) {
			output = "";
			for (int column = 0; column < mazeColumns * 3; column++) {
				output += (mazeStructure [row, column] + " ");
			}
			print (output);
		}
		*/
	}

	public int[,] GetMazeStructure() {
		return mazeStructure;
	}

	public MazeCell[,] GetMazeCells() {
		return mazeCells;
	}
}
