using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPositionScript : MonoBehaviour {

	public int playerX, playerZ;

	private InputScript script;
	private bool playerPosChanged;

	void FixedUpdate() {
		if (controllerFound()) {
			playerPosChanged = script.GetPlayerX() != this.playerX || script.GetPlayerZ() != this.playerZ;
			if (playerPosChanged) {
				this.playerX = script.GetPlayerX();
				this.playerZ = script.GetPlayerZ();
			}
		}
	}

	private bool controllerFound() {
		if (script == null) {
			GameObject NonHmdController = GameObject.Find("[CameraRig]");
			if (NonHmdController != null) {
                script = NonHmdController.GetComponent<InputScript>();
				return true;
			}
			return false;
		}
		return true;
	}
}