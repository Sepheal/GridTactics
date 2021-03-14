using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitListing : MonoBehaviour
{
    public Owner Controller;
    public int MonsterId;
    public int MyLevel = 1;
    public int Exp = 0;
    public Tile StartingTile;
    //public Vector2 Pos;
    //public Stats UnitStats;

    public string MyName;
    //public int MovementPoints, AttackRange, MaxHealth, AttackStat, DefenceStat;
    public Element MyElement;
    public int[] AttackList;

    public bool Placed = false;
    public UnitMovement MyBody;
}
