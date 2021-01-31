using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class ChallengeTarget : NetworkBehaviour
{

    public int id;
    
    [SyncVar]
    public bool isComplete = false;
    
    public Challenge1Manager manager;
    public ParticleSystem particle;

    void Start()
    {
        particle.Stop();
    }

	private void OnTriggerStay(Collider other)
	{
        ChallengeCrate cc = other.GetComponent<ChallengeCrate>();
        if(cc) {
            if(cc.id == id) {
                isComplete = true;
                particle.Play();
            }
        }
    }

	private void OnTriggerExit(Collider other)
	{
        ChallengeCrate cc = other.GetComponent<ChallengeCrate>();
        if(cc) {
            if(cc.id == id) {
                isComplete = false;
                particle.Stop();
            }
        }
    }

}
