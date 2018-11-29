using UnityEngine;
using System.Collections;

public class MazeLoader : MonoBehaviour {
	public int mazeRows; // x-Axis
	public int mazeColumns; // z-Axis
	public GameObject wall;
	public GameObject roof;
	public Transform secondCamera;
	public float size = 2f;

	private MazeCell[,] mazeCells;
	private GameObject plane;
	private float width; // x-Axis
	private float height; // z-Axis

	void Start () {
		InitializeMaze ();

		MazeAlgorithm ma = new HuntAndKillMazeAlgorithm (mazeCells);
		ma.CreateMaze ();

		// Generate and place plane for map
		CreateAndPlacePlane ();

		//Set position of second camera above center of plane
		SetSecondCameraPosition();
	}

	private void InitializeMaze() {

		mazeCells = new MazeCell[mazeRows,mazeColumns];

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
					mazeCells[r,c].westWall = Instantiate (wall, new Vector3 (r*size, 0, (c*size) - (size/2f)), Quaternion.identity) as GameObject;
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

	private void CreateAndPlacePlane() {
		// GameObject plane = GameObject.CreatePrimitive (PrimitiveType.Plane);
		width = mazeRows /10;
		height = mazeColumns /10 ;
		float wallThickness = wall.transform.localScale.z;
		wallThickness += wallThickness / size;
		plane = Instantiate(roof, new Vector3 (mazeRows/2f - wallThickness, 2f, mazeColumns/2f - wallThickness), Quaternion.identity) as GameObject;
		plane.transform.localScale = new Vector3(width, 1f, height);
		// plane.transform.position = new Vector3 (mazeRows - wallThickness - width, size, mazeColumns - wallThickness - height);
	}

	private void SetSecondCameraPosition() {
		//Trigonometrie for yAxis for quadratical maze
		float yAxis = plane.transform.localScale.x * 10 + plane.transform.localScale.x * 2; 
		//float yAxis = plane.transform.position.x/Mathf.Tan(45f);
		secondCamera.position = new Vector3 (plane.transform.position.x, yAxis, plane.transform.position.z);
	}
}
