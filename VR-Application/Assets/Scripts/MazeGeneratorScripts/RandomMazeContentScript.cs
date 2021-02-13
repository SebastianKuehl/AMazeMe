using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMazeContentScript : MonoBehaviour
{

    public GameObject dirt;
    public GameObject torch;
    public GameObject chair;
    public GameObject table;
    public GameObject[] moss;
    public GameObject[] pots;
    public GameObject[] spiderweb;

    private const float SIZE = 3f;

    private DataScript data;
    private InputScript inputScript;
    private MazeCell[,] maze;
    private List<GameObject> torches;

    private Vector3 position, scaleFactor;
    private float rotationValue;

    void Awake() {
        GameObject dataObj = GameObject.Find("Settings");
        data = dataObj.GetComponent<DataScript>();

        torches = new List<GameObject>();
    }

    void Start() {
        GameObject mazeLoaderObj = GameObject.Find("[CameraRig]");
        MazeLoader mazeScript = mazeLoaderObj.GetComponent<MazeLoader>();
        inputScript = mazeLoaderObj.GetComponent<InputScript>();
        maze = mazeScript.GetMazeCells();

        for (int i = 0; i < data.mazeSize; i++) {
            for (int j = 0; j < data.mazeSize; j++) {
                PlaceDirt(i, j);
                PlaceTorches(i, j);
                PlaceMoss(i, j);
                PlaceSpiderWeb(i, j);
                PlaceTableAndChair(i, j);
                PlacePots(i, j);
            }
        }
    }

    void Update() {
        foreach (GameObject torch in torches) {
            Vector3 currentPos = maze[inputScript.GetPlayerX(), inputScript.GetPlayerZ()].floor.transform.position;
            if (Vector3.Distance(currentPos, torch.transform.position) <= 10f) {
                torch.SetActive(true);
            } else {
                torch.SetActive(false);
            }
        }
    }

    private void PlaceDirt(int i, int j) {
        float randomFactor = Random.Range(0f, 1f);
        if (randomFactor <= 0.1f) {
            position = new Vector3(i * SIZE - Random.Range(-SIZE / 2f, SIZE / 2f), 0f, j * SIZE - Random.Range(-SIZE / 2f, SIZE / 2f));
            scaleFactor = new Vector3(SIZE / 2f, SIZE / 2f, SIZE / 2f);
            rotationValue = 90f;
            CreateObj(dirt, "Dirt " + i + "," + j);
        }
    }

    private void PlaceTorches(int i, int j) {
        float randomFactor = Random.Range(0f, 1f);
        if (randomFactor <= 0.2f) {
            if (maze[i, j].northWallExists) {
                position = new Vector3((i * SIZE) - (SIZE / 2f), 2f, j * SIZE);
                rotationValue = 90f;

            } else if (maze[i, j].eastWallExists) {
                position = new Vector3(i * SIZE, 2f, (j * SIZE) + (SIZE / 2f));
                rotationValue = 180f;
            } else if (maze[i, j].southWallExists) {
                position = new Vector3((i * SIZE) + (SIZE / 2f), 2f, j * SIZE);
                rotationValue = -90f;

            } else if (maze[i, j].westWallExists) {
                position = new Vector3(i * SIZE, 2f, (j * SIZE) - (SIZE / 2f));
            }
            torches.Add(CreateObj(torch, "Torch " + i + "," + j));
        }
    }

    private void PlaceMoss(int i, int j) {
        float randomFactor = Random.Range(0f, 1f);
        if (randomFactor <= 0.2f) {
            float height = randomFactor <= 0.5f ? SIZE - 0.75f : SIZE - 0.4f;
            if (maze[i, j].northWallExists) {
                position = new Vector3((i * SIZE) - (SIZE / 2f) + 0.05f, height, j * SIZE);
                rotationValue = 90f;

            } else if (maze[i, j].eastWallExists) {
                position = new Vector3(i * SIZE, height, (j * SIZE) + (SIZE / 2f) - 0.1f);
                rotationValue = 180f;

            } else if (maze[i, j].southWallExists) {
                position = new Vector3((i * SIZE) + (SIZE / 2f) - 0.1f, height, j * SIZE);
                rotationValue = -90f;

            } else if (maze[i, j].westWallExists) {
                position = new Vector3(i * SIZE, height, (j * SIZE) - (SIZE / 2f) + 0.1f);
            }
            CreateObj(GetRandomObjFromArray(moss), "Moss " + i + "," + j);
        }
    }

    private void PlaceSpiderWeb(int i, int j) {
        float randomFactor = Random.Range(0f, 1f);
        if (randomFactor <= 0.3f) {
            float height = SIZE - 0.35f;
            if (maze[i, j].northWallExists) {
                position = new Vector3(-1f + (i * SIZE), height, j * SIZE + Random.Range(-SIZE / 2f, SIZE / 2f));
                rotationValue = 180f;
            } else if (maze[i, j].eastWallExists) {
                position = new Vector3(i * SIZE + Random.Range(-SIZE / 2f, SIZE / 2f), height, 1f + (SIZE * j));
                rotationValue = 270f;
            } else if (maze[i, j].southWallExists) {
                position = new Vector3(1f + (SIZE * i), height, j * SIZE + Random.Range(-SIZE / 2f, SIZE / 2f));
            } else if (maze[i, j].westWallExists) {
                position = new Vector3(i * SIZE + Random.Range(-SIZE / 2f, SIZE / 2f), height, -1f + (j * SIZE));
                rotationValue = 90f;
            }
            scaleFactor = new Vector3(SIZE, SIZE, SIZE);
            CreateObj(GetRandomObjFromArray(spiderweb), "Spiderweb " + i + "," + j);
        }
    }

    private void PlaceTableAndChair(int i, int j) {
        float randomFactor = Random.Range(0f, 1f);
        if (randomFactor <= 0.01f) {
            Vector3 floorPosition = maze[i, j].floor.transform.position;
            position = new Vector3(-1.15f + floorPosition.x, 0f, 0.78f + floorPosition.z);
            rotationValue = maze[i, j].northWallExists ? 90f : 0f;
            CreateObj(table, "Table " + i + "," + j);

            if (maze[i, j].northWallExists) {
                position = new Vector3(-0.8f + floorPosition.x, 0f, 0.8f + floorPosition.z);
            } else {
                position = new Vector3(-1.2f + floorPosition.x, 0f, 0.5f + floorPosition.z);
            }
            CreateObj(chair, "Chair " + i + "," + j);
        }
    }

    private void PlacePots(int i, int j) {
        float randomFactor = Random.Range(0f, 1f);
        if (randomFactor <= 0.05f) {
            Vector3 floorPosition = maze[i, j].floor.transform.position;
            position = new Vector3(1f + floorPosition.x, 0.05f, -1f + floorPosition.z);
            CreateObj(GetRandomObjFromArray(pots), "Pot " + i + "," + j);
        }
    }

    private GameObject GetRandomObjFromArray(GameObject[] array) {
        int random = (int)Random.Range(0, array.Length);
        return array[random];
    }

    private GameObject CreateObj(GameObject targetObj, string name) {
        GameObject obj = Instantiate(targetObj, position, Quaternion.identity) as GameObject;
        obj.name = name;
        obj.transform.Rotate(Vector3.up * rotationValue);
        Vector3 objScale = obj.transform.localScale;
        obj.transform.localScale = Vector3.Scale(objScale, scaleFactor);
        ResetStartValues();
        return obj;
    }

    private void ResetStartValues() {
        position = new Vector3(0f, -30f, 0f);
        scaleFactor = new Vector3(SIZE / 2f, SIZE / 2f, SIZE / 2f);
        rotationValue = 0f;
    }
}
