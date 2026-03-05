using UnityEngine;

public class DamageModel:ScriptableObject
{
    private int damage;

    public void TakeDamage( string caller)  
    {
        if (caller == "Player")
        {
            PlayerStats stats = GameObject.Find(caller).GetComponent<PlayerStats>();
            damage = stats.baseDamage;
            
        }
       /* else
        {
            //still need to create EnemyStats when I get to creating enemies
            EnemyStats stats = GameObject.Find(caller).GetComponent<EnemyStats>();
            damage = stats.baseDamage;
            
        }*/

    }
}
