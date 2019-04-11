using UnityEngine;

public class MazeCell {
	public bool visited = false;
	public GameObject northWall, southWall, eastWall, westWall, floor, ceiling;
	public bool northWallExists = true, southWallExists = true, eastWallExists = true, westWallExists = true;
}
