using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;
namespace Initiation {
	public class InitiationMazeSpawner:NetworkBehaviour {
		public enum MazeGenerationAlgorithm {
			PureRecursive,
			RecursiveTree,
			RandomTree,
			OldestTree,
			RecursiveDivision,
		}


		[Header("Maze Generation Settings")]
		public MazeGenerationAlgorithm Algorithm = MazeGenerationAlgorithm.PureRecursive;
		public bool FullRandom = false;
		public int RandomSeed = 12345;
		public List<GameObject> Floor;
		public List<GameObject> Wall;
		public List<GameObject> Pillar;
		public List<GameObject> Goals;
		public int Rows = 8;
		public int Columns = 8;
		public float CellWidth = 4;
		public float CellHeight = 4;
		public bool AddGaps = false;




		[HideInInspector]
		public GameObject[,] MazeCells;


		public GameObject OuterWalls;
		public GameObject OuterWallsNorth;
		public GameObject OuterWallsSouth;
		public GameObject OuterWallsEast;
		public GameObject OuterWallsWest;

		[Header("Holes for multiple connections between multiple mazes")]
		public int CreateGapInNorthWall = -1;
		public int CreateGapInSouthWall = -1;
		public int CreateGapInEastWall = -1;
		public int CreateGapInWestWall = -1;

		// private variables
		private BasicMazeGenerator mMazeGenerator = null;

		private void Start()
		{
			if(isServer) {
				Generate();
			}
		}

