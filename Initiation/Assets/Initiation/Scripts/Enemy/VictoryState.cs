using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Initiation {

    public class VictoryState:BaseState {

		Enemy enemy;

		public VictoryState(Enemy enemy) : base(enemy.gameObject)
		{
			this.enemy = enemy;
		}

		public override Type Tick()
		{
			if(enemy.target) {
				if(enemy.target.dead) {
					return null;
				}

			}

			return typeof(IdleState);
		}

		public override void Start()
		{
			base.Start();
			enemy.SetTarget(null);
			enemy.animator.SetTrigger("Victory");
		}

		public override void Finish()
		{
			base.Finish();
		}
	}

}