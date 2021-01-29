using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class CharacterStatChanger : NetworkBehaviour
{
    public virtual void Apply(CharacterStats characterStats)
    {
    }
}