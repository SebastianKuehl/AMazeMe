using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCheckPoint : MonoBehaviour {

	public Material lineMat;// = new Material("Shader \"Lines/Colored Blended\" {" + "SubShader { Pass { " + "    Blend SrcAlpha OneMinusSrcAlpha " + "    ZWrite Off Cull Off Fog { Mode Off } " + "    BindChannels {" + "      Bind \"vertex\", vertex Bind \"color\", color }" + "} } }");
	public List<Vector2> PlayerPositionList;
	public GameObject floor;

	private DataScript dataScript;
	private int[,] maze;
	private bool mazeGenerated = false;
	private GameObject[,] mazeFloorRows;
	private int playerX, playerZ, oldPlayerX, oldPlayerZ;
	private int mazeRows, mazeColumns;

	void Start()
	{
		PlayerPositionList = new List<Vector2> ();
	}

	void FixedUpdate() {
		if (dataScript == null) {
			GameObject tempObject = GameObject.Find ("DataCube");
			if (tempObject != null) {
				dataScript = tempObject.GetComponent<DataScript> ();
			}
		} else {
			oldPlayerX = playerX;
			oldPlayerZ = playerZ;
			playerX = (int) dataScript.PlayerPosVec.x;
			playerZ = (int) dataScript.PlayerPosVec.z;
			mazeRows = dataScript.mazeRows;
			mazeColumns = dataScript.mazeColumns;
		}
		// Generate MazeArray if not done before
		if (mazeRows != 0 && mazeColumns != 0) {
			if (!mazeGenerated) {
				mazeGenerated = true;
				maze = new int[mazeRows, mazeColumns];
				mazeFloorRows = new GameObject[mazeRows * 3, mazeColumns * 3];
				InitializeMaze ();
			}

			Vector3 blockPosition = mazeFloorRows [playerX*3+1, playerZ*3+1].transform.position;
			mazeFloorRows [playerX*3+1, playerZ*3+1].transform.position = new Vector3 (blockPosition.x, -0.6f, blockPosition.z);

			if (playerX == oldPlayerX) {
				if (oldPlayerZ == playerZ)
					return;
				int direction = playerZ > oldPlayerZ ? 1 : -1;

				Vector3 blockInBetween0 = mazeFloorRows [oldPlayerX*3+1, oldPlayerZ*3+1+(1*direction)].transform.position;
				mazeFloorRows [oldPlayerX*3+1, oldPlayerZ*3+1+(1*direction)].transform.position = new Vector3 (blockInBetween0.x, -0.6f, blockInBetween0.z);

				Vector3 blockInBetween1 = mazeFloorRows [oldPlayerX*3+1, oldPlayerZ*3+1+(2*direction)].transform.position;
				mazeFloorRows [oldPlayerX*3+1, oldPlayerZ*3+1+(2*direction)].transform.position = new Vector3 (blockInBetween1.x, -0.6f, blockInBetween1.z);
			} else {
				if (oldPlayerX == playerX)
					return;
				int direction = playerX > oldPlayerX ? 1 : -1;

				Vector3 blockInBetween0 = mazeFloorRows [oldPlayerX*3+1+(1*direction), oldPlayerZ*3+1].transform.position;
				mazeFloorRows [oldPlayerX*3+1+(1*direction), oldPlayerZ*3+1].transform.position = new Vector3 (blockInBetween0.x, -0.6f, blockInBetween0.z);

				Vector3 blockInBetween1 = mazeFloorRows [oldPlayerX*3+1+(2*direction), oldPlayerZ*3+1].transform.position;
				mazeFloorRows [oldPlayerX*3+1+(2*direction), oldPlayerZ*3+1].transform.position = new Vector3 (blockInBetween1.x, -0.6f, blockInBetween1.z);

			}
		}


	}

	void InitializeMaze() {
		for (int r = 0; r < mazeRows * 3; r++) {
			for (int c = 0; c < mazeColumns * 3; c++) {
				mazeFloorRows[r, c] =  Instantiate (floor, new Vector3 (r, 0, c), Quaternion.identity) as GameObject;
				/*
				mazeCells [r, c] = new MazeCell ();

				// For now, use the same wall object for the floor!
				mazeCells [r, c] .floor = Instantiate (wall, new Vector3 (r*size, -1.5f, c*size), Quaternion.identity) as GameObject;
				mazeCells [r, c] .floor.name = "Floor " + r + "," + c;
				mazeCells [r, c] .floor.transform.Rotate (Vector3.right, 90f);
				mazeCells [r, c].floor.transform.localScale = new Vector3 (mazeCells [r, c] .floor.transform.localScale.x, mazeCells [r, c] .floor.transform.localScale.x, mazeCells [r, c] .floor.transform.localScale.z);
				*/
			}
		}
	}
		
	private void DrawConnectingLines() {
		/*
		if (rightControllerScript == null) {
			if (GameObject.Find ("NonHmdController") != null) {
				rightControllerScript = GameObject.Find ("NonHmdController").GetComponent<RightController> ();
			}
		} else {
			// Punkte holen
			PlayerPosition = rightControllerScript.PlayerPosition;
			if (PlayerPosition.Count > 1) {
				for (int i = 0; i < PlayerPosition.Count - 1; i++) {
					// Debug.DrawLine (PlayerPosition[i], PlayerPosition[i + 1], Color.green);
					GL.Begin(GL.LINES);
					lineMat.SetPass(0);
					GL.Color(new Color(70f, 250f, 60f, 0.5f));
					GL.Vertex3(PlayerPosition[i].x, 2.1f, PlayerPosition[i].y);
					GL.Vertex3(PlayerPosition[i + 1].x, 2.1f, PlayerPosition[i + 1].y);
					GL.End();
				}
			}
		}
		*/
	}

	// To show the lines in the game window when it is running
	void OnPostRender() {
		DrawConnectingLines();
	}

	// To show the lines in the editor
	/*
	void OnDrawGizmos() {
		DrawConnectingLines();
	}
	*/
}