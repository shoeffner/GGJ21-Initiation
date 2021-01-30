using UnityEngine;
using Mirror;


[RequireComponent(typeof(Collider))]
public class HealTriggerTarget : NetworkBehaviour
{
    public int health = 5;

    void OnTriggerEnter(Collider collider)
    {
        CharacterStats characterStats = collider.gameObject.GetComponent<CharacterStats>();
        if (characterStats != null)
        {
            characterStats.CmdHeal(health);
        }
    }
}
