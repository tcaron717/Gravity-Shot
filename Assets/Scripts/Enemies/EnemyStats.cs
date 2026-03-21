using UnityEngine;
using System.Collections.Generic;

public class EnemyStats : MonoBehaviour
{
    public int hp = 3;
    public int baseDamage = 1;
    public float launchPower = 1f;
    public float maxLaunchForce = 5f;
    public float onDragVelocityMultiplier;
    private float gravityScale;
    private Rigidbody2D _rb;
    private readonly Dictionary<string, float> _stats = new Dictionary<string, float>();

    void Awake()
    {
        //Get Rigidbody2d stats
       _rb = GetComponent<Rigidbody2D>();

        // Seed dictionary from current inspector/default values.
        _stats[nameof(hp)] = hp;
        _stats[nameof(baseDamage)] = baseDamage;
        _stats[nameof(launchPower)] = launchPower;
        _stats[nameof(maxLaunchForce)] = maxLaunchForce;
        _stats[nameof(onDragVelocityMultiplier)] = onDragVelocityMultiplier;
        _stats[nameof(gravityScale)] = _rb.gravityScale;
    }

    void OnEnable()
    {
        GameManager.RegisterEnemyStats(this);
    }

    void OnDisable()
    {
        GameManager.UnregisterEnemyStats(this);
    }

    public bool SetStat(string statName, float value)
    {
        if (string.IsNullOrWhiteSpace(statName))
        {
            return false;
        }

        _stats[statName] = value;

        // Keep strongly-typed fields in sync for existing code paths.
        switch (statName)
        {
            case nameof(hp):
                hp = Mathf.RoundToInt(value);
                if (hp <= 0)
                    Destroy(gameObject);
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

    public float TryGetStat(string statName)
    {
        var statValue = _stats.GetValueOrDefault(statName);
        Debug.LogWarning($"Retrieved stat '{statName}' with value {statValue}");
        return statValue;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
