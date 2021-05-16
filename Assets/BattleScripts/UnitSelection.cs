using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//UI control for the unit placement phase of battle, shows list of units you could place

public class UnitSelection : MonoBehaviour
{
    public GameObject[] UnitChoices;
    public GameObject CancelButton;
    public GameObject Confirmation;
    public GameObject[] ConfirmHighs;
    Tile TileSelected;

    List<GameObject> OptionList;
    int NumListed = 1, NumListedConfirm = 0;
    bool Active = false, FrameBuffer = false;
    float CooldownStart; readonly float FrameCooldown = 0.2f;

    bool ConfirmScreen = false;

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
                    if (!ConfirmScreen)
                    {
                        if (Input.GetAxisRaw("Vertical") > 0) //|| Input.GetAxisRaw("Mouse Y") > 0)
                        {
                            NumListed--;
                            if (NumListed < 0) NumListed = 0; //NumListed = OptionList.Count - 1;
                            SetHighlight();
                            CooldownStart = Time.time;
                        }
                        else if (Input.GetAxisRaw("Vertical") < 0)//|| Input.GetAxisRaw("Mouse Y") < 0)
                        {
                            NumListed++;
                            if (NumListed == OptionList.Count) NumListed = OptionList.Count - 1; //NumListed = 0;
                            SetHighlight();
                            CooldownStart = Time.time;
                        }
                    }
                    else
                    {
                        if (Input.GetAxisRaw("Horizontal") != 0)
                        {
                            NumListedConfirm++;
                            if (NumListedConfirm > 1) NumListedConfirm = 0;
                            SetConfirmHighlight();
                            CooldownStart = Time.time;
                        }
                    }
                }
                if (!ConfirmScreen)
                {
                    //Select button
                    if (Input.GetButtonDown("Select"))
                    {
                        if (OptionList[NumListed].name == "Cancel")
                        {
                            Cancel();
                        }
                        else
                        {
                            ChosenUnit();
                        }
                    }
                    //Cancel button
                    else if (Input.GetButtonDown("Cancel"))
                    {
                        Cancel();
                    }
                }
                else
                {
                    //Select button
                    if (Input.GetButtonDown("Select"))
                    {
                        if (NumListedConfirm == 0)
                        {
                            Confirm(true);
                        }
                        else
                        {
                            Confirm(false);
                        }
                        Active = false;
                    }
                    //Cancel button
                    else if (Input.GetButtonDown("Cancel"))
                    {
                        Confirm(false);
                        Active = false;
                    }
                }
                
            }
        }
        else FrameBuffer = true;
    }

    public void Confirm(bool Answer)
    {
        switch (Answer)
        {
            case true:
                GetComponent<AudioSource>().PlayOneShot(ConfirmAudio2);
                FindObjectOfType<MegaMenuControl>().StartCombat();
                break;
            case false:
                break;
        }
        Cancel();
        Confirmation.SetActive(false);
        ConfirmScreen = false;
    }

    public void ChosenUnit()
    {
        GetComponent<AudioSource>().PlayOneShot(ConfirmAudio);
        OptionList[NumListed].GetComponent<OnHighlightUI>().MyAssignedUnit.StartingTile = TileSelected;
        FindObjectOfType<GameManager>().SpawnUnit(OptionList[NumListed].GetComponent<OnHighlightUI>().MyAssignedUnit);
        if (OptionList.Count == 2) //final unit placed
        {
            foreach (Transform item in transform)
            {
                item.gameObject.SetActive(false);
            }
            Confirmation.SetActive(true);
            ConfirmScreen = true;
            NumListedConfirm = 0;
            SetConfirmHighlight();
            //CooldownStart = Time.time;
        }
        else
        {
            Cancel();
        }
    }

    public void Cancel()
    {
        GetComponent<AudioSource>().PlayOneShot(CancelAudio);
        foreach (Transform item in transform)
        {
            item.gameObject.SetActive(false);
        }
        FindObjectOfType<SelectControl>().Activate();
        Deactivate();
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

    void SetConfirmHighlight()
    {
        switch (NumListedConfirm)
        {
            case 0:
                ConfirmHighs[0].SetActive(true);
                ConfirmHighs[1].SetActive(false);
                break;
            case 1:
                ConfirmHighs[0].SetActive(false);
                ConfirmHighs[1].SetActive(true);
                break;
            default:
                break;
        }
        //GetComponent<AudioSource>().PlayOneShot(ChangeAudio);
    }

    public void SetHighlightRemote(int i) //Is used don't delete!
    {
        if (i <= 19)
        {
            NumListed = i;
            SetHighlight();
        }
        else
        {
            NumListedConfirm = i - 20;
            SetConfirmHighlight();
        }
        
    }

    public void Activate(Tile t, PlayerMovement P)
    {
        P.MyListing.Placed = false;
        P.MyListing.MyBody = null;
        Destroy(P.ArrowUI);
        Destroy(P.HealthScript.gameObject);
        Destroy(P.AlertBox);
        Destroy(P.gameObject);
        Activate(t);
    }

    public void Activate(Tile t)
    {
        TileSelected = t;

        int NumUnitsToList = 0;
        foreach (UnitListing unit in FindObjectsOfType<UnitListing>())
        {
            if (unit.Controller == Owner.Player && !unit.Placed)
            {
                UnitChoices[NumUnitsToList].SetActive(true);
                UnitChoices[NumUnitsToList].GetComponent<OnHighlightUI>().MyAssignedUnit = unit;
                UnitChoices[NumUnitsToList].transform.Find("Text").GetComponent<Text>().text = unit.MyName;
                NumUnitsToList++;
            }
        }
        CancelButton.SetActive(true);

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
    }

    public void Deactivate()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Active = false;
    }
}
