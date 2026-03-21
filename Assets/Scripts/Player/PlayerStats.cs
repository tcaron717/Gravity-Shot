using System;
using UnityEngine;
using System.Collections.Generic;

public class PlayerStats : MonoBehaviour
{
    public int hp = 3;
    public int baseDamage = 1;
    public float launchPower = 1f;
    public float maxLaunchForce = 5f;
    public float onDragVelocityMultiplier;
    private float gravityScale;
    private Rigidbody2D _rb;
    private readonly Dictionary<string, float> _stats = new Dictionary<string, float>();
    private readonly Dictionary<string, float> _baseStats = new Dictionary<string, float>();
    private PlayerBoons playerBoons;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        playerBoons = GetComponent<PlayerBoons>();

        if (_rb == null)
        {
            Debug.LogError("PlayerStats requires a Rigidbody2D component on the same GameObject.", this);
            return;
        }

        // Seed current stat dictionary from current inspector/default values.
        _stats[nameof(hp)] = hp;
        _stats[nameof(baseDamage)] = baseDamage;
        _stats[nameof(launchPower)] = launchPower;
        _stats[nameof(maxLaunchForce)] = maxLaunchForce;
        _stats[nameof(onDragVelocityMultiplier)] = onDragVelocityMultiplier;
        _stats[nameof(gravityScale)] = _rb.gravityScale;
        
        // Seed current stat dictionary from current inspector/default values.
        _baseStats[nameof(hp)] = hp;
        _baseStats[nameof(baseDamage)] = baseDamage;
        _baseStats[nameof(launchPower)] = launchPower;
        _baseStats[nameof(maxLaunchForce)] = maxLaunchForce;
        _baseStats[nameof(onDragVelocityMultiplier)] = onDragVelocityMultiplier;
        _baseStats[nameof(gravityScale)] = _rb.gravityScale;

        if (playerBoons == null)
        {
            Debug.LogError("PlayerStats requires a PlayerBoons component on the same GameObject.", this);
            return;
        }

    }

    void Start()
    {
        if (playerBoons == null)
        {
            return;
        }
        
    }

    public bool SetStat(string statName, float value)
    {
        if (string.IsNullOrWhiteSpace(statName))
        {
            Debug.Log("SetStat: Stat name cannot be empty.");
            return false;
        }

        _stats[statName] = value;

        // Keep strongly-typed fields in sync for existing code paths.
        switch (statName)
        {
            case nameof(hp):
                hp = Mathf.RoundToInt(value);
                if (hp <= 0)
                    //update when we have game over logic
                    Debug.Log($"Player died. HP: {hp}");
                break;
            case nameof(baseDamage):
                baseDamage = Mathf.RoundToInt(value);
                break;
            case nameof(launchPower):
                launchPower = value;
                break;
            case nameof(maxLaunchForce):
                maxLaunchForce = value;
                break;
            case nameof(onDragVelocityMultiplier):
                onDragVelocityMultiplier = value;
                break;
            case nameof(gravityScale):
                _rb.gravityScale = value;
                break;
        }
        Debug.Log($"SetStat: Successfully set stat '{statName}' to {value}");
        return true;
    }

    public float TryGetStatValue(string statName)
    {
        var statValue = _stats.GetValueOrDefault(statName);
        Debug.LogWarning($"Retrieved stat '{statName}' with value {statValue}");
        return statValue;
    }
    
    public float TryGetBaseStatValue(string statName)
    {
        var statValue = _baseStats.GetValueOrDefault(statName);
        Debug.LogWarning($"Retrieved stat '{statName}' with value {statValue}");
        return statValue;
    }
    

    
}
