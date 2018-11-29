using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeCheckpoint : MonoBehaviour {

	GameObject objToSpawn;
	
	// Update is called once per frame
	void FixedUpdate () {
		objToSpawn = new GameObject ("Checkpoint");
		objToSpawn.tag = "Checkpoint";
		objToSpawn.transform.position = this.transform.position;
	}
}
