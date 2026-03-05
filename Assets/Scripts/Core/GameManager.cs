using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static InputAction aimPlayer { get; private set; }
    public static InputAction aimPress { get; private set; }

    void Awake()
    {
        aimPlayer = InputSystem.actions["AimPlayer"];
        aimPress = InputSystem.actions["AimPress"];

        if (aimPlayer == null)
        {
            Debug.LogError("Input action 'AimPlayer' was not found.");
        }

        if (aimPress == null)
        {
            Debug.LogError("Input action 'AimPress' was not found.");
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

    public void updateStats (string statName, float value, string caller)
    {
        if (caller == "Player")
        {
            GameObject player = GameObject.Find("Player");
            if (player == null)
            {
                Debug.LogWarning("Player object not found.");
                return;
            }

            PlayerStats stats = player.GetComponent<PlayerStats>();
            if (stats == null)
            {
                Debug.LogWarning("PlayerStats component not found on Player.");
                return;
            }

            if (!stats.SetStat(statName, value))
            {
                Debug.LogWarning($"Invalid stat name: '{statName}'.");
                return;
            }
            else
            {
                stats.SetStat(statName, value);
            }
            
        }
       /* else
        {
            
            EnemeyStats stats = player.GetComponent<EnemeyStats>();
            if (stats == null)
            {
                Debug.LogWarning("EnemeyStats component not found on Enemey.");
                return;
            }

            if (!stats.SetStat(statName, value))
            {
                Debug.LogWarning($"Invalid stat name: '{statName}'.");
            }
        }*/
    }
}
