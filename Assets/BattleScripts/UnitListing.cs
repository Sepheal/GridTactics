using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Unit listing struct for holding unit values between fights

public class UnitListing : MonoBehaviour
{
    public Owner Controller;
    public int MonsterId;
    public int MyLevel = 1;
    public int Exp = 0;
    public Tile StartingTile;

    public string MyName;
    public int PassiveLevel = 1;

    public bool Placed = false;
    public UnitMovement MyBody;
}
