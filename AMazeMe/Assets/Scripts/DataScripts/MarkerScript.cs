using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerScript : MonoBehaviour {

    public int markerX, markerZ;
    public bool usesMarker;

    private InputScript script;
    private bool markerPosChanged;

    void FixedUpdate() {
        if (controllerFound()) {
            markerPosChanged = script.GetMarkerX() != this.markerX || script.GetMarkerZ() != this.markerZ;
            if (markerPosChanged) {
                this.markerX = script.GetMarkerX();
                this.markerZ = script.GetMarkerZ();
                this.usesMarker = script.UsesMarker();
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
