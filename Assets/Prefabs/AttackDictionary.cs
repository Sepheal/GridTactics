using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack
{
    public int ID;
    public string Name;
    public int BaseDamage;
    public int AOEID;
    public AudioClip AttSound;
}

public class AttackDictionary : MonoBehaviour
{
    public List<Attack> AttackList = new List<Attack>();

    public List<string> AttNames;
    public List<int> BaseDamage;
    public List<int> AreaOfEffectId;
    public List<AudioClip> AttSound;

    private void Awake()
    {
        for (int i = 0; i < AttNames.Count; i++)
        {
            AttackList.Add(new Attack { ID = i + 1, Name = AttNames[i], BaseDamage = BaseDamage[i], AOEID = AreaOfEffectId[i], AttSound = AttSound[i] });
        }
    }

    public int[] GetAttacks(int id, int lvl)
    {
        int[] Attacks = new int[] { 0, 0, 0 };
        int[] AttackToGive;

        switch (id)
        {
            case 0: //Slime
                AttackToGive = new int[3] { 1, 2, 3 };
                break;
            case 1: //Shell
                AttackToGive = new int[3] { 5, 2, 4 };
                break;
            case 2: //Player
                AttackToGive = new int[3] { 6, 3, 2 };
                break;
            case 3: //Blue Slime
                AttackToGive = new int[3] { 1, 2, 3 };
                break;
            case 4: //Red Shell
                AttackToGive = new int[3] { 5, 2, 4 };
                break;
            case 5: //Spider
                AttackToGive = new int[3] { 4, 1, 2 };
                break;
            case 6: //Red Spider
                AttackToGive = new int[3] { 4, 1, 2 };
                break;
            case 7: //Yellow Spider
                AttackToGive = new int[3] { 4, 1, 2 };
                break;
            default: //default
                AttackToGive = new int[3] { 7, 8, 1 };
                break;
        }

        Attacks[0] = AttackToGive[0];
        if (lvl >= 3) Attacks[1] = AttackToGive[1];
        if (lvl >= 6) Attacks[2] = AttackToGive[2];

        return Attacks;
    }
}
