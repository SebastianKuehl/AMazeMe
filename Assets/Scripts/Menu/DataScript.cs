using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataScript : MonoBehaviour {

	public int mazeSize;
	public int treasures;
    public float treasureChance;
	public int treasuresFoundCount;
    public bool foundKey;
	public bool useController;

    void Awake() {
		DontDestroyOnLoad (this);
	}
}
