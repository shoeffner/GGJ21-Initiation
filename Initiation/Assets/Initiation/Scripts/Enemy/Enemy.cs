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

  //      public enum EnemyState {
		//	Idle,
		//	Chase,
		//	Attack
		//}
		//public EnemyState state;


		//NavMeshAgent navMeshAgent;
  //      CharacterStats stats;
  //      Animator animator;


		

		//public float senseRadius = 10;
  //      public float attackRange = 1;

  //      CharacterStats target;



		//private void OnTriggerEnter(Collider other)
		//{
		//	if(other.CompareTag("Player")) {
		//		target = other.GetComponent<CharacterStats>();
		//		SetState(EnemyState.Chase);
		//	}
		//}


		//public void SetTarget(CharacterStats target)
		//{
  //          this.target = target;
		//}

		//public void Attack()
		//{
		//	// Do attack here
		//}

		//void Start()
  //      {
  //          navMeshAgent = GetComponent<NavMeshAgent>();
  //          stats = GetComponent<CharacterStats>();
  //          animator = GetComponent<Animator>();
			
		//}

		//void SetState(EnemyState enemyState)
		//{
		//	state = enemyState;
		//	switch(state) {
		//	case EnemyState.Idle:
		//		animator.SetTrigger("Idle");
		//		break;
		//	case EnemyState.Attack:
		//		animator.SetTrigger("Attack");
		//		break;
		//	case EnemyState.Chase:
		//		animator.SetTrigger("Move");
		//		break;
		//	}
			
		//}

		

		//// Update is called once per frame
		//void Update()
  //      {
		//	if(target != null) {
		//		navMeshAgent.SetDestination(target.transform.position);
		//		if(navMeshAgent.remainingDistance < attackRange) {
		//			SetState(EnemyState.Attack);
		//		}

		//	} else {
		//		SetState(EnemyState.Idle);
		//	}
  //      }
	}
}

