using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRotationScript : MonoBehaviour {

	public int playerYRotation;
    public RawImage rawImage;

    private void FixedUpdate() {
        rawImage.rectTransform.eulerAngles = new Vector3(0f, 0f, 90-playerYRotation);
    }
}
