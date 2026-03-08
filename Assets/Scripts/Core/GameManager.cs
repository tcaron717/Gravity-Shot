using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.PlayerLoop;

public class GameManager : MonoBehaviour
{
    public static InputAction aimPlayer { get; private set; }
    public static InputAction aimPress { get; private set; }
    public static PlayerStats playerStats;
    public static EnemyStats enemyStats;
    private static readonly Dictionary<string, EnemyStats> enemyStatsByName = new Dictionary<string, EnemyStats>();
    private static readonly Dictionary<string, PlayerStats> playerStatsByName = new Dictionary<string, PlayerStats>();
    [SerializeField] private PlayerStats playerStatsReference;
    [SerializeField] private EnemyStats enemyStatsReference;
    
    void Awake()
    {
        aimPlayer = InputSystem.actions["AimPlayer"];
        aimPress = InputSystem.actions["AimPress"];
        if (playerStatsReference != null)
        {
            playerStats = playerStatsReference;
        }
        else
        {
            playerStats = FindFirstObjectByType<PlayerStats>();
        }
        
        if (enemyStatsReference != null)
        {
            enemyStats = enemyStatsReference;
        }
        else
        {
            enemyStats = FindFirstObjectByType<EnemyStats>();
        }

        if (aimPlayer == null)
        {
            Debug.LogError("Input action 'AimPlayer' was not found.");
        }

        if (aimPress == null)
        {
            Debug.LogError("Input action 'AimPress' was not found.");
        }

        if (playerStats == null)
        {
            Debug.LogWarning("PlayerStats could not be resolved. Assign it on GameManager for best performance.");
        }
    }

    void OnEnable()
    {
        aimPlayer?.Enable();
        aimPress?.Enable();
    }

    void OnDisable()
    {
        aimPlayer?.Disable();
        aimPress?.Disable();
    }

    public static void RegisterEnemyStats(EnemyStats enemyStats)
    {
        if (enemyStats == null)
        {
            return;
        }

        enemyStatsByName[enemyStats.gameObject.name] = enemyStats;
    }

    public static void UnregisterEnemyStats(EnemyStats enemyStats)
    {
        if (enemyStats == null)
        {
            return;
        }

        enemyStatsByName.Remove(enemyStats.gameObject.name);
    }

    public static bool TryGetEnemyStats(string enemyName, out EnemyStats enemyStats)
    {
        if (string.IsNullOrWhiteSpace(enemyName))
        {
            enemyStats = null;
            return false;
        }

        return enemyStatsByName.TryGetValue(enemyName, out enemyStats);
    }
    

    public static void UpdateStats(string statName, float value, string caller)
    {
        Debug.Log($"UpdateStats: Trying to update stat '{statName}' with value {value} for {caller}");
        if (string.IsNullOrWhiteSpace(statName))
        {
            Debug.LogWarning("UpdateStats: Stat name cannot be empty.");
            return;
        }

        if (string.IsNullOrWhiteSpace(caller))
        {
            Debug.LogWarning("UpdateStats: Caller cannot be empty.");
            return;
        }

        if (caller == "Player")
        {
            if (playerStats == null)
            {
                Debug.LogWarning("UpdateStats: PlayerStats component not found on Player.");
                return;
            }

            if (!playerStats.SetStat(statName, value))
            {
                Debug.LogWarning($"UpdateStats: Invalid stat name: '{statName}'.");
                return;
            }
            
            Debug.Log($"UpdateStats: Calling SetStat");
            playerStats.SetStat(statName, value);
            return;
        }

        if (!TryGetEnemyStats(caller, out EnemyStats enemyStats))
        {
            Debug.LogWarning($"UpdateStats: EnemyStats for '{caller}' was not found. Make sure the enemy is registered.");
            return;
        }

        if (!enemyStats.SetStat(statName, value))
        {
            Debug.LogWarning($"UpdateStats: Invalid stat name: '{statName}'.");
            return;
        }
        
        //Debug.Log($"UpdateStats: Calling SetStat");
        //enemyStats.SetStat(statName, value);
    }

    public static float GetCurrentStatValue(string statName, string caller, out float value)
    {
        if (caller == "Player")
        {
            if (playerStats == null)
            {
                Debug.LogWarning("GetCurrentStatValue: PlayerStats component not found on Player.");
                return value = 0;
            }

            return value = playerStats.TryGetStat(statName);
        }

        if (!TryGetEnemyStats(caller, out EnemyStats enemyStats))
        {
            Debug.LogWarning($"GetCurrentStatValue: EnemyStats for '{caller}' was not found. Make sure the enemy is registered.");
            return value = 0;
        }

        return value = enemyStats.TryGetStat(statName);

    }
       
}
