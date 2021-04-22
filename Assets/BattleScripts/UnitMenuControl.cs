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

    public AudioClip ConfirmAudio, ConfirmAudio2, ChangeAudio, CancelAudio;

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
                    if (OptionList[NumListed].name == "Wait") Wait(); //P.Wait();
                    else if (OptionList[NumListed].name == "Cancel") Reset(); // P.Reset();
                    else if (OptionList[NumListed].name == "Attack1") ShowAttack(1); //  P.ShowAttack(1);
                    else if (OptionList[NumListed].name == "Attack2") ShowAttack(2); // P.ShowAttack(2);
                    else if (OptionList[NumListed].name == "Attack3") ShowAttack(3); // P.ShowAttack(3);
                    else if (OptionList[NumListed].name == "Attack4") ShowAttack(4); // P.ShowAttack(4);
                    else if (OptionList[NumListed].name == "Attack5") ShowAttack(5); // P.ShowAttack(5);
                    else if (OptionList[NumListed].name == "Attack6") ShowAttack(6); // P.ShowAttack(5);
                    else if (OptionList[NumListed].name == "Attack7") ShowAttack(7); // P.ShowAttack(5);
                    else if (OptionList[NumListed].name == "Attack8") ShowAttack(8); // P.ShowAttack(5);
                    Active = false;
                }
                else if (Input.GetButtonDown("Cancel"))
                {
                    Reset(); // P.Reset();
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
        GetComponent<AudioSource>().PlayOneShot(ChangeAudio);
    }

    public void SetHighlightRemote(int i) //Is used don't delete!
    {
        NumListed = i;
        SetHighlight();
    }

    public void ShowAttack(int id)
    {
        P.ShowAttack(id);
        GetComponent<AudioSource>().PlayOneShot(ConfirmAudio);
    }

    public void Wait()
    {
        P.Wait();
        GetComponent<AudioSource>().PlayOneShot(ConfirmAudio);
    }

    public void Reset()
    {
        P.Reset();
        GetComponent<AudioSource>().PlayOneShot(CancelAudio);
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
