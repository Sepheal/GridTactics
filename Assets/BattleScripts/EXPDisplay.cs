using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Exp Displays control for ingame, needs work

public class EXPDisplay : MonoBehaviour
{
    public GameObject[] UnitBoxs;
    public MonsterDictionary MD;
    public PersistantStats PS;
    public Texture CamTexture;

    public List<UnitListing> UnitList;

    public void Setup(List<UnitListing> Units, MonsterDictionary md, PersistantStats ps)
    {
        MD = md;
        UnitList = Units;
        PS = ps;

        //UnitBoxs[0].SetActive(true);
        //UnitBoxs[0].transform.Find("Prof").GetComponent<Image>().sprite = MD.MonsterSpritesRight[2];
        //UnitBoxs[0].transform.Find("Text").GetComponent<Text>().text = "Lv. " + PS.PlayerLevel;
        //UnitBoxs[0].transform.Find("Bar").GetComponent<Scrollbar>().size = PS.PlayerExp / 100.0f;

        //for (int i = 1; i <= Units.Count; i++)
        //{
        //    UnitBoxs[i].SetActive(true);
        //    UnitBoxs[i].transform.Find("Prof").GetComponent<Image>().sprite = MD.MonsterSpritesRight[Units[i - 1].MonsterId];
        //    UnitBoxs[i].transform.Find("Text").GetComponent<Text>().text = "Lv. " + Units[i - 1].MyLevel;
        //    UnitBoxs[i].transform.Find("Bar").GetComponent<Scrollbar>().size = Units[i - 1].Exp / 100.0f;
        //}
    }

    public IEnumerator DoExp()
    {
        CalculateEXP();
        gameObject.SetActive(true);
        yield return new WaitForSeconds(4);
        FindObjectOfType<GameManager>().EndBattleInstance(true);
    }

    void CalculateEXP()
    {
        int unitcounter = 1;
        string Texto;
        foreach (UnitListing unit in UnitList)
        {
            
            unit.Exp += 70;
            if (unit.Exp >= 100)
            {
                unit.Exp -= 100;
                unit.MyLevel++;
                Texto = "Lv. " + unit.MyLevel + "  (Lvl up!)";
                UnitBoxs[unitcounter].transform.Find("Bar").Find("Image").Find("Anim").GetComponent<Image>().color = new Color(255, 255, 0);
            }
            else
            {
                Texto = "Lv. " + unit.MyLevel;
            }
            UnitBoxs[unitcounter].SetActive(true);
            UnitBoxs[unitcounter].transform.Find("Prof").GetComponent<Image>().sprite = MD.Monsters[unit.MonsterId].MonsterSpriteRight; //MD.MonsterSpritesRight[unit.MonsterId];
            UnitBoxs[unitcounter].transform.Find("Text").GetComponent<Text>().text = Texto;
            UnitBoxs[unitcounter].transform.Find("Bar").GetComponent<Scrollbar>().size = unit.Exp / 100.0f;
            unitcounter++;
        }

        PS.PlayerExp += 70;
        if (PS.PlayerExp >= 100)
        {
            PS.PlayerExp -= 100;
            PS.PlayerLevel++;
            Texto = "Lv. " + PS.PlayerLevel + "  (Lvl up!)";
            UnitBoxs[0].transform.Find("Bar").Find("Image").Find("Anim").GetComponent<Image>().color = new Color(255, 255, 0);
        }
        else
        {
            Texto = "Lv. " + PS.PlayerLevel;
        }

        UnitBoxs[0].SetActive(true);
        UnitBoxs[0].transform.Find("Prof").GetComponent<Image>().sprite = FindObjectOfType<PersistantStats>().HeroImage;
        UnitBoxs[0].transform.Find("Text").GetComponent<Text>().text = Texto;
        UnitBoxs[0].transform.Find("Bar").GetComponent<Scrollbar>().size = PS.PlayerExp / 100.0f;
    }
}
