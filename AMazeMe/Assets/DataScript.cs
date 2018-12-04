using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataScript : MonoBehaviour {

    public Vector3 PlayerPosVec;
    public Quaternion PlayerRotVec;
    public int mazeRows, mazeColumns;

    private MazeLoader MazeLoaderScript;
    private RightController RightControllerScript;
    private Transform ViveCameraRotation;
    private bool PlayerPosChanged;

    private void FixedUpdate()
    {
        // Update info about maze size 
        if (MazeLoaderScript == null)
        {
            MazeLoaderScript = GameObject.Find("[CameraRig]").GetComponent<MazeLoader>();
            this.mazeRows = MazeLoaderScript.mazeRows;
            this.mazeColumns = MazeLoaderScript.mazeColumns;
        }

        // Update player position
        if (RightControllerScript == null)
        {
            GameObject NonHmdController = GameObject.Find("NonHmdController");
            if (NonHmdController != null)
            {
                RightControllerScript = NonHmdController.GetComponent<RightController>();
            }
        } else
        {
            PlayerPosChanged = RightControllerScript.playerX != this.PlayerPosVec.x || RightControllerScript.playerZ != this.PlayerPosVec.z;

            if (PlayerPosChanged)
            {
                this.PlayerPosVec = new Vector3(RightControllerScript.playerX, 0, RightControllerScript.playerZ);
            }
        }

        // Update player camera rotation
        if (ViveCameraRotation == null)
        {
            ViveCameraRotation = GameObject.Find("Camera (eye)").GetComponent<Transform>();
        } else
        {
            this.PlayerRotVec = ViveCameraRotation.rotation;
        }

    }
}
