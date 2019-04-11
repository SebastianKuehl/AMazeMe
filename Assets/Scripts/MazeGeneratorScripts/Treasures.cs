using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class Treasures : MonoBehaviour {

    public GameObject chest;
    public GameObject treasurebag;
    public Tilemap floorTileMap;
    public Tile treasureMarkTile;
	public TextMeshProUGUI lootCounterText;
	public TextMeshProUGUI keyFoundText;

    private DataScript data;
    private MazeLoader mazeStructure;
    private InputScript inputScript;
    private List<TreasureBag> treasureBagList;
    private List<Vector2> treasurePositionList;
    private GameObject[] bagsInTreasureRoom;
    private int treasureCount, mazeSize, collectedTreasureCount;
	private bool keyFound;

    void Awake() {
        GameObject obj = GameObject.Find("Settings");
        data = obj.GetComponent<DataScript>();
        treasureCount = data.treasures;
        mazeSize = data.mazeSize;

        obj = GameObject.Find("[CameraRig]");
        while (!obj) {
            obj = GameObject.Find("[CameraRig]");
        }
        mazeStructure = obj.GetComponent<MazeLoader>();
        inputScript = obj.GetComponent<InputScript>();
    }

    private void Start() {
        GeneratePositionsForTreasureBags();
        HideTreasureBagsInFinalRoom();
        PlaceTreasureBagsInMaze();
        PlaceTreasureMarksInTileMap();
    }

    public void Update() {
        Vector2 currentPos = new Vector2(inputScript.GetPlayerX(), inputScript.GetPlayerZ());
        
        foreach (TreasureBag container in treasureBagList) {
            if (container.position != currentPos) {
                continue;
            }

            collectedTreasureCount += 1;

			CheckForKey ();

            Destroy(container.bag);

            treasureBagList.Remove(container);
            treasurePositionList.Remove(container.position);
            break;
        }
		data.treasuresFoundCount = collectedTreasureCount;
		lootCounterText.text = "Treasures: " + collectedTreasureCount + "/" + treasureCount;
    }

	private void CheckForKey () {
		if (!keyFound) {
			float randomValue = Random.Range (0.5f, 1f);
			float currentChance = (float) collectedTreasureCount / (float) treasureCount;
			keyFound = randomValue <= currentChance;
			if (keyFound) {
				keyFoundText.text = "Key: 1/1";
                data.foundKey = true;
			}
		}
	}

    private void GeneratePositionsForTreasureBags() {
        treasurePositionList = new List<Vector2>();

        for (int i = 0; i < treasureCount; i++) {
			Vector2 targetPosition;
			do {
				targetPosition = new Vector2 (Random.Range (1, mazeSize - 2), Random.Range (1, mazeSize - 2));
			} while(treasurePositionList.Contains (targetPosition));
            treasurePositionList.Add(targetPosition);
        }
    }

    private void HideTreasureBagsInFinalRoom() {
        bagsInTreasureRoom = GameObject.FindGameObjectsWithTag("Loot");
        foreach (GameObject obj in bagsInTreasureRoom) {
            obj.SetActive(false);
        }
    }

    private void PlaceTreasureBagsInMaze() {
        treasureBagList = new List<TreasureBag>();
        for (int i = 0; i < treasureCount; i++) {
            int x = (int) treasurePositionList[i].x;
            int y = (int) treasurePositionList[i].y;
            Vector3 floorPosition = mazeStructure.GetFloorPosition(x, y);
            TreasureBag obj = new TreasureBag() {
                bag = Object.Instantiate(treasurebag, new Vector3(floorPosition.x, 0f, floorPosition.z), Quaternion.identity) as GameObject,
                position = treasurePositionList[i]
            };
            treasureBagList.Add(obj);
        }
    }

    private void PlaceTreasureMarksInTileMap() {
        float probabilityToShowUp = data.treasureChance;
        foreach (TreasureBag bag in treasureBagList) {
            float rand = Random.value;
            if (rand <= probabilityToShowUp) {
                floorTileMap.SetTile(new Vector3Int((int)bag.position.x, (int)bag.position.y, 0), treasureMarkTile);
            }
        }
    }
}
