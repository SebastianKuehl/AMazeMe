using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPositionScript : MonoBehaviour {

	public int playerX, playerZ;

	private RightControllerScript script;
	private bool playerPosChanged;

	void FixedUpdate() {
		if (controllerFound()) {
			playerPosChanged = script.playerX != this.playerX || script.playerZ != this.playerZ;
			if (playerPosChanged) {
				this.playerX = script.playerX;
				this.playerZ = script.playerZ;
			}
		}
	}

	private bool controllerFound() {
		if (script == null) {
			GameObject NonHmdController = GameObject.Find("Controller (right)");
			if (NonHmdController != null) {
                script = NonHmdController.GetComponent<RightControllerScript>();
				return true;
			}
			return false;
		}
		return true;
	}
}