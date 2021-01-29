using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(Collider))]
public class DestroyOnHit : NetworkBehaviour
{
    public List<string> ignoredTags = new List<string>();

    void OnCollisionEnter(Collision collision)
    {
        foreach (string tag in ignoredTags)
        {
            if (collision.collider.CompareTag(tag))
            {
                return;
            }
        }
        NetworkServer.Destroy(gameObject);
    }
}
