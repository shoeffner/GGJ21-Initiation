using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

namespace Initiation {
	[RequireComponent(typeof(InitiationMazeSpawner))]
	public class InitiationMazeTransformer:MonoBehaviour {
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

		private Dictionary<string,object> movementParams = new Dictionary<string,object>()
		{
		{iT.MoveBy.time, 1.0f},
		{iT.MoveBy.easetype, iTween.EaseType.linear},
		{iT.MoveBy.space, Space.World}
	};



		void Start()
		{
			maze = GetComponent<InitiationMazeSpawner>();

			mazeComponentsDict = new Dictionary<string,GameObject>();
			foreach(GameObject obj in mazeComponents) {
				mazeComponentsDict.Add(obj.name,obj);
			}
		}

		public void AfterCellsMoved(GameObject toDelete = null)
		{
			if(toDelete != null) {
				NetworkTransformChild[] ntcs = toDelete.GetComponents<NetworkTransformChild>();
				foreach(NetworkTransformChild ntc in ntcs) {
					NetworkServer.Destroy(ntc.target.gameObject);
				}
				NetworkServer.Destroy(toDelete);


				moving = false;
			}
		}



		GameObject CreateCell(GameObject cell,Vector3 pos)
		{

			GameObject newCell = Instantiate(mazeComponentsDict[cell.name]);
			newCell.transform.position = pos;
			newCell.name = cell.name;

			InitiationMazeTrap trap = cell.GetComponent<InitiationMazeTrap>();
			if(trap != null) {
				newCell.GetComponent<InitiationMazeTrap>().InitTrap(trap.cellId,trap.direction,trap.maze,trap.transformer,trap.isActivated);
			}


			NetworkServer.Spawn(newCell);
			NetworkTransformChild[] ntcs = cell.GetComponents<NetworkTransformChild>();
			foreach(NetworkTransformChild ntc in ntcs) {
				GameObject child = Instantiate(mazeComponentsDict[ntc.target.name]);
				child.GetComponent<InitiationMazeWall>()?.InitWall(ntc.target.GetComponent<InitiationMazeWall>().direction,
					maze.CellWidth,maze.CellHeight,Vector3.zero,pos.x,pos.y,pos.z);

				

				//child.transform.position = ntc.target.position;
				//child.transform.rotation = ntc.target.rotation;			
				child.name = ntc.target.name;
				NetworkTransformChild newNTC = newCell.AddComponent<NetworkTransformChild>();
				newNTC.target = child.transform;
				NetworkServer.Spawn(child);
			}
			return newCell;
		}

