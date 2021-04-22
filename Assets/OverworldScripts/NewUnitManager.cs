using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewUnitManager : MonoBehaviour
{
    public UnitListing[] UnitListings;
    public GameObject MonsterSpot;
    public MonsterDictionary MD;
    public Text InputNameText;

    public Text SpeciesText, ElementText, SAttacksText, FAttacksText;

    PersistantStats PS;
    public GameObject AttDictPrefab;
    AttackDictionary AD;

    UnitListing NewUnit;
    GameObject UnitBody;

    private void Awake()
    {
        GameObject AttDictObject = Instantiate(AttDictPrefab);
        AD = AttDictObject.GetComponent<AttackDictionary>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        PS = FindObjectOfType<PersistantStats>();

        NewUnit = UnitListings[PS.EncounterIdMessage];
        UnitBody = Instantiate(MD.MonDictionaryPrefabs[NewUnit.MonsterId]);
        UnitBody.transform.position = MonsterSpot.transform.position;
        UnitBody.transform.rotation = MonsterSpot.transform.rotation;

        SpeciesText.text = "Species:\n" + NewUnit.MyName;
        ElementText.text = "Element:\n" + NewUnit.MyElement.ToString();
        string AttackString = "Starting Attacks:";
        int[] AttackList = AD.GetAttacks(NewUnit.MonsterId, NewUnit.MyLevel);
        for (int i = 0; i < AttackList.Length; i++)
        {
            if (AttackList[i] != 0)
            {
                AttackString += "\n-" + AD.AttackList[AttackList[i] - 1].Name;
            }
        }
        //foreach (int Id in NewUnit.AttackList)
        //{
        //    AttackString += "\n-" + AD.AttackList[Id - 1].Name;
        //}
        SAttacksText.text = AttackString;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ConfirmUnit()
    {
        NewUnit.transform.SetParent(PS.gameObject.transform);
        if (InputNameText.text != "") NewUnit.MyName = InputNameText.text;
        StartCoroutine(FindObjectOfType<LevelLoader>().LoadLevel(PS.PreviousSceneName));
    }
}
