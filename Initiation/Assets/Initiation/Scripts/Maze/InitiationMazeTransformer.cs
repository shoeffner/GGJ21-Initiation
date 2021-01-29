using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mirror;

[RequireComponent(typeof(InitiationMazeSpawner))]
public class InitiationMazeTransformer : MonoBehaviour
{
	public List<GameObject> mazeComponents;

	Dictionary<string,GameObject> mazeComponentsDict;

	InitiationMazeSpawner maze;

	public enum MoveDirection {
		North,
		South,
		East,
		West
	}

	public MoveDirection moveDirection;
	public int cellIdx = 0;

	private bool moving = false;

	private Dictionary<string, object> movementParams = new Dictionary<string, object>()
	{
		{iT.MoveBy.time, 1.0f},
		{iT.MoveBy.easetype, iTween.EaseType.linear},
	};

	
    
	void Start()
	{
		maze = GetComponent<InitiationMazeSpawner>();

		mazeComponentsDict = new Dictionary<string,GameObject>();
		foreach(GameObject obj in mazeComponents) {
			mazeComponentsDict.Add(obj.name,obj);
		}
	}

	public void AfterCellsMoved(GameObject toDelete=null) {
		if (toDelete != null)
		{
				NetworkServer.Destroy(toDelete);
				//Destroy(toDelete);
				moving = false;
		}
	}

	/**
		* index is always from W -> E for N and S
		* index is always from S -> N for E and W
		*/
	public void AlterMaze(MoveDirection moveDirection, int index)
	{
		if (moving)
		{
			return;
		}
		if (index < 0)
		{
			Debug.LogError("Cannot move maze for negative indices.");
			return;
		}
		switch (moveDirection)
		{
			case MoveDirection.North:
			case MoveDirection.South:
				if (index > maze.Rows - 1)
				{
					Debug.LogError($"Only {maze.Rows} available (idx {index} too big)");
					return;
				}
				break;
			case MoveDirection.West:
			case MoveDirection.East:
				if (index > maze.Columns - 1)
				{
					Debug.LogError($"Only {maze.Columns} available (idx {index} too big)");
					return;
				}
				break;
		}

		moving = true;

		Dictionary<string, object> iTweenParams = new Dictionary<string, object>(movementParams);
        
		GameObject first = null;
		GameObject last;
		switch (moveDirection)
		{
			case MoveDirection.North:
				iTweenParams.Add(iT.MoveBy.z, maze.CellWidth);
				first = maze.MazeCells[maze.Rows - 1, index];
				last = maze.MazeCells[0, index];

				for (int row = maze.Rows - 1; row >= 0; --row)
				{
					if (maze.MazeCells[row, index] == last)
					{
						GameObject replace = Instantiate(first);
						replace.transform.position = last.transform.position - new Vector3(0, 0, maze.CellWidth);
						//NetworkServer.Spawn(replace);
						maze.MazeCells[row, index] = replace;
					}
					else
					{
						maze.MazeCells[row, index] = maze.MazeCells[row - 1, index];
					}
					iTween.MoveBy(maze.MazeCells[row, index], new Hashtable(iTweenParams));
				}
				break;
			case MoveDirection.South:
				iTweenParams.Add(iT.MoveBy.z, -maze.CellWidth);
				first = maze.MazeCells[0, index];
				last = maze.MazeCells[maze.Rows - 1, index];

				for (int row = 0; row < maze.Rows; ++row)
				{
					if (maze.MazeCells[row, index] == last)
					{
						GameObject replace = Instantiate(first);
						replace.transform.position = last.transform.position + new Vector3(0, 0, maze.CellWidth);
						//NetworkServer.Spawn(replace);
						maze.MazeCells[row, index] = replace;
					}
					else
					{
						maze.MazeCells[row, index] = maze.MazeCells[row + 1, index];
					}
					iTween.MoveBy(maze.MazeCells[row, index], new Hashtable(iTweenParams));
				}
				break;
			case MoveDirection.West:
				iTweenParams.Add(iT.MoveBy.x, -maze.CellWidth);
				first = maze.MazeCells[index, 0];
				last = maze.MazeCells[index, maze.Columns - 1];

				for (int col = 0; col < maze.Rows; ++col)
				{
					if (maze.MazeCells[index, col] == last)
					{
						GameObject replace = Instantiate(first);
						replace.transform.position = last.transform.position + new Vector3(maze.CellWidth, 0, 0);
						//NetworkServer.Spawn(replace);
						maze.MazeCells[index, col] = replace;
					}
					else
					{
						maze.MazeCells[index, col] = maze.MazeCells[index, col + 1];
					}
					iTween.MoveBy(maze.MazeCells[index, col], new Hashtable(iTweenParams));
				}
				break;
			case MoveDirection.East:
				iTweenParams.Add(iT.MoveBy.x, maze.CellWidth);
				first = maze.MazeCells[index, maze.Columns - 1];
				last = maze.MazeCells[index, 0];

				for (int col = maze.Rows - 1; col >= 0; --col)
				{
					if (maze.MazeCells[index, col] == last)
					{
						GameObject replace = Instantiate(first);
						replace.transform.position = last.transform.position - new Vector3(maze.CellWidth, 0, 0);
						maze.MazeCells[index, col] = replace;
					}
					else
					{
						maze.MazeCells[index, col] = maze.MazeCells[index, col - 1];
					}
					iTween.MoveBy(maze.MazeCells[index, col], new Hashtable(iTweenParams));
				}
				break;
		}

		if (first != null)
		{
			// Move and destroy first
			iTweenParams.Add(iT.MoveBy.oncomplete, "AfterCellsMoved");
			iTweenParams.Add(iT.MoveBy.oncompletetarget, this.gameObject);
			iTweenParams.Add(iT.MoveBy.oncompleteparams, first);
			iTween.MoveBy(first, new Hashtable(iTweenParams));
		}
	}

    
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Space)) {
			AlterMaze(moveDirection, cellIdx);
		}
	}

}
