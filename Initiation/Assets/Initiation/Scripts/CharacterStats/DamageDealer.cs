using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : CharacterStatChanger
{
    public int amount = 1;

    public new void Apply(CharacterStats cs)
    {
        if (!isServer)
        {
            return;
        }
        cs.health -= amount - cs.armor;
    }
}
