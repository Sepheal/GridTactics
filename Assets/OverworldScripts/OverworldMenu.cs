using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverworldMenu : MonoBehaviour
{
    PersistantStats PS;
    MonsterDictionary MonDictionary;
    AttackDictionary attackDictionary;

    public GameObject UnitDisplayPrefab, DexDisplayPrefab, MonsterDictionaryObject, AttackDictionaryObject;

    public GameObject UnitMenu, DexMenu, OptionsMenu;
    public GameObject UnitsList, DexsList;

    // Start is called before the first frame update
    void Start()
    {
        //base setup
        PS = FindObjectOfType<PersistantStats>();
        MonDictionary = (Instantiate(MonsterDictionaryObject)).GetComponent<MonsterDictionary>();
        attackDictionary = (Instantiate(AttackDictionaryObject)).GetComponent<AttackDictionary>();
        gameObject.SetActive(false);

        /////////Setup units menu
        ///player
        GameObject listing = Instantiate(UnitDisplayPrefab, UnitsList.transform);
        listing.transform.Find("Stats").GetComponent<Text>().text = 
            "Lvl " + PS.PlayerLevel + "\n" +
            "Exp " + PS.PlayerExp + "/100" + "\n" +
            "HP " + (100 + (10 * PS.PlayerLevel)) + "\n" +
            "Atk " + (5 * PS.PlayerLevel) + "\n" +
            "Def " + (3 * PS.PlayerLevel) + "\n" +
            "Element " + Element.Neutral.ToString();

        InputField input = listing.transform.Find("NameField").GetComponent<InputField>();
        input.text = PS.PName;
        listing.GetComponent<UnitChangeScript>().Setup(2, PS, null, input);

        string MovesLearnt = "";
        foreach (int attackid in MonDictionary.GetAttacks(2, PS.PlayerLevel))
        {
            if (attackid != 0) MovesLearnt += "-" + attackDictionary.AttackList[attackid - 1].Name + "\n";
        }
        MovesLearnt = MovesLearnt.Substring(0, MovesLearnt.Length - 1);
        listing.transform.Find("Moves").GetComponent<Text>().text = MovesLearnt;

        listing.transform.Find("Image").GetComponent<Image>().sprite = PS.HeroImage;

        ///all other units
        int numofunits = 1;
        foreach (UnitListing unit in FindObjectsOfType<UnitListing>())
        {
            if (unit.Controller == Owner.Player && unit.gameObject.transform.parent == PS.transform)
            {
                listing = Instantiate(UnitDisplayPrefab, UnitsList.transform);
                Monster MyMonster = MonDictionary.Monsters[unit.MonsterId];

                listing.transform.Find("Stats").GetComponent<Text>().text = 
                    "Lvl " + unit.MyLevel + "\n" +
                    "Exp " + unit.Exp + "/100" + "\n" +
                    "HP " + (MyMonster.HPBaseStat + (10 * unit.MyLevel)) + "\n" +
                    "Atk " + (MyMonster.AttackBaseStat * unit.MyLevel) + "\n" +
                    "Def " + (MyMonster.DefenceBaseStat * unit.MyLevel) + "\n" +
                    "Element " + MyMonster.MyElement.ToString();

                input = listing.transform.Find("NameField").GetComponent<InputField>();
                input.text = unit.MyName;
                listing.GetComponent<UnitChangeScript>().Setup(unit.MonsterId, null, unit, input);

                listing.transform.Find("NameField").GetComponent<InputField>().text = unit.MyName;

                MovesLearnt = "";
                foreach (int attackid in MonDictionary.GetAttacks(unit.MonsterId, unit.MyLevel))
                {
                    if (attackid != 0) MovesLearnt += "-" + attackDictionary.AttackList[attackid - 1].Name + "\n";
                }
                MovesLearnt = MovesLearnt.Substring(0, MovesLearnt.Length - 1);
                listing.transform.Find("Moves").GetComponent<Text>().text = MovesLearnt;

                listing.transform.Find("Image").GetComponent<Image>().sprite = MyMonster.MonsterSpriteRight;

                numofunits++;
            }
        }

        //Setup Dex
        foreach (Monster monster in MonDictionary.Monsters)
        {
            listing = Instantiate(DexDisplayPrefab, DexsList.transform);
            if (monster.ID != 2) listing.transform.Find("Image").GetComponent<Image>().sprite = monster.MonsterSpriteRight;
            else listing.transform.Find("Image").GetComponent<Image>().sprite = PS.HeroImage;

            

            if (PS.MonsterDex[monster.ID] == 0)
            {
                listing.transform.Find("Image").GetComponent<Image>().color = new Color(0, 0, 0, 255);
                listing.transform.Find("Stats").GetComponent<Text>().text = "Species: ???\n" +
                "Element: ???";
                listing.transform.Find("Details").GetComponent<Text>().text = "???";
            }
            else
            {
                listing.transform.Find("Stats").GetComponent<Text>().text = "Species: " + monster.Species + "\n" +
                "Element: " + monster.MyElement;

                listing.transform.Find("Details").GetComponent<Text>().text = monster.Description;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UnitsClicked()
    {
        UnitMenu.SetActive(true);
        DexMenu.SetActive(false);
        OptionsMenu.SetActive(false);
    }

    public void MonsterDexClicked()
    {
        UnitMenu.SetActive(false);
        DexMenu.SetActive(true);
        OptionsMenu.SetActive(false);
    }

    public void OptionsClicked()
    {
        UnitMenu.SetActive(false);
        DexMenu.SetActive(false);
        OptionsMenu.SetActive(true);
    }
}
