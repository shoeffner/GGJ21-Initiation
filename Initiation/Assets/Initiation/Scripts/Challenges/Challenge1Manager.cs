using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
public class Challenge1Manager : NetworkBehaviour
{
    public List<ChallengeTarget> targets;

    public GameObject rune;
    bool challengeCompleted;
    void Start()
    {
        rune.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(challengeCompleted) {
            return;
		}
        bool complete = targets.TrueForAll(t => t.isComplete);
        print($"challenge complete {complete}");
		if(complete) {
            rune.SetActive(true);
            //Hashtable ht = new Hashtable();
            //ht.Add(iT.MoveFrom.y,-4);
            //ht.Add(iT.MoveFrom.time,2);
            //iTween.MoveFrom(rune,ht);
            //challengeCompleted = complete;
        }
    }
}
