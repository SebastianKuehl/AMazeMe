﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour {

    //https://www.youtube.com/watch?v=OzENv_ZRA1g
    public GameObject wall;
    public float wallLength = 1.0f;
    public int xSize = 5;
    public int ySize = 5;
    private Vector3 initPos;
    private GameObject wallHolder;
	// Use this for initialization
	void Start () {
        CreateWalls();
	}
	
    void CreateWalls ()
    {
        wallHolder = new GameObject();
        wallHolder.name = "Maze";
        initPos = new Vector3((-xSize / 2) + wallLength / 2, 0.0f, (-ySize / 2) + wallLength / 2);
        Vector3 myPos = initPos;
        GameObject tempWall;

        //for x Axis
        for(int i = 0; i < ySize; i++)
        {
            for(int j = 0; j <= xSize; j++)
            {
                myPos = new Vector3(initPos.x + (j * wallLength) - wallLength / 2, 0.0f, initPos.z + (i * wallLength) - wallLength / 2);
                tempWall = Instantiate(wall, myPos, Quaternion.identity) as GameObject;
                tempWall.transform.parent = wallHolder.transform;
            }
        }
        //for y Axis
        for (int i = 0; i <= ySize; i++)
        {
            for (int j = 0; j < xSize; j++)
            {
                myPos = new Vector3(initPos.x + (j * wallLength), 0.0f, initPos.z + (i * wallLength) - wallLength);
                tempWall = Instantiate(wall, myPos, Quaternion.Euler(0.0f,90.0f,0.0f)) as GameObject;
                tempWall.transform.parent = wallHolder.transform;
            }
        }
    }

	// Update is called once per frame
	void Update () {
		
	}
}
