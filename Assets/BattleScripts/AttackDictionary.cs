using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Attack dictionary with struct

[System.Serializable]
public struct Attack
{
    public int ID;
    public string Name;
    public int BaseDamage;
    public int AOEID;
    public AudioClip AttSound;
}

public class AttackDictionary : MonoBehaviour
{
    public List<Attack> AttackList;
}
