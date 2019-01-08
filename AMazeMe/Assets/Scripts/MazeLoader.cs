using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MazeLoader : MonoBehaviour {
	public int mazeRows; // x-Axis
	public int mazeColumns; // z-Axis
    public GameObject[] wallArray;
	public GameObject floor;
	public GameObject corner;
	public GameObject chest;
    public GameObject breadcrumb;
	public float size;

	private MazeCell[,] mazeCells;
	private Vector3 objectScale;
    private List<Breadcrumb> crumbs;

    void Start () {
		InitializeMaze ();

		MazeAlgorithm ma = new HuntAndKillMazeAlgorithm (mazeCells);
		ma.CreateMaze ();

        PlaceChest();

        PlaceBreadCrumbs();
	}

	private void InitializeMaze() {
		mazeCells = new MazeCell[mazeRows, mazeColumns];
        
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
        int draw = (int) Random.Range(0, wallArray.Length - 1);
        return wallArray[draw];
    }

    private void PlaceChest() {
        Vector3 chestLocation = mazeCells[mazeRows - 1, mazeColumns - 1].floor.transform.localPosition;
        chest = Instantiate(chest, new Vector3(chestLocation.x, -1f, chestLocation.z), Quaternion.identity) as GameObject;

        // Turn the chest
        bool southWall = mazeCells[mazeRows - 2, mazeColumns - 1].southWallExists;
        bool eastWall = mazeCells[mazeRows - 1, mazeColumns - 2].eastWallExists;
        float turnY = !eastWall && !southWall ? -130f : southWall ? -180f : -90f;
        Quaternion target = Quaternion.Euler(0, turnY, 0);
        chest.transform.rotation = target;
    }

    private void PlaceBreadCrumbs() {
        crumbs = new List<Breadcrumb>();

        int breadcrumbCount = GameObject.FindGameObjectsWithTag("Loot").Length;

        for (int counter = 0; counter < breadcrumbCount; counter++) {
            int x = Random.Range(0, mazeRows - 1);
            int z = Random.Range(0, mazeColumns - 2); // TODO? -2 and not -1 because: Weird bug. z == mazeColumns at time although Random.Range should only be able to return 0-14 with (0, 14) as input
            Vector2 target = new Vector2(x, z);
            bool farEnough = true;

            foreach (Breadcrumb crumb in crumbs) {
                if (crumb.position == target) {
                    farEnough = false;
                    break;
                }
                if (Vector2.Distance(target, crumb.position) < 3) {
                    farEnough = false;
                    break;
                }
            }
            if (farEnough) {
                Vector3 floorPosition = mazeCells[x, z].floor.transform.position;
                Vector3 breadcrumbPosition = new Vector3(floorPosition.x, -1.1f, floorPosition.z);
                Breadcrumb crumbObj = new Breadcrumb();
                crumbObj.crumb = Instantiate(breadcrumb, breadcrumbPosition, Quaternion.identity) as GameObject;
                crumbObj.position = target;
                Debug.Log(target);
                crumbs.Add(crumbObj);
            }
        }
    }

    public MazeCell[,] GetMazeCells() {
        return mazeCells;
    }

    public int GetMazeSize() {
        return (int)size;
    }

    public List<Breadcrumb> GetCrumbs() {
        return crumbs;
    }
}
