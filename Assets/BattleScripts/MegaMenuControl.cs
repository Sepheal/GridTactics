using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Game menu in battle control, needs work

public class MegaMenuControl : MonoBehaviour
{
    public bool Active = false;
    public GameObject Holder;
    public GameObject TitlePanel;
    public GameObject[] Options;
    int OptionSelected = 0;
    bool FrameBuffer = false;
    float CooldownStart; readonly float FrameCooldown = 0.2f;

    // Update is called once per frame
    void Update()
    {
        if (FrameBuffer)
        {
            if (Active)
            {
                if (Time.time - CooldownStart >= FrameCooldown)
                {
                    if (Input.GetAxisRaw("Vertical") > 0) //|| Input.GetAxisRaw("Mouse Y") > 0)
                    {
                        OptionSelected--;
                        if (OptionSelected < 0) OptionSelected = 0; //NumListed = OptionList.Count - 1;
                        SetHighlight();
                        CooldownStart = Time.time;
                    }
                    else if (Input.GetAxisRaw("Vertical") < 0)//|| Input.GetAxisRaw("Mouse Y") < 0)
                    {
                        OptionSelected++;
                        if (OptionSelected == Options.Length) OptionSelected = Options.Length - 1; //NumListed = 0;
                        SetHighlight();
                        CooldownStart = Time.time;
                    }
                }

                //Select button
                if (Input.GetButtonDown("Select"))
                {
                    if (Options[OptionSelected].name == "GameStats") BackToMap();
                    else if (Options[OptionSelected].name == "EndTurn") EndTurn();
                    else if (Options[OptionSelected].name == "StartCombat") StartCombat();
                    else if (Options[OptionSelected].name == "ReturnToMap") BackToMap();
                }
                else if (Input.GetButtonDown("Cancel"))
                {
                    BackToMap();
                }
                else if (Input.GetButtonDown("StartButton"))
                {
                    BackToMap();
                }
            }
        }
        else FrameBuffer = true;
    }

    public void EndTurn()
    {
        foreach (PlayerMovement p in FindObjectsOfType<PlayerMovement>())
        {
            if (p.MyState == UnitState.Ready)
            {
                p.EndPawnTurn();
            }
        }
        //BackToMap();
        ToggleMenu(false);
    }

    public void StartCombat()
    {
        TitlePanel.GetComponent<Animator>().SetTrigger("PlayTitle2");
        ToggleMenu(false);
        FindObjectOfType<SelectControl>().SetupMode = false;
        FindObjectOfType<GameManager>().SetupOver();
    }

    public void BackToMap()
    {
        ToggleMenu(false);
        FindObjectOfType<SelectControl>().Activate();
    }

    public void ShowMenu(bool SetupMode)
    {
        switch (SetupMode)
        {
            case true:
                Options[1] = Holder.transform.Find("StartCombat").gameObject;
                Holder.transform.Find("StartCombat").gameObject.SetActive(true);
                Holder.transform.Find("EndTurn").gameObject.SetActive(false);
                break;
            case false:
                Options[1] = Holder.transform.Find("EndTurn").gameObject;
                Holder.transform.Find("StartCombat").gameObject.SetActive(false);
                Holder.transform.Find("EndTurn").gameObject.SetActive(true);
                break;
            default:
                break;
        }
        ToggleMenu(true);
        FrameBuffer = false;
    }

    void SetHighlight()
    {
        foreach (GameObject item in Options)
        {
            item.transform.Find("H").gameObject.SetActive(false);
        }
        Options[OptionSelected].transform.Find("H").gameObject.SetActive(true);
    }

    void SetHighlightRemote(int i) //Is used don't delete!
    {
        OptionSelected = i;
        SetHighlight();
    }

    void ToggleMenu(bool NewBool)
    {
        Active = NewBool;
        switch (Active)
        {
            case true:
                GetComponent<Image>().enabled = true;
                Holder.SetActive(true);
                OptionSelected = 0;
                SetHighlight();
                Cursor.lockState = CursorLockMode.None;
                break;
            case false:
                GetComponent<Image>().enabled = false;
                Holder.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                break;

            default:
                break;
        }
        
    }
}
