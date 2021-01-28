using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[RequireComponent(typeof(InitiationMazeSpawner))]
public class InitiationMazeTransformer : MonoBehaviour
{
    InitiationMazeSpawner maze;
    List<GameObject> cellsToMove;
    public enum MoveDirection {
		North,
		South,
		East,
		West
	}

    public MoveDirection moveDirection;
    public int cellIdx = 0;
    // Start is called before the first frame update
    void Start()
    {
        maze = GetComponent<InitiationMazeSpawner>();
    }


    public void AfterCellsMoved() {
        print("cells moved");
        if(moveDirection == MoveDirection.North) {
            maze.MazeCells[cellIdx].Remove(cellsToMove.Last());
            Destroy(cellsToMove.Last());
        } else if(moveDirection == MoveDirection.South) {
            maze.MazeCells[cellIdx].Remove(cellsToMove.First());
            Destroy(cellsToMove.First());
        } else if(moveDirection == MoveDirection.East) {
            Destroy(cellsToMove.First());
        } else if(moveDirection == MoveDirection.West) {
            
        }

    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKeyDown(KeyCode.Space)) {
            cellsToMove = new List<GameObject>();

            if(moveDirection == MoveDirection.North || moveDirection == MoveDirection.South) {
                cellsToMove.AddRange(maze.MazeCells[cellIdx]);
                if(moveDirection == MoveDirection.North) {
                    GameObject newCell = Instantiate(cellsToMove.Last());
                    newCell.transform.position = cellsToMove.First().transform.position - new Vector3(0,0,maze.CellWidth);
                    cellsToMove.Insert(0,newCell);
                    maze.MazeCells[cellIdx].Insert(0,newCell);
                } else {
                    GameObject newCell = Instantiate(cellsToMove.First());
                    newCell.transform.position = cellsToMove.Last().transform.position + new Vector3(0,0,maze.CellWidth);
                    cellsToMove.Add(newCell);
                    maze.MazeCells[cellIdx].Add(newCell);
                }
            } else {
                for(int i = 0; i < maze.Columns; i++) {
                    cellsToMove.Add(maze.MazeCells[i][cellIdx]);
                }
                if(moveDirection == MoveDirection.East) {

                    GameObject newCell = Instantiate(cellsToMove.Last());
                    newCell.transform.position = cellsToMove.First().transform.position + new Vector3(maze.CellWidth,0,0);
                    cellsToMove.Add(newCell);
                    maze.MazeCells[0][cellIdx] = newCell;

                } else {
                }
            }


            Hashtable ht = new Hashtable();
            if(moveDirection == MoveDirection.North) {
                ht.Add(iT.MoveBy.z,maze.CellWidth);
            } else if(moveDirection == MoveDirection.South) {
                ht.Add(iT.MoveBy.z,-maze.CellWidth);
            } else if(moveDirection == MoveDirection.East) {
                ht.Add(iT.MoveBy.x,-maze.CellWidth);
            } else if(moveDirection == MoveDirection.West) {
                ht.Add(iT.MoveBy.x, maze.CellWidth);
            }

            ht.Add(iT.MoveBy.time, 1.0f);

			
			ht.Add(iT.MoveBy.easetype,iTween.EaseType.linear);

            for(int i = 0; i < cellsToMove.Count; i++) {
				if(i == cellsToMove.Count - 1) {
					ht.Add(iT.MoveBy.oncomplete,"AfterCellsMoved");
					ht.Add(iT.MoveBy.oncompletetarget,gameObject);
				}
				iTween.MoveBy(cellsToMove[i],ht);
            }
            
        }
    }

}
