using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Monster dictionary with struct

[System.Serializable]
public struct Monster
{
    public int ID;
    public GameObject MonPrefab;
    public Sprite MonsterSpriteRight, MonsterSpriteLeft;
    public int AttackBaseStat, DefenceBaseStat, SpeedBaseStat, HPBaseStat;
    public Element MyElement;
    public int[] AttackTree;
    public AudioClip TakeDamageSound, DeathSound, WalkSound;
    public string Species, Description;
}

public class MonsterDictionary : MonoBehaviour
{
    public List<Monster> Monsters;

    public int[] GetAttacks(int id, int lvl)
    {
        int[] Attacks = new int[] { 0, 0, 0 };

        Attacks[0] = Monsters[id].AttackTree[0];
        if (lvl >= 3) Attacks[1] = Monsters[id].AttackTree[1];
        if (lvl >= 6) Attacks[2] = Monsters[id].AttackTree[2];

        return Attacks;
    }

    public int[] GetFutureAttacks(int id, int lvl)
    {
        int[] Attacks = new int[] { 0, 0, 0 };

        if (lvl < 3) Attacks[1] = Monsters[id].AttackTree[1];
        if (lvl < 6) Attacks[2] = Monsters[id].AttackTree[2];

        return Attacks;
    }
}
