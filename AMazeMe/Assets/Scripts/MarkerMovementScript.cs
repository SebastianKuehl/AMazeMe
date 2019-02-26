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

        if (!inputScript.UsesMarker()) {
            GetComponent<Renderer>().enabled = false;
            return;
        } else {
            GetComponent<Renderer>().enabled = true;

            step = (step + 0.05f) % 360f;
            float x = inputScript.GetMarkerX() * 5f;
            float y = -1.1f;
            y -= (Mathf.Sin(step) / 15);
            float z = inputScript.GetMarkerZ() * 5f;
            this.transform.position = new Vector3(x, y, z);
        }
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
