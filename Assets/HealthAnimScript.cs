using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

public class HealthAnimScript : MonoBehaviour
{
    public Image Front, Back;
    bool Draining = false, EndDelay = false;
    float DrainStartTime, DrainTimeLength = 1.0f, EndDelayTime, EndDelayLength = 0.5f;
    float CurrentPercent = 1.0f, LastPercent = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Draining)
        {
            float Factor = (Time.time - DrainStartTime) / DrainTimeLength;
            if (Factor >= 0.3f && Factor < 1.0f)
            {
                float Value = CurrentPercent + ((LastPercent - CurrentPercent) * (1.3f - Factor) * (1.3f - Factor));
                Back.GetComponent<Image>().fillAmount = Value;
            }
            else if (Factor >= 1.0f)
            {
                Back.GetComponent<Image>().fillAmount = CurrentPercent;
                Draining = false;
                EndDelayTime = Time.time;
                EndDelay = true;
            }
        }
        else if (EndDelay)
        {
            float Factor = (Time.time - EndDelayTime) / EndDelayLength;
            if (Factor >= 1.0f)
            {
                TurnOff();
                EndDelay = false;
            }
        }
    }

    public void SetNewHealthPercent(float percent)
    {
        Front.GetComponent<Image>().fillAmount = percent;
        LastPercent = CurrentPercent;
        CurrentPercent = percent;
        DrainStartTime = Time.time;
        Draining = true;
    }

    public void SetMainColour(Color32 color)
    {
        Front.GetComponent<Image>().color = color;
    }

    public void SetPos(Vector3 NewPos)
    {
        Front.enabled = true;
        Back.enabled = true;
        gameObject.transform.position = NewPos;
    }

    public void TurnOff()
    {
        Front.enabled = false;
        Back.enabled = false;
    }
}
