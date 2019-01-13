using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureCountScript : MonoBehaviour {

    public int treasureCount = 0;

    private GameObject[] treasurebags;
    
	// Use this for initialization
	void Start () {
        treasurebags = GameObject.FindGameObjectsWithTag("Loot");
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        int counter = 0;
        foreach (GameObject bag in treasurebags) {
            Component[] items = bag.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in items) {
                if (renderer.enabled) {
                    counter++;
                    break;
                }
            }
        }
        treasureCount = counter;
    }
}
