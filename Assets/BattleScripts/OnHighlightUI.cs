using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//Used for showing the highlight animation in menus, needs reworking for controller

public class OnHighlightUI : MonoBehaviour, IPointerEnterHandler
{
    public int MyId = 0;
    public GameObject Control;
    public int AttackOption = 0;
    public UnitListing MyAssignedUnit;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Control)
        {
            //Control.SetHighlight(MyId);
            Control.SendMessage("SetHighlightRemote", MyId);
        }
    }
}
