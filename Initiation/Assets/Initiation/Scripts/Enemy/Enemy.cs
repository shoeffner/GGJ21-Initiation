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

		public Transform attackOrigin;

		public float senseRadius = 10;
		public float attackRange = 1;
		public float attackSpeed = 1;
		public int attackDamage = 2;

		public CharacterStats target; // { get; private set; }

		bool isDying;

		//private void OnTriggerEnter(Collider other)
		//{
		//	if(other.CompareTag("Player")) {
		//		SetTarget(other.GetComponent<CharacterStats>());
		//	}
		//}


		public void SetTarget(CharacterStats target)
		{
			this.target = target;
			bool hasTarget = target != null;
			navMeshAgent.updateRotation = hasTarget;
			animator.SetBool("HasTarget",hasTarget);
		}


		void Start()
		{
			stateMachine = GetComponent<StateMachine>();


			stateMachine.SetStates(new Dictionary<Type,BaseState>() {
				{ typeof(IdleState), new IdleState(this) },
				{ typeof(ChaseState), new ChaseState(this) },
				{ typeof(AttackState), new AttackState(this) },
				{ typeof(VictoryState), new VictoryState(this) }
			});


			navMeshAgent = GetComponent<NavMeshAgent>();
			navMeshAgent.updateRotation = false;
			stats = GetComponent<CharacterStats>();
			animator = GetComponent<Animator>();

			stats.OnTakeDammage += Stats_OnTakeDammage;
			stats.OnDie += Stats_OnDie;

		}

		IEnumerator AsyncDie() {
			yield return new WaitForSeconds(2);
			Destroy(gameObject,2);
			NetworkServer.Destroy(gameObject);
		}

		private void Stats_OnDie(CharacterStats obj)
		{
			animator.SetBool("IsDead", true);
			if(!isDying) {
				isDying = true;
				StartCoroutine(AsyncDie());
			}
			
			
		}

		private void Stats_OnTakeDammage(CharacterStats obj)
		{
			animator.SetTrigger("GetHit");
		}

		private void OnDestroy()
		{
			stats.OnTakeDammage -= Stats_OnTakeDammage;
			stats.OnDie -= Stats_OnDie;
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

