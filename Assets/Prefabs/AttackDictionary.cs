using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack
{
    public int ID;
    public string Name;
    public int BaseDamage;
}

public class AttackDictionary : MonoBehaviour
{
    public List<Attack> AttackList = new List<Attack>();

    private void Awake()
    {
        AttackList.Add(new Attack { ID = 1, Name = "Tackle", BaseDamage = 30 });
        AttackList.Add(new Attack { ID = 2, Name = "Wave Slap", BaseDamage = 15 });
        AttackList.Add(new Attack { ID = 3, Name = "Double Prong Poke", BaseDamage = 15 });
        AttackList.Add(new Attack { ID = 4, Name = "String Shot", BaseDamage = 20 });
        AttackList.Add(new Attack { ID = 5, Name = "Shell Stab", BaseDamage = 25 });
    }
}
