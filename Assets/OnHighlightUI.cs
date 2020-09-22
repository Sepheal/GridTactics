using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnHighlightUI : MonoBehaviour, IPointerEnterHandler
{
    public int MyId = 0;
    public UnitMenuControl Control;
    public int AttackOption = 0;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Control)
        {
            Control.SetHighlight(MyId);
        }
    }
}
