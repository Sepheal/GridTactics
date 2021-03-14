using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UnitMenuControl : MonoBehaviour
{
    List<GameObject> OptionList;
    int NumListed = 1;
    bool Active = false, FrameBuffer = false;
    float CooldownStart;  readonly float FrameCooldown = 0.2f;
    PlayerMovement P;

    // Update is called once per frame
    void Update()
    {
        if (FrameBuffer)
        {
            if (Active)
            {
                if (Time.time - CooldownStart >= FrameCooldown)
                {
                    if (Input.GetAxisRaw("Vertical") > 0 ) //|| Input.GetAxisRaw("Mouse Y") > 0)
                    {
                        NumListed--;
                        if (NumListed < 0) NumListed = 0; //NumListed = OptionList.Count - 1;
                        SetHighlight();
                        CooldownStart = Time.time;
                    }
                    else if (Input.GetAxisRaw("Vertical") < 0 )//|| Input.GetAxisRaw("Mouse Y") < 0)
                    {
                        NumListed++;
                        if (NumListed == OptionList.Count) NumListed = OptionList.Count - 1; //NumListed = 0;
                        SetHighlight();
                        CooldownStart = Time.time;
                    }
                }

                //Select button
                if (Input.GetButtonDown("Select") )//|| Input.GetMouseButtonDown(0))
                {
                    if (OptionList[NumListed].name == "Wait") P.Wait();
                    else if (OptionList[NumListed].name == "Cancel") P.Reset();
                    else if (OptionList[NumListed].name == "Attack1") P.ShowAttack(1);
                    else if (OptionList[NumListed].name == "Attack2") P.ShowAttack(2);
                    else if (OptionList[NumListed].name == "Attack3") P.ShowAttack(3);
                    else if (OptionList[NumListed].name == "Attack4") P.ShowAttack(4);
                    else if (OptionList[NumListed].name == "Attack5") P.ShowAttack(5);
                    Active = false;
                }
                else if (Input.GetButtonDown("Cancel"))
                {
                    P.Reset();
                }
            }
        }
        else FrameBuffer = true;
    }

    void SetHighlight()
    {
        foreach (GameObject item in OptionList)
        {
            item.transform.Find("H").gameObject.SetActive(false);
        }
        OptionList[NumListed].transform.Find("H").gameObject.SetActive(true);
    }

    public void SetHighlightRemote(int i) //Is used don't delete!
    {
        NumListed = i;
        SetHighlight();
        print(i);
    }

    public void ShowAttack(int id)
    {
        P.ShowAttack(id);
    }

    public void Wait()
    {
        P.Wait();
    }

    public void Reset()
    {
        P.Reset();
    }

    public void Activate(PlayerMovement p)
    {
        //Enable options
        foreach (Transform item in transform)
        {
            if (item.GetComponent<OnHighlightUI>().AttackOption != 0)
            {
                foreach (int AttackId in p.AttackIDs)
                {
                    if (AttackId == item.GetComponent<OnHighlightUI>().AttackOption)
                    {
                        item.gameObject.SetActive(true);
                        //item.GetComponent<Button>().
                    }
                }
            }
            else
            {
                item.gameObject.SetActive(true);
            }
        }

        OptionList = new List<GameObject>();
        int i = 0;
        foreach (Transform item in transform)
        {
            if (item.gameObject.activeSelf)
            {
                OptionList.Add(item.gameObject);
                item.GetComponent<OnHighlightUI>().MyId = i;
                item.GetComponent<OnHighlightUI>().Control = gameObject;
                i++;
            }
        }
        NumListed = 0;
        SetHighlight();
        CooldownStart = Time.time;
        Active = true;
        Cursor.lockState = CursorLockMode.None;
        FrameBuffer = false;
        P = p;
    }

    public void Deactivate()
    {
        foreach (Transform item in transform)
        {
            item.gameObject.SetActive(false);
        }
        Active = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
