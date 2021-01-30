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
			if(enemy.target.dead) {
				return typeof(VictoryState);
			}

			
			// do cooldown
			if(Time.time - lastAttackTime > enemy.attackSpeed) {
				enemy.animator.SetTrigger("Attack");
				lastAttackTime = Time.time;

				Ray ray = new Ray(enemy.attackOrigin.position,enemy.attackOrigin.forward * enemy.attackRange);
				Debug.DrawRay(ray.origin,ray.direction);
				RaycastHit hit;
				if(Physics.Raycast(ray,out hit, enemy.attackRange, enemy.mask)) {
					Debug.Log(hit.transform.name);
					enemy.target.CmdTakeDamage(enemy.attackDamage);
				}
			}
			
			

			return null;
		}

		public override void Start()
		{
			base.Start();
		}

		public override void Finish()
		{
			//enemy.animator.SetBool("Attack",false);
			base.Finish();
		}
	}

}

