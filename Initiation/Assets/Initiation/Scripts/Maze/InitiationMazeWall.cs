using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitiationMazeWall : MonoBehaviour
{
    public Direction direction;

	public void InitWall(Direction dir, float cellWidth,float cellHeight, Vector3 offset, float x, float y, float z)
	{
		this.direction = dir;
		switch(direction) {
		case Direction.Left:
			transform.position = new Vector3(x - cellWidth / 2,y,z) + offset;
			transform.rotation = Quaternion.Euler(0,270,0);				
			break;
		case Direction.Right:
			transform.position = new Vector3(x + cellWidth / 2,y,z) + offset;
			transform.rotation = Quaternion.Euler(0,90,0);
			break;
		case Direction.Front:
			transform.position = new Vector3(x,y,z + cellHeight / 2) + offset;
			transform.rotation = Quaternion.Euler(0,0,0);
			break;
		case Direction.Back:
			transform.position = new Vector3(x,y,z - cellHeight / 2) + offset;
			transform.rotation = Quaternion.Euler(0,180,0);
			break;
		}

	}
    
}
