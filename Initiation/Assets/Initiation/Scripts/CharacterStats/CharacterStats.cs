using Mirror;

public class CharacterStats : NetworkBehaviour
{
    public int maxHealth = 10;

    [SyncVar]
    public int health = 10;
    
    [SyncVar]
    public bool dead = false;

    [Command]
    public void TakeDamage(int amount)
    {
        if (dead)
        {
            return;
        }

        health -= amount;
        if (health <= 0)
        {
            health = 0;
            dead = true;
        }
    }

    [Command]
    public void Heal(int amount)
    {
        if (dead)
        {
            return;
        }

        health += amount;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }
}
