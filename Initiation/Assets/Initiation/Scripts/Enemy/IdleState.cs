using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Initiation {

	public class IdleState:BaseState {

		Enemy enemy;
		public float ThetaScale = 0.05f;
		float rayOffsetScale = 1;

		float rayOffset;

		public IdleState(Enemy enemy) : base(enemy.gameObject)
		{
			this.enemy = enemy;
		}

		public override Type Tick()
		{
			if(!enemy.target) {

				float Theta = rayOffset;
				float Size = (int)((1f / ThetaScale) + 1f);
				

				for(int i = 0; i < Size; i++) {
					Theta += (2.0f * Mathf.PI * ThetaScale);
					float x = enemy.senseRadius * Mathf.Cos(Theta);
					float z = enemy.senseRadius * Mathf.Sin(Theta);

					Ray ray = new Ray(enemy.attackOrigin.position, new Vector3(x,0,z) * enemy.senseRadius);
					Debug.DrawRay(ray.origin,ray.direction * enemy.senseRadius);
					RaycastHit hit;
					if(Physics.Raycast(ray,out hit,enemy.attackRange,enemy.mask)) {
						Debug.Log($"Enemy sensed {hit.transform.name}");
						enemy.SetTarget(hit.transform.GetComponent<CharacterStats>());
					}
				}

				rayOffset += Time.deltaTime * rayOffsetScale;
			}


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