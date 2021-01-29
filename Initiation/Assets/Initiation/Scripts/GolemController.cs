using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

[RequireComponent(typeof(NavMeshAgent)),
 RequireComponent(typeof(CharacterStats))]
public class GolemController : NetworkBehaviour
{ 
    enum GolemState
    {
        IDLE,
        SEARCH,
        FOLLOW_TO_RANGE,
        ATTACK,
        DEAD,
    }

    private NavMeshAgent navMeshAgent;
    public Animator animator;
    private GolemState state = GolemState.IDLE;
    public GameObject projectile;
    public CharacterStats characterStats;

    [Header("Animation parameters")]
    public string isCriticallyHit = "isCriticallyHit";
    public string isWalking = "isWalking";
    public string isIdle = "idle";
    public string isAttacking = "isAttacking";
    public string isDead = "isDead";

    void Act()
    {
        switch (state)
        {
            case GolemState.IDLE:
                break;
            case GolemState.SEARCH:
                break;
            case GolemState.FOLLOW_TO_RANGE:
                break;
            case GolemState.ATTACK:
                break;
            case GolemState.DEAD:
                navMeshAgent.ResetPath();
                break;
        }
    }

    void Start()
    {
        if (navMeshAgent == null)
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
        }
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
        if (characterStats == null)
        {
            characterStats = GetComponentInChildren<CharacterStats>();
        }
        if (isLocalPlayer)
        {
            return;
        }
        navMeshAgent.SetDestination(Vector3.zero);
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            return;
        }
        if (characterStats.isDead)
        {
            state = GolemState.DEAD;
        }
        Act();
    }

    void StopBeingHit()
    {
        animator.SetBool(isCriticallyHit, false);
        animator.SetBool(isWalking, false);
        animator.SetBool(isAttacking, false);
        animator.SetBool(isDead, GetComponent<CharacterStats>()?.isDead ?? false);
        animator.SetBool(isIdle, true);
    }

    void ReleaseProjectile()
    {
        GameObject rock = Instantiate(projectile, transform.position, Quaternion.identity);
        rock.GetComponent<Rigidbody>().velocity = rock.transform.up * 50;
        NetworkServer.Spawn(rock);
    }
}
