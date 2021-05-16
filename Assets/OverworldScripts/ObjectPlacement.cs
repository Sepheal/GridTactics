using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacement : MonoBehaviour
{
    PersistantStats PS;
    public int EncounterNumRequired = 0;
    public Vector3 PosMoveToo = new Vector3();

    // Start is called before the first frame update
    void Start()
    {
        PS = FindObjectOfType<PersistantStats>();
        if (PS.EncounterList[EncounterNumRequired] == true) transform.localPosition = PosMoveToo;
    }
}
