using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Challeng2Manager : Challenge1Manager
{
    public new List<CharacterStats> targets;

	// Start is called before the first frame update
    void Start()
    {
        if(!isServer) {
            return;
        }
        rune.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(!isServer) {
            return;
        }

        if(challengeCompleted) {
            return;
        }

        bool complete = targets.TrueForAll(t => t.dead);
        if(complete) {
            RpcActivateRune();
            challengeCompleted = complete;
        }
    }
}
