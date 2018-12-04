using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCheckPoint : MonoBehaviour {

	public Material lineMat;// = new Material("Shader \"Lines/Colored Blended\" {" + "SubShader { Pass { " + "    Blend SrcAlpha OneMinusSrcAlpha " + "    ZWrite Off Cull Off Fog { Mode Off } " + "    BindChannels {" + "      Bind \"vertex\", vertex Bind \"color\", color }" + "} } }");

	public List<Vector2> PlayerPositionList;
	public int mazeRows, mazeColumns;
	public Quaternion cameraRot;

	void Start() {
		PlayerPositionList = new List<Vector2> ();
	}

	private void DrawConnectingLines() {



		/*
		if (rightControllerScript == null) {
			if (GameObject.Find ("NonHmdController") != null) {
				rightControllerScript = GameObject.Find ("NonHmdController").GetComponent<RightController> ();
			}
		} else {
			// Punkte holen
			PlayerPosition = rightControllerScript.PlayerPosition;
			if (PlayerPosition.Count > 1) {
				for (int i = 0; i < PlayerPosition.Count - 1; i++) {
					// Debug.DrawLine (PlayerPosition[i], PlayerPosition[i + 1], Color.green);
					GL.Begin(GL.LINES);
					lineMat.SetPass(0);
					GL.Color(new Color(70f, 250f, 60f, 0.5f));
					GL.Vertex3(PlayerPosition[i].x, 2.1f, PlayerPosition[i].y);
					GL.Vertex3(PlayerPosition[i + 1].x, 2.1f, PlayerPosition[i + 1].y);
					GL.End();
				}
			}
		}
		*/
	}

	// To show the lines in the game window when it is running
	void OnPostRender() {
		DrawConnectingLines();
	}

	// To show the lines in the editor
	void OnDrawGizmos() {
		DrawConnectingLines();
	}
}