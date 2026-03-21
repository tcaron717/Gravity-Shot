using System;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerBoons : MonoBehaviour
{
    [SerializeField] BoonList boonList;
    private PlayerStats playerStats;
    private int _currentBoonCount;
    public static int maxBoonCount = 3;
    private int [] activeBoonsIds = new int[maxBoonCount];

    void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
    }
    
    //Add or change a boon to the player
    bool AddBoon(int newBoonId, int existingBoonId = 0)
    {
        int positionToAdd;
        if (existingBoonId == 0)
        {

            if (_currentBoonCount < maxBoonCount)
                if (_currentBoonCount == 0)
                {
                    positionToAdd = 0;
                    activeBoonsIds[positionToAdd] = newBoonId;
                    _currentBoonCount++;
                    BoonStatUpdate();
                    return true;
                }
                else
                {
                    positionToAdd = activeBoonsIds[_currentBoonCount - 1];
                    activeBoonsIds[positionToAdd] = newBoonId;
                    _currentBoonCount++;
                    BoonStatUpdate();
                    return true;
                }
        }
        
        positionToAdd = Array.IndexOf(activeBoonsIds, existingBoonId);
        activeBoonsIds[positionToAdd] = newBoonId;
        BoonStatUpdate();
        return true;

    }

    public void BoonStatUpdate()
    {
        if (playerStats == null)
        {
            playerStats = GetComponent<PlayerStats>();
        }

        if (boonList == null)
        {
            Debug.LogError("PlayerBoons requires a BoonList reference in the Inspector.", this);
            return;
        }

        if (playerStats == null)
        {
            Debug.LogError("PlayerBoons requires a PlayerStats component on the same GameObject.", this);
            return;
        }

        if (activeBoonsIds.Length != 0)
        {
            foreach (int boonId in activeBoonsIds)
            {
                string[,] boonStats = boonList.GetBoonStatsById(boonId);
                for (int i = 0; i < boonStats.GetLength(0); i++)
                {
                    string statName = boonStats[i, 0];
                    string statValue = boonStats[i, 1];
                
                    float currentStatValue = playerStats.TryGetBaseStatValue(statName);
                    float newStatValue = currentStatValue + float.Parse(statValue);
                
                    playerStats.SetStat(statName, newStatValue);
                    Debug.Log("Boon stat updated " + statName + " by " + statValue);
                }
            }
        }
        else
        {
            Debug.Log("No boons active");
        }

    }
    
}