		void Generate()
		{
			MazeCells = new GameObject[Rows,Columns];


			if(!FullRandom) {
				Random.InitState(RandomSeed);
			}

			switch(Algorithm) {
			case MazeGenerationAlgorithm.PureRecursive:
				mMazeGenerator = new RecursiveMazeGenerator(Rows,Columns);
				break;
			case MazeGenerationAlgorithm.RecursiveTree:
				mMazeGenerator = new RecursiveTreeMazeGenerator(Rows,Columns);
				break;
			case MazeGenerationAlgorithm.RandomTree:
				mMazeGenerator = new RandomTreeMazeGenerator(Rows,Columns);
				break;
			case MazeGenerationAlgorithm.OldestTree:
				mMazeGenerator = new OldestTreeMazeGenerator(Rows,Columns);
				break;
			case MazeGenerationAlgorithm.RecursiveDivision:
				mMazeGenerator = new DivisionMazeGenerator(Rows,Columns);
				break;
			}

			mMazeGenerator.GenerateMaze();

			for(int row = 0; row < Rows; row++) {
				for(int column = 0; column < Columns; column++) {
					float x = CellWidth / 2 - (Columns * CellWidth / 2) + transform.position.x + column * (CellWidth + (AddGaps ? .2f : 0));
					float y = transform.position.y;
					float z = CellWidth / 2 - (Columns * CellWidth / 2) + transform.position.z + row * (CellHeight + (AddGaps ? .2f : 0));
					MazeCell cell = mMazeGenerator.GetMazeCell(row,column);
					GameObject tmp;
					GameObject floor;
					//NetworkTransformChild tnc = gameObject.AddComponent<NetworkTransformChild>();
					int floorIdx = Random.Range(0,Floor.Count);
					floor = Instantiate(Floor[floorIdx],new Vector3(x,y,z),Quaternion.Euler(0,0,0)) as GameObject;
					floor.name = Floor[floorIdx].name;
					InitiationMazeTrap trap = floor.GetComponent<InitiationMazeTrap>();
					if(trap != null) {
						InitiationMazeTransformer.MoveDirection dir = (InitiationMazeTransformer.MoveDirection)Random.Range(0,4);
						trap.InitTrap(new Vector2Int(row,column),dir,this, GetComponent<InitiationMazeTransformer>(), true);
					}
	
				//tnc.target = floor.transform;
					NetworkServer.Spawn(floor);

					if(cell.WallLeft) {
						int wallIdx = Random.Range(0,Wall.Count);
						NetworkTransformChild parent;
						if(column == 0) {
							parent = OuterWallsEast.AddComponent<NetworkTransformChild>();
						} else {
							parent = floor.AddComponent<NetworkTransformChild>();
						}
						tmp = Instantiate(Wall[wallIdx]);
						tmp.GetComponent<InitiationMazeWall>().InitWall(Direction.Left,CellWidth,CellHeight,Wall[wallIdx].transform.position,x,y,z);
						tmp.name = Wall[wallIdx].name;

						parent.target = tmp.transform;
						NetworkServer.Spawn(tmp);
					}

					if(cell.WallRight) {
						int wallIdx = Random.Range(0,Wall.Count);
						NetworkTransformChild parent;
						if(column == Columns - 1) {
							parent = OuterWallsWest.AddComponent<NetworkTransformChild>();
						} else {
							parent = floor.AddComponent<NetworkTransformChild>();
						}
						tmp = Instantiate(Wall[wallIdx]);
						tmp.GetComponent<InitiationMazeWall>().InitWall(Direction.Right,CellWidth,CellHeight,Wall[wallIdx].transform.position,x,y,z);
						tmp.name = Wall[wallIdx].name;
						parent.target = tmp.transform;
						NetworkServer.Spawn(tmp);
					}
					if(cell.WallFront) {
						int wallIdx = Random.Range(0,Wall.Count);
						NetworkTransformChild parent;
						if(row == Rows - 1) {
							parent = OuterWallsNorth.AddComponent<NetworkTransformChild>();
						} else {
							parent = floor.AddComponent<NetworkTransformChild>();
						}
						tmp = Instantiate(Wall[wallIdx]);
						tmp.GetComponent<InitiationMazeWall>().InitWall(Direction.Front,CellWidth,CellHeight,Wall[wallIdx].transform.position,x,y,z);
						tmp.name = Wall[wallIdx].name;
						parent.target = tmp.transform;
						NetworkServer.Spawn(tmp);
					}

					if(cell.WallBack) {
						NetworkTransformChild parent;
						int wallIdx = Random.Range(0,Wall.Count);
						if(row == 0) {
							parent = OuterWallsSouth.AddComponent<NetworkTransformChild>();
						} else {
							parent = floor.AddComponent<NetworkTransformChild>();
						}
						tmp = Instantiate(Wall[wallIdx]);
						tmp.GetComponent<InitiationMazeWall>().InitWall(Direction.Back,CellWidth,CellHeight,Wall[wallIdx].transform.position,x,y,z);
						tmp.name = Wall[wallIdx].name;
						parent.target = tmp.transform;
						NetworkServer.Spawn(tmp);

					}
					if(cell.IsGoal) {
						int goalIdx = Random.Range(0,Goals.Count);
						tmp = Instantiate(Goals[goalIdx],new Vector3(x,y + 1,z),Quaternion.Euler(0,0,0)) as GameObject;
						tmp.name = Goals[goalIdx].name;
						//floor.AddComponent<NetworkTransformChild>().target = tmp.transform;
						NetworkServer.Spawn(tmp);
					}
					MazeCells[row,column] = floor;
				}
			}

			if(Pillar != null) {
				for(int row = 0; row < Rows + 1; row++) {
					for(int column = 0; column < Columns + 1; column++) {
						float x = CellWidth / 2 - (Columns * CellWidth / 2) + transform.position.x + column * (CellWidth + (AddGaps ? .2f : 0));
						float y = transform.position.y;
						float z = CellWidth / 2 - (Columns * CellWidth / 2) + transform.position.z + row * (CellHeight + (AddGaps ? .2f : 0));
						int pillarIdx = Random.Range(0,Pillar.Count);

						GameObject tmp = Instantiate(Pillar[pillarIdx],new Vector3(x - CellWidth / 2,y,z - CellHeight / 2),Quaternion.identity) as GameObject;
						NetworkTransformChild tnc = gameObject.AddComponent<NetworkTransformChild>();
						tmp.name = Pillar[pillarIdx].name;
						tnc.target = tmp.transform;
						NetworkServer.Spawn(tmp);
					}
				}
			}

			if(CreateGapInNorthWall != -1 && CreateGapInNorthWall < Columns) {
				NetworkTransformChild ntc = OuterWallsNorth.GetComponents<NetworkTransformChild>()[CreateGapInNorthWall];
				GameObject obj = ntc.target.gameObject;
				Destroy(ntc);
				NetworkServer.Destroy(obj);
				Destroy(obj);
			}
			if(CreateGapInSouthWall != -1 && CreateGapInSouthWall < Columns) {
				NetworkTransformChild ntc = OuterWallsSouth.GetComponents<NetworkTransformChild>()[CreateGapInSouthWall];
				GameObject obj = ntc.target.gameObject;
				Destroy(ntc);
				NetworkServer.Destroy(obj);
				Destroy(obj);
			}

			if(CreateGapInEastWall != -1 && CreateGapInEastWall < Rows) {
				NetworkTransformChild ntc = OuterWallsEast.GetComponents<NetworkTransformChild>()[CreateGapInEastWall];
				GameObject obj = ntc.target.gameObject;
				Destroy(ntc);
				NetworkServer.Destroy(obj);
				Destroy(obj);
			}

			if(CreateGapInWestWall != -1 && CreateGapInWestWall < Rows) {
				NetworkTransformChild ntc = OuterWallsWest.GetComponents<NetworkTransformChild>()[CreateGapInWestWall];
				GameObject obj = ntc.target.gameObject;
				Destroy(ntc);
				NetworkServer.Destroy(obj);
				Destroy(obj);
			}
		}
	}
}