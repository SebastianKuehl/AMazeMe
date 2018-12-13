using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeDataScript : MonoBehaviour {

	public int mazeRows, mazeColumns;

	private MazeLoader MazeLoaderScript;

	void Start() {
		// Update info about maze size 
		if (MazeLoaderScript == null)
		{
			MazeLoaderScript = GameObject.Find("[CameraRig]").GetComponent<MazeLoader>();
			this.mazeRows = MazeLoaderScript.mazeRows;
			this.mazeColumns = MazeLoaderScript.mazeColumns;
		}
	}
}
