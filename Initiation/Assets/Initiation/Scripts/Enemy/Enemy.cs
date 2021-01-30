using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using Mirror;

namespace Initiation {
	[RequireComponent(typeof(CharacterStats))]
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Animator))]
    
    public class Enemy:NetworkBehaviour {


		public LayerMask mask;
		StateMachine stateMachine;

		public NavMeshAgent navMeshAgent { get; private set; }
		public CharacterStats stats { get; private set; }
		public Animator animator { get; private set; }
		

		
		public float senseRadius = 10;
		public float attackRange = 1;
		public float attackSpeed = 1;

		public CharacterStats target; // { get; private set; }



		private void OnTriggerEnter(Collider other)
		{
			if(other.CompareTag("Player")) {
				SetTarget(other.GetComponent<CharacterStats>());
			}
		}


		public void SetTarget(CharacterStats target)
		{
			this.target = target;
			animator.SetBool("HasTarget", target != null);
		}

		public void Attack()
		{
			// Do attack here
		}

		void Start()
		{
			stateMachine = GetComponent<StateMachine>();


			stateMachine.SetStates(new Dictionary<Type,BaseState>() {
				{ typeof(IdleState), new IdleState(this) },
				{ typeof(ChaseState), new ChaseState(this) },
				{ typeof(AttackState), new AttackState(this) }
			});


			navMeshAgent = GetComponent<NavMeshAgent>();
			navMeshAgent.updateRotation = false;
			stats = GetComponent<CharacterStats>();
			animator = GetComponent<Animator>();

		}

		


		// Update is called once per frame
		void Update()
		{
			if(target) {

				Vector3 view = target.transform.position - transform.position;
				view.y = 0f;
				transform.rotation = Quaternion.LookRotation(view);
				 
			}
			
		}
	}
}

