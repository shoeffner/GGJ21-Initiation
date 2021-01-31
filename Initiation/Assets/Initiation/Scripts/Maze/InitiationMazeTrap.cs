using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Initiation {

    public class InitiationMazeTrap:NetworkBehaviour {

        public Sprite[] sprites;

        public SpriteRenderer directionSprite;

		[SyncVar]
        public Vector2Int cellId;
        [SyncVar]
        public InitiationMazeTransformer.MoveDirection direction;
        [SyncVar]
        public InitiationMazeSpawner maze;
        [SyncVar]
        public InitiationMazeTransformer transformer;
        [SyncVar]
        public bool isActivated = true;


		
        public void InitTrap(Vector2Int cellId,
			InitiationMazeTransformer.MoveDirection direction,
			InitiationMazeSpawner maze,
            InitiationMazeTransformer transformer,
            bool isActivated) {
            this.cellId = cellId;
            this.direction = direction;
            this.maze = maze;
            this.transformer = transformer;
            this.isActivated = isActivated;


            directionSprite.sprite = sprites[(int)direction];

        }


        void OnTriggerEnter(Collider other)
        {
            if(isActivated &&
				(other.CompareTag("Player") || other.CompareTag("Enemy"))
				) {
                int id = -1;
                if(direction == InitiationMazeTransformer.MoveDirection.North || direction == InitiationMazeTransformer.MoveDirection.South) {
                    id = cellId.y;
                } else {
                    id = cellId.x;
                }
                transformer.AlterMaze(direction,id);
                //directionSprite.sprite = sprites[(int)direction];
            }
        }
    }

}