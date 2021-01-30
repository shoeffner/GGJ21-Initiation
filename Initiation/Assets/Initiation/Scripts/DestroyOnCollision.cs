using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(Collider))]
public class DestroyOnCollision : NetworkBehaviour
{
    public readonly SyncList<string> ignoredTags = new SyncList<string>();
    public readonly SyncList<GameObject> ignoredGameObjects = new SyncList<GameObject>();

    void OnCollisionEnter(Collision collision)
    {
        if (!ignoredTags.Contains(collision.collider.tag) && !ignoredGameObjects.Contains(collision.collider.gameObject))
        {
            NetworkServer.Destroy(gameObject);
        }
    }
}
