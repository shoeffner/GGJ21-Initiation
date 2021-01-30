using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(DestroyOnCollision))]
public class Fireball : NetworkBehaviour
{
    [Range(0, 100)]
    public float fireballVelocity = 25.0f;

    private DestroyOnCollision doc;

    public Collider triggerCollider;


    public void CastBy(GameObject character)
    {
        if (doc == null)
        {
            doc = gameObject.GetComponent<DestroyOnCollision>();
        }

        Physics.IgnoreCollision(triggerCollider,character.GetComponent<Collider>(),true);

        doc.ignoredGameObjects.Add(character);
        AbilityManager am = character.GetComponent<AbilityManager>();
        transform.position = am.shootAnchor.position;
        transform.rotation = am.shootAnchor.rotation;
        GetComponent<Rigidbody>().velocity = am.shootAnchor.forward * fireballVelocity;
    }

	

}
