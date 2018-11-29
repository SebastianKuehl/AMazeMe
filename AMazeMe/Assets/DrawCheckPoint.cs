using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCheckPoint : MonoBehaviour {

	public GameObject[] checkpoint;

	public Material lineMat;// = new Material("Shader \"Lines/Colored Blended\" {" + "SubShader { Pass { " + "    Blend SrcAlpha OneMinusSrcAlpha " + "    ZWrite Off Cull Off Fog { Mode Off } " + "    BindChannels {" + "      Bind \"vertex\", vertex Bind \"color\", color }" + "} } }");

	private void DrawConnectingLines() {

		checkpoint = GameObject.FindGameObjectsWithTag ("Checkpoint"); // TODO Sehr ressourcenaufwändig
		if (checkpoint.Length > 1) {
			for (int i = 0; i < checkpoint.Length - 1; i++) {
				GL.Begin(GL.LINES);
				lineMat.SetPass(0);
				GL.Color(new Color(70f, 250f, 60f, 0.5f));
				GL.Vertex3(checkpoint [i].transform.position.x, 2.1f, checkpoint [i].transform.position.z);
				GL.Vertex3(checkpoint [i + 1].transform.position.x, 2.1f, checkpoint [i + 1].transform.position.z);
				GL.End();
			}
		}
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
