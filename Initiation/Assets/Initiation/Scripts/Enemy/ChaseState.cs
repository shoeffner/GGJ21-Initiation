using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Initiation {


	public class ChaseState:BaseState {
		Enemy enemy;

		public ChaseState(Enemy enemy) : base(enemy.gameObject)
		{
			this.enemy = enemy;
		}

		public override Type Tick()
		{
			if(enemy.target == null) {
				return typeof(IdleState);
			}
			float dist = Vector3.Distance(enemy.transform.position,enemy.target.transform.position);			
			if(dist > enemy.attackRange) {
				enemy.navMeshAgent.SetDestination(enemy.target.transform.position);
				enemy.animator.SetFloat("DirX",enemy.navMeshAgent.velocity.x / enemy.navMeshAgent.speed);
				enemy.animator.SetFloat("DirZ",enemy.navMeshAgent.velocity.z / enemy.navMeshAgent.speed);
			} else {
				return typeof(AttackState);
			}

			return null;
		}

		public override void Start()
		{
			enemy.animator.SetBool("Move",true);
			base.Start();
		}

		public override void Finish()
		{
			enemy.animator.SetBool("Move",false);
			base.Finish();
		}
	}
}
