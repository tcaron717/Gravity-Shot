using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BoonList", menuName = "Scriptable Objects/BoonList")]
public class BoonList : ScriptableObject
{
    public class Boon
    {
        public int id { get; set; }
        public string name { get; set; }
        public string icon { get; set; }
        //Gravity, Momentum, Magnetism, Elasticity, Force, HP, Damage, Velcoity Cap
        public string domain { get; set; }
        //T1 = early game, T2 = mid game, T3 = late game
        public string tier { get; set; }
        public string description { get; set; } 
        public Sprite sprite { get; set; }
        //[Stat effected, value +-]
        public string[,] effect { get; set; }
    }

    private List<Boon> boons = new List<Boon>
    {
        new Boon { id = 1, name = "Orbital Shift", domain = "Gravity", tier = "T1", description = "Shift time and space to increase the gravity around you, making you heavier.", sprite = null, effect = new string [,] {{"gravityScale", "1"}}  },
        new Boon { id = 2, name = "Kinetic Surge", domain = "Force, Damage", tier = "T1", description = "You know generate a surge of power when you make contact with an enemy.", sprite = null, effect = new string [,] {{"damage", "1"}}  },
        new Boon { id = 3, name = "Core Distortion", domain = "Force, Damage", tier = "T1", description = "Your core becomes more unstable making movement more difficult but increasing your speed.", sprite = null, effect = new string [,] {{"launchPower", ".5"}, {"maxLaunchForce", "5"}} }

    };

    public string[,] GetBoonStatsById(int id)
    {
        return boons.Find(boon => boon.id == id).effect;
    }

}
