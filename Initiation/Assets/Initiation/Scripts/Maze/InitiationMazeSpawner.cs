using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitiationMazeSpawner : MonoBehaviour
{
	public enum MazeGenerationAlgorithm {
		PureRecursive,
		RecursiveTree,
		RandomTree,
		OldestTree,
		RecursiveDivision,
	}

	public MazeGenerationAlgorithm Algorithm = MazeGenerationAlgorithm.PureRecursive;
	public bool FullRandom = false;
	public int RandomSeed = 12345;
	public GameObject Floor = null;
	public GameObject Wall = null;
	public GameObject Pillar = null;
	public int Rows = 5;
	public int Columns = 5;
	public float CellWidth = 4;
	public float CellHeight = 4;
	public bool AddGaps = false;
	public GameObject GoalPrefab = null;

	private BasicMazeGenerator mMazeGenerator = null;

	public GameObject[,] MazeCells;

	void Start()
	{
		MazeCells = new GameObject[Rows, Columns];

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
				float x = column * (CellWidth + (AddGaps ? .2f : 0));
				float z = row * (CellHeight + (AddGaps ? .2f : 0));
				MazeCell cell = mMazeGenerator.GetMazeCell(row,column);
				GameObject tmp;
				GameObject floor;
				floor = Instantiate(Floor,new Vector3(x,0,z),Quaternion.Euler(0,0,0)) as GameObject;
				floor.transform.parent = transform;
				if(cell.WallRight) {
					tmp = Instantiate(Wall,new Vector3(x + CellWidth / 2,0,z) + Wall.transform.position,Quaternion.Euler(0,90,0)) as GameObject;// right
					tmp.transform.parent = floor.transform;
				}
				if(cell.WallFront) {
					tmp = Instantiate(Wall,new Vector3(x,0,z + CellHeight / 2) + Wall.transform.position,Quaternion.Euler(0,0,0)) as GameObject;// front
					tmp.transform.parent = floor.transform;
				}
				if(cell.WallLeft) {
					tmp = Instantiate(Wall,new Vector3(x - CellWidth / 2,0,z) + Wall.transform.position,Quaternion.Euler(0,270,0)) as GameObject;// left
					tmp.transform.parent = floor.transform;
				}
				if(cell.WallBack) {
					tmp = Instantiate(Wall,new Vector3(x,0,z - CellHeight / 2) + Wall.transform.position,Quaternion.Euler(0,180,0)) as GameObject;// back
					tmp.transform.parent = floor.transform;
				}
				if(cell.IsGoal && GoalPrefab != null) {
					tmp = Instantiate(GoalPrefab,new Vector3(x,1,z),Quaternion.Euler(0,0,0)) as GameObject;
					tmp.transform.parent = floor.transform;
				}
				MazeCells[row, column] = floor;
			}
		}

		if (Pillar != null) {
			for (int row = 0; row < Rows + 1; row++) {
				for (int column = 0; column < Columns + 1; column++) {
					float x = column * (CellWidth + (AddGaps ? .2f : 0));
					float z = row * (CellHeight + (AddGaps ? .2f : 0));
					GameObject tmp = Instantiate(Pillar,new Vector3(x - CellWidth / 2,0,z - CellHeight / 2),Quaternion.identity) as GameObject;
					tmp.transform.parent = transform;
				}
			}
		}
	}
}
