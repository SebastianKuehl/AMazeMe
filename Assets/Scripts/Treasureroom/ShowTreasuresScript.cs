using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowTreasuresScript : MonoBehaviour {

    private void Awake() {
        GameObject dataObject = GameObject.Find("Settings");
        DataScript data = dataObject.GetComponent<DataScript>();
		int treasuresFoundCount = data.treasuresFoundCount;

        GameObject[] lootBoxes = GameObject.FindGameObjectsWithTag("Loot");

		if (lootBoxes.Length < treasuresFoundCount || lootBoxes.Length <= 0) {
            Debug.Log("Could not enable any or further treasures to show..");
            return;
        }

        // Hide all treasures that have not been collected
        for (int i = 0; i < lootBoxes.Length; i++) {
			if (i >= treasuresFoundCount) {
                lootBoxes[i].SetActive(false);
            } 
        }
    }
}
