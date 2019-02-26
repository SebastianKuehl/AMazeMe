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
    public GameObject treasurebag;
	public float size;

	private MazeCell[,] mazeCells;
	private Vector3 objectScale;
    private List<TreasureBag> bagList;

    void Start () {
		InitializeMaze ();

		MazeAlgorithm ma = new HuntAndKillMazeAlgorithm (mazeCells);
		ma.CreateMaze ();

        PlaceChest();

        HideTreasureBagsInTreasureRoom();
        PlaceTreasureBags();
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
				GameObject cornerObj = Instantiate (corner, new Vector3 (r*size - size/2f, 0, c*size - size/2f), Quaternion.identity) as GameObject;
				objectScale = cornerObj.transform.localScale;
                cornerObj.transform.localScale = new Vector3 (objectScale.x * size, objectScale.y, objectScale.z * size);
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

    private void HideTreasureBagsInTreasureRoom() {
        GameObject[] bags = GameObject.FindGameObjectsWithTag("Loot");
        foreach (GameObject obj in bags) {
            Component[] items = obj.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in items) {
                renderer.enabled = false;
            }
        }
    }

    private void PlaceTreasureBags() {
        bagList = new List<TreasureBag>();

        int bagCounter = GameObject.FindGameObjectsWithTag("Loot").Length;

        for (int counter = 0; counter < bagCounter; counter++) {
            bool validPosition = false;

            int x = 0, z = 0;
            Vector2 target = Vector2.zero;

            // Generate a new position for a crumb while the targeted position is a crumb or not far enough from other crumbs
            while (!validPosition) {
                x = Random.Range(0, mazeRows - 1);
                z = Random.Range(0, mazeColumns - 1);

                while((x == 0 && z == 0) || (x == mazeRows - 1 && z == mazeColumns - 1)) {
                    x = Random.Range(0, mazeRows - 1);
                    z = Random.Range(0, mazeColumns - 1);
                }

                target = new Vector2(x, z);

                foreach (TreasureBag bag in bagList) {
                    if (bag.position == target || Vector2.Distance(target, bag.position) < 3) {
                        validPosition = false;
                        break;
                    }
                }
                validPosition = true;

            }

            Vector3 floorPosition = mazeCells[x, z].floor.transform.position;
            TreasureBag bagObj = new TreasureBag() {
                bag = Instantiate(treasurebag, new Vector3(floorPosition.x, -1.1f, floorPosition.z), Quaternion.identity) as GameObject,
                position = target
            };
            bagList.Add(bagObj);
        }
    }

    public MazeCell[,] GetMazeCells() {
        return mazeCells;
    }

    public int GetMazeSize() {
        return (int)size;
    }

    public List<TreasureBag> GetBags() {
        return bagList;
    }
}
