using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Challeng2Manager : Challenge1Manager
{
    public List<CharacterStats> targetCharacters;

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

        bool complete = targetCharacters.TrueForAll(t => t.dead);
        if(complete) {
            RpcActivateRune();
            challengeCompleted = complete;
        }
    }
}
