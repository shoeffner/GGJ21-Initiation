using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeTarget : MonoBehaviour
{

    public int id;
    public bool isComplete = false;
    public float radius = 1;
    public Challenge1Manager manager;

    public ParticleSystem particle;

    void Start()
    {
        particle.Stop();
    }

	private void OnTriggerStay(Collider other)
	{
        print($"{other.name} stay in target {id}");
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
