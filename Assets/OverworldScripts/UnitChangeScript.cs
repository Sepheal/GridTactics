using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitChangeScript : MonoBehaviour
{
    public int MyId;
    public InputField input;
    public PersistantStats PS;
    public UnitListing Unit;

    public void Setup(int id ,PersistantStats ps, UnitListing unit, InputField IF)
    {
        PS = ps;
        Unit = unit;
        input = IF;
        MyId = id;

        input.onValueChanged.AddListener(delegate { SetName(); });
    }

    public void SetName()
    {
        if (MyId == 2) PS.PName = input.text;
        else
        {
            Unit.MyName = input.text;
        }
    }
}
