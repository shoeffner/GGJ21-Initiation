using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Initiation {

    public class InitiationMazeTrap:MonoBehaviour {

        public Vector2Int cellId;
        public InitiationMazeTransformer.MoveDirection direction;
        public InitiationMazeSpawner maze;
        public InitiationMazeTransformer transformer;
        public bool isActivated = true;


        void OnTriggerEnter(Collider other)
        {
            if(isActivated && other.CompareTag("Player")) {
                int id = -1;
                if(direction == InitiationMazeTransformer.MoveDirection.North || direction == InitiationMazeTransformer.MoveDirection.South) {
                    id = cellId.x;
                } else {
                    id = cellId.y;
                }
                transformer.AlterMaze(direction,id);
            }
        }
    }

}