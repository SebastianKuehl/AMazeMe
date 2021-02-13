using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**  
 * Original code by @kylewbanks
 * kylewbanks.com/blog/unity3d-panning-and-pinch-to-zoom-camera-with-touch-and-mouse-input
 */
public class TouchControl : MonoBehaviour
{
	private static readonly float PanSpeed = 10f;
	private static readonly float ZoomSpeedMouse = 10f;

	private static readonly float[] ZoomBounds = new float[]{10f, 85f};

	private Camera cam;

	private Vector3 lastPanPosition;
	private int panFingerId; // Touch mode only

	private bool wasZoomingLastFrame; // Touch mode only
	private Vector2[] lastZoomPositions; // Touch mode only

	void Awake() {
		cam = GetComponent<Camera>();
	}

	void Update() {
		HandleMouse();
	}

	void HandleMouse() {
		// On mouse down, capture it's position.
		// Otherwise, if the mouse is still down, pan the camera.
		if (Input.GetMouseButtonDown(0)) {
			lastPanPosition = Input.mousePosition;
		} else if (Input.GetMouseButton(0)) {
			PanCamera(Input.mousePosition);
		}

		// Check for scrolling to zoom the camera
		float scroll = Input.GetAxis("Mouse ScrollWheel");
		ZoomCamera(scroll, ZoomSpeedMouse);
	}

	void PanCamera(Vector3 newPanPosition) {
		// Determine how much to move the camera
		Vector3 offset = cam.ScreenToViewportPoint(lastPanPosition - newPanPosition);
		Vector3 move = new Vector3(offset.x * PanSpeed, offset.y * PanSpeed, 0);

		// Perform the movement
		transform.Translate(move, Space.World);  

		// Cache the position
		lastPanPosition = newPanPosition;
	}

	void ZoomCamera(float offset, float speed) {
		if (offset == 0) {
			return;
		}

		float size = cam.fieldOfView - (offset * speed);
		cam.fieldOfView = size < ZoomBounds [0] ? ZoomBounds[0] : size > ZoomBounds [1] ? ZoomBounds [1] : size;
	}
}
