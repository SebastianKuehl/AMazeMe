using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowScript : MonoBehaviour {

    private PlayerRotationScript rotationScript;
    
	void FixedUpdate () {
        if (GotValidScript()) {
            SetRotationOfArrow();
        }
	}

    private bool GotValidScript() {
        if (!rotationScript) {
            GameObject obj = GameObject.Find("PlayerRotationObject");
            if (obj) {
                rotationScript = obj.GetComponent<PlayerRotationScript>();
                return (rotationScript == null);
            } else {
                return false;
            }
        }
        return true;
    }

    private void SetRotationOfArrow() {
        RectTransform transform = gameObject.GetComponent<RectTransform>();
        if (transform) {
            int yAxis = 90 - rotationScript.playerYRotation;
            transform.eulerAngles = new Vector3(0f, 0f, yAxis);
        }
    }
}
