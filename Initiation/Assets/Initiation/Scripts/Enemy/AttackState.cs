using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Initiation {
	public class AttackState:BaseState {
		Enemy enemy;

		float lastAttackTime;

		public AttackState(Enemy enemy) : base(enemy.gameObject)
		{
			this.enemy = enemy;
		}

		public override Type Tick()
		{
			float dist = Vector3.Distance(enemy.transform.position,enemy.target.transform.position);
			if(dist > enemy.attackRange) {
				return typeof(ChaseState);
			}

			
			// do cooldown
			if(Time.time - lastAttackTime > enemy.attackSpeed) {
				enemy.animator.SetBool("Attack",true);
				lastAttackTime = Time.time;

				// TODO: check hit and deal damage
				

			} else {
				enemy.animator.SetBool("Attack",false);
			}
			Ray ray = new Ray(enemy.transform.position,enemy.transform.forward * enemy.attackRange);
			Debug.DrawRay(ray.origin,ray.direction);
			RaycastHit hit;
			if(Physics.Raycast(ray,out hit)) {
				Debug.Log(hit.transform.name);
			}

			return null;
		}

		public override void Start()
		{
			enemy.animator.SetBool("Attack",true);
			base.Start();
		}

		public override void Finish()
		{
			enemy.animator.SetBool("Attack",false);
			base.Finish();
		}
	}

}

