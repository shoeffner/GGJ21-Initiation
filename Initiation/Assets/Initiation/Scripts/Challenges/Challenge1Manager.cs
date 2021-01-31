using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
public class Challenge1Manager : NetworkBehaviour
{
    public List<ChallengeTarget> targets;

    public GameObject rune;
	[SyncVar]
    bool challengeCompleted;

    void Start()
    {
		if(!isServer) {
            return;
		}
        rune.SetActive(false);
    }

	[ClientRpc]
	public void RpcActivateRune()
	{
        rune.SetActive(true);
        Hashtable ht = new Hashtable();
        ht.Add(iT.ScaleFrom.y,-4);
        ht.Add(iT.ScaleFrom.time,2);
        iTween.ScaleFrom(rune,ht);
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

        bool complete = targets.TrueForAll(t => t.isComplete);
        print($"challenge complete {complete}");
		if(complete) {
            RpcActivateRune();
            challengeCompleted = complete;
		}
    }
}
