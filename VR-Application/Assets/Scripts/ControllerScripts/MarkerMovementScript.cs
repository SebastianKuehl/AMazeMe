using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerMovementScript : MonoBehaviour {
    private float step;
    private InputScript inputScript;

    void FixedUpdate () {
        if (!GotInputScript()) {
            return;
        }
    
        step = (step + 0.05f) % 360f;
        float x = inputScript.GetMarkerX() * 3f;
        float y = 0f;
        float z = inputScript.GetMarkerZ() * 3f;
        this.transform.position = new Vector3(x, y, z);
    }

    private bool GotInputScript() {
        if (inputScript == null) {
            GameObject cameraRig = GameObject.Find("[CameraRig]");
            if (cameraRig != null) {
                inputScript = cameraRig.GetComponent<InputScript>();
                return true;
            }
            return false;
        }
        return true;
    }
}
