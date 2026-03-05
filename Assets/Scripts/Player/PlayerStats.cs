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
        
        return true;
    }

    public bool TryGetStat(string statName, out float value)
    {
        return _stats.TryGetValue(statName, out value);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