		/**
		* index is always from W -> E for N and S
		* index is always from S -> N for E and W
		*/
		public void AlterMaze(MoveDirection moveDirection,int index)
		{
			if(moving) {
				return;
			}
			if(index < 0) {
				Debug.LogError("Cannot move maze for negative indices.");
				return;
			}
			switch(moveDirection) {
			case MoveDirection.North:
			case MoveDirection.South:
				if(index > maze.Rows - 1) {
					Debug.LogError($"Only {maze.Rows} available (idx {index} too big)");
					return;
				}
				break;
			case MoveDirection.West:
			case MoveDirection.East:
				if(index > maze.Columns - 1) {
					Debug.LogError($"Only {maze.Columns} available (idx {index} too big)");
					return;
				}
				break;
			}

			moving = true;

			Dictionary<string,object> iTweenParams = new Dictionary<string,object>(movementParams);

			GameObject first = null;
			GameObject last;
			switch(moveDirection) {
			case MoveDirection.North:

				first = maze.MazeCells[maze.Rows - 1,index];
				last = maze.MazeCells[0,index];

				iTweenParams.Add(iT.MoveBy.z,maze.CellWidth);

				for(int row = maze.Rows - 1; row >= 0; --row) {
					if(maze.MazeCells[row,index] == last) {
						GameObject replace = CreateCell(first,last.transform.position - new Vector3(0,0,maze.CellWidth));
						maze.MazeCells[row,index] = replace;
					} else {
						maze.MazeCells[row,index] = maze.MazeCells[row - 1,index];
					}

					//Hashtable ht = new Hashtable(iTweenParams);
					iTween.MoveBy(maze.MazeCells[row,index],new Hashtable(iTweenParams));
					NetworkTransformChild[] ntcs = maze.MazeCells[row,index].GetComponents<NetworkTransformChild>();
					foreach(NetworkTransformChild ntc in ntcs) {
						iTween.MoveBy(ntc.target.gameObject,new Hashtable(iTweenParams));
					}
				}
				break;
			case MoveDirection.South:
				iTweenParams.Add(iT.MoveBy.z,-maze.CellWidth);
				first = maze.MazeCells[0,index];
				last = maze.MazeCells[maze.Rows - 1,index];

				for(int row = 0; row < maze.Rows; ++row) {
					if(maze.MazeCells[row,index] == last) {
						GameObject replace = CreateCell(first,last.transform.position + new Vector3(0,0,maze.CellWidth));
						maze.MazeCells[row,index] = replace;
					} else {
						maze.MazeCells[row,index] = maze.MazeCells[row + 1,index];
					}
					Hashtable ht = new Hashtable(iTweenParams);
					iTween.MoveBy(maze.MazeCells[row,index],ht);
					NetworkTransformChild[] ntcs = maze.MazeCells[row,index].GetComponents<NetworkTransformChild>();
					foreach(NetworkTransformChild ntc in ntcs) {
						iTween.MoveBy(ntc.target.gameObject,ht);
					}

				}
				break;
			case MoveDirection.West:
				iTweenParams.Add(iT.MoveBy.x,-maze.CellWidth);
				first = maze.MazeCells[index,0];
				last = maze.MazeCells[index,maze.Columns - 1];

				for(int col = 0; col < maze.Rows; ++col) {
					if(maze.MazeCells[index,col] == last) {
						GameObject replace = CreateCell(first,last.transform.position + new Vector3(maze.CellWidth,0,0));
						maze.MazeCells[index,col] = replace;
					} else {
						maze.MazeCells[index,col] = maze.MazeCells[index,col + 1];
					}
					Hashtable ht = new Hashtable(iTweenParams);
					iTween.MoveBy(maze.MazeCells[index,col],ht);
					NetworkTransformChild[] ntcs = maze.MazeCells[index,col].GetComponents<NetworkTransformChild>();
					foreach(NetworkTransformChild ntc in ntcs) {
						iTween.MoveBy(ntc.target.gameObject,ht);
					}
				}
				break;
			case MoveDirection.East:
				iTweenParams.Add(iT.MoveBy.x,maze.CellWidth);
				first = maze.MazeCells[index,maze.Columns - 1];
				last = maze.MazeCells[index,0];

				for(int col = maze.Rows - 1; col >= 0; --col) {
					if(maze.MazeCells[index,col] == last) {
						GameObject replace = CreateCell(first,last.transform.position - new Vector3(maze.CellWidth,0,0));
						maze.MazeCells[index,col] = replace;
					} else {
						maze.MazeCells[index,col] = maze.MazeCells[index,col - 1];
					}
					Hashtable ht = new Hashtable(iTweenParams);
					iTween.MoveBy(maze.MazeCells[index,col],ht);
					NetworkTransformChild[] ntcs = maze.MazeCells[index,col].GetComponents<NetworkTransformChild>();
					foreach(NetworkTransformChild ntc in ntcs) {
						iTween.MoveBy(ntc.target.gameObject,ht);
					}
				}
				break;
			}

			if(first != null) {
				// Move and destroy first

				NetworkTransformChild[] ntcs = first.GetComponents<NetworkTransformChild>();

				foreach(NetworkTransformChild ntc in ntcs) {
					iTween.MoveBy(ntc.target.gameObject,new Hashtable(iTweenParams));
				}

				iTweenParams.Add(iT.MoveBy.oncomplete,"AfterCellsMoved");
				iTweenParams.Add(iT.MoveBy.oncompletetarget,this.gameObject);
				iTweenParams.Add(iT.MoveBy.oncompleteparams,first);

				iTween.MoveBy(first,new Hashtable(iTweenParams));
			}
		}


		void Update()
		{
			if(Input.GetKeyDown(KeyCode.X)) {
				AlterMaze(moveDirection,cellIdx);
			}
		}

	}
}