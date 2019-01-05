using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerMovementScript : MonoBehaviour {
    private float step;

    private InputScript inputScript;

    void FixedUpdate () {
        if (GotInputScript()) {
            if (!inputScript.UsesMarker()) {
                GetComponent<Renderer>().enabled = false;
                return;
            } else {
                GetComponent<Renderer>().enabled = true;
            }
            step = (step + 0.05f) % 360f;
            float x = 0.5f + inputScript.GetMarkerX() * 5f;
            float y = MarkerOnPlayer() ? 4f : 2.5f;
            y += (Mathf.Sin(step) / 2);
            float z = 0.5f + inputScript.GetMarkerZ() * 5f;
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

    private bool MarkerOnPlayer() {
        return inputScript.GetMarkerX() == inputScript.GetPlayerX() && inputScript.GetMarkerZ() == inputScript.GetPlayerZ();
    }
}
