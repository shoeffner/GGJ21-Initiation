using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InitiationMazeSpawner))]
public class InitiationMazeTransformer : MonoBehaviour
{
    InitiationMazeSpawner maze;

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
	}

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKeyDown(KeyCode.Space)) {
            List<GameObject> cellsToMove = new List<GameObject>();

            if(moveDirection == MoveDirection.North || moveDirection == MoveDirection.South) {
                cellsToMove.AddRange(maze.MazeCells[cellIdx]);
            } else {
                for(int i = 0; i < maze.Columns; i++) {
                    cellsToMove.Add(maze.MazeCells[i][cellIdx]);
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
