using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class KillAllWithTag : MonoBehaviour
{

    public string tagToKill = "Enemy";

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		if(Input.GetKeyDown(KeyCode.Q)) {
            GameObject[] objs = GameObject.FindGameObjectsWithTag(tagToKill);
			foreach(var obj in objs) {
                NetworkServer.Destroy(obj);
			}
		}
    }
}
