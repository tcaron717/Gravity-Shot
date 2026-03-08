using UnityEngine;

public class DamageModel
{

    public static void TakeDamage(string entityDealingDamage, string entityTakingDamage)
    {
        if (entityDealingDamage == "Player")
        {
            PlayerStats playerStats = GameManager.playerStats;
            if (playerStats == null)
            {
                Debug.LogWarning("TakeDamage failed: PlayerStats was not available on GameManager.");
                return;
            }

            int damageDealt = playerStats.baseDamage;
            
            if (!GameManager.TryGetEnemyStats(entityTakingDamage, out EnemyStats enemyStats))
            {
                Debug.LogWarning($"TakeDamage failed: EnemyStats for '{entityTakingDamage}' was not found.");
                return;
            }

            int newHp = Mathf.Max(0, enemyStats.hp - damageDealt);
            
            Debug.Log($"TakeDamage: Player dealt {damageDealt} damage to {entityTakingDamage}. New HP: {newHp}");
            GameManager.UpdateStats(nameof(EnemyStats.hp), newHp, entityTakingDamage);
        }
        else
        {
            EnemyStats enemyStats = GameManager.enemyStats;
            PlayerStats playerStats = GameManager.playerStats;
            if (enemyStats == null)
            {
                Debug.LogWarning("TakeDamage failed: EnemyStats was not available on GameManager.");
                return;
            }

            int damageDealt = enemyStats.baseDamage;
            
            if (playerStats == null)
            {
                Debug.LogWarning($"TakeDamage failed: PlayerStats were not found.");
                return;
            }

            int newHp = Mathf.Max(0, playerStats.hp - damageDealt);
            
            Debug.Log($"TakeDamage: Enemy dealt {damageDealt} damage to {entityTakingDamage}. New HP: {newHp}");
            GameManager.UpdateStats(nameof(PlayerStats.hp), newHp, entityTakingDamage);
        }
        


            
    }
    /* still need to create EnemyStats when I get to creating enemies
     public void TakeDamageEnemy()
        {
            string caller = gameObject.name;
            
            EnemyStats stats = GameObject.Find(caller).GetComponent<EnemyStats>();
            damage = stats.baseDamage;
            
        }*/
}
