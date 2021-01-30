using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Initiation {

	public class IdleState:BaseState {

		Enemy enemy;

		public IdleState(Enemy enemy) : base(enemy.gameObject)
		{
			this.enemy = enemy;
		}

		public override Type Tick()
		{
			if(enemy.target) {
				float dist = Vector3.Distance(enemy.transform.position,enemy.target.transform.position);
				
				if(dist > enemy.attackRange) {
					return typeof(ChaseState);
				} else {
					return typeof(AttackState);
				}
				
			}

			return null;
		}

		public override void Start()
		{
			base.Start();
			enemy.SetTarget(null);
			enemy.animator.SetBool("Move",false);
		}

		public override void Finish()
		{
			base.Finish();
		}

	}
}