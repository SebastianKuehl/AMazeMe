using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullScript : MonoBehaviour {

	private DataScript data;
	private MazeCell[,] mazeStructure;

	public Vector2 startPos;
	public Vector2 targetPos;
	public bool reachedTargetPos, moveOnXAxis;

	void Awake() {
		GameObject dataObj = GameObject.Find ("Settings");
		data = dataObj.GetComponent<DataScript> ();
	}

	// Use this for initialization
	void Start () {
		mazeStructure = GameObject.Find ("[CameraRig]").GetComponent<MazeLoader>().GetMazeCells();
		startPos = new Vector2((int) Random.Range(0f, (data.mazeSize - 1f) * 3f), (int) Random.Range(0f, (data.mazeSize - 1f) * 3f));
		targetPos = startPos;
		reachedTargetPos = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (reachedTargetPos) {
			GenerateNewTarget ();
		} else {
			CreepTowardsTarget ();
		}
	}

	private void GenerateNewTarget() {
		moveOnXAxis = Random.Range (0f, 1f) <= 0.5;
		if (moveOnXAxis) {
			targetPos.x = (int) Random.Range (0f, (data.mazeSize - 1f) * 3f);
			if (targetPos.x < transform.position.x) {
				transform.rotation = Quaternion.Euler (0f, -90f, 0f);
			} else {
				transform.rotation = Quaternion.Euler (0f, 90f, 0f);
			}
		} else {
			targetPos.y = (int) Random.Range (0f, (data.mazeSize - 1f) * 3f);
			if (targetPos.y < transform.position.z) {
				transform.rotation = Quaternion.Euler (0f, 180f, 0f);
			} else {
				transform.rotation = Quaternion.Euler (0f, 0f, 0f);
			}
		}
		reachedTargetPos = false;
	}

	private void CreepTowardsTarget() {
		if (moveOnXAxis) {
			float ownXAxisValue = transform.position.x;
			if (ownXAxisValue < targetPos.x) {
				transform.position = new Vector3 (transform.position.x + 0.01f, transform.position.y, transform.position.z);
			} else {
				transform.position = new Vector3 (transform.position.x - 0.01f, transform.position.y, transform.position.z); 
			}
			if (transform.position.x > targetPos.x - 0.1f && transform.position.x < targetPos.x + 0.1f) {
				reachedTargetPos = true;
			}
		} else {
			float ownZAxisValue = transform.position.z;
			if (ownZAxisValue < targetPos.y) {
				transform.position = new Vector3 (transform.position.x, transform.position.y, transform.position.z + 0.01f);
			} else {
				transform.position = new Vector3 (transform.position.x, transform.position.y, transform.position.z - 0.01f); 
			}
			if (transform.position.z > targetPos.y - 0.1f && transform.position.z < targetPos.y + 0.1f) {
				reachedTargetPos = true;
			}
		}
	}
}
