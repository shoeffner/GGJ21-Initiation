using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : CharacterStatChanger
{
    public int amount = 5;

    public new void Apply(CharacterStats cs)
    {
        if (!isServer)
        {
            return;
        }
        cs.health += amount;
    }
}
