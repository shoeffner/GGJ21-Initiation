using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DestroyAfterTime : NetworkBehaviour
{
    public float timeToLive = 5;

    // Update is called once per frame
    void Update()
    {
        timeToLive -= Time.deltaTime;
        if (timeToLive <= 0)
        {
            NetworkServer.Destroy(gameObject);
        }
    }
}
