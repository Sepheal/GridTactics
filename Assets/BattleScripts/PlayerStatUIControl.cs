using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//UI battle preview script

public class PlayerStatUIControl : MonoBehaviour
{
    public GameObject StatUIParent, StatUIParent2;
    public Image ProfileImage, ProfileImage2;
    public Text UnitName, UnitStats, UnitName2, UnitStats2, UnitHealthText, UnitHealthText2;
    public GameObject HPFront, HPFront2, HPDiff2;
    public Sprite[] MonsterSpritesRight;
    public Sprite[] MonsterSpritesLeft;
    public List<Monster> MonsterList;
    public GameObject DamageHolder;
    public GameObject[] DamageBoxs;

    public RawImage TextureImage;

    public void SetValues(UnitMovement Unit, int Scenario)
    {
        Image Prof;
        Text UnName, UnStats, UnHealthText;
        GameObject HP, ParentBox;
        if (Scenario == 0 && Unit.MyOwner == Owner.Player)
        {
            UnName = UnitName;
            UnStats = UnitStats;
            HP = HPFront;
            ParentBox = StatUIParent;
            UnHealthText = UnitHealthText;
            Prof = ProfileImage;
            if (Unit.MonsterId != 2)
            {
                Prof.enabled = true;
                TextureImage.enabled = false;
                Prof.sprite = MonsterList[Unit.MonsterId].MonsterSpriteRight; //MonsterSpritesRight[Unit.MonsterId];
            }
            else
            {
                //Prof.enabled = false;
                //TextureImage.enabled = true;

                Prof.enabled = true;
                TextureImage.enabled = false;
                Prof.sprite = FindObjectOfType<PersistantStats>().HeroImage;
            }
            StatUIParent.SetActive(true);
        }
        else
        {
            UnName = UnitName2;
            UnStats = UnitStats2;
            HP = HPFront2;
            ParentBox = StatUIParent2;
            UnHealthText = UnitHealthText2;
            Prof = ProfileImage2;
            Prof.sprite = MonsterList[Unit.MonsterId].MonsterSpriteLeft; //MonsterSpritesLeft[Unit.MonsterId];
            StatUIParent2.SetActive(true);
        }
        object[] Stats = Unit.GetStats();
        string s = "Lv: " + Stats[7]; 
        s += "\nAt: " + Stats[3];
        s += "\nDf: " + Stats[4];
        s += "\nMv: " + Stats[5];
        
        UnName.text = Stats[0].ToString();
        UnStats.text = s;
        UnHealthText.text = Stats[2].ToString() + "/" + Stats[1].ToString();
        float Percent = Convert.ToSingle(Stats[2]) / Convert.ToSingle(Stats[1]);
        HP.GetComponent<RectTransform>().sizeDelta = new Vector2(Percent * 800, 100);
        switch (Unit.MyElement)
        {
            case Element.Neutral:
                ParentBox.GetComponent<Image>().color = new Color32(0, 0, 0, 70);
                break;
            case Element.Rock:
                ParentBox.GetComponent<Image>().color = new Color32(0, 0, 255, 70);
                break;
            case Element.Paper:
                ParentBox.GetComponent<Image>().color = new Color32(0, 255, 0, 70);
                break;
            case Element.Scissors:
                ParentBox.GetComponent<Image>().color = new Color32(255, 0, 0, 70);
                break;
            case Element.Pencil:
                ParentBox.GetComponent<Image>().color = new Color32(255, 120, 0, 70);
                break;
            default:
                break;
        }
    }

    public void HideStats(int id)
    {
        switch (id)
        {
            case 1: StatUIParent.SetActive(false); break;
            case 2: StatUIParent2.SetActive(false); break;
            case 3:
                DamageHolder.SetActive(false);
                foreach (GameObject item in DamageBoxs)
                {
                    item.SetActive(false);
                }
                break;
            case 4: StatUIParent.SetActive(false); StatUIParent2.SetActive(false); break;
            default:
                break;
        }
    }

    public void ShowDamagePreview(List<UnitMovement> units, List<int> damages)
    {
        HideStats(2);

        DamageHolder.SetActive(true);
        foreach (GameObject item in DamageBoxs)
        {
            item.SetActive(false);
        }
        GameObject Box = DamageBoxs[units.Count - 1];
        Box.SetActive(true);
        switch (units.Count)
        {
            case 1:

                SetTargetBox(units[0], damages[0],
                    Box.transform.Find("Profile1").GetComponent<Image>(),
                    Box.transform.Find("HPBar1").Find("HPFront").gameObject,
                    Box.transform.Find("HPBar1").Find("HPDifference").gameObject,
                    Box.transform.Find("HPBar1").GetComponent<RectTransform>().sizeDelta.x);

                break;
            case 2:

                SetTargetBox(units[0], damages[0],
                    Box.transform.Find("Profile1").GetComponent<Image>(),
                    Box.transform.Find("HPBar1").Find("HPFront").gameObject,
                    Box.transform.Find("HPBar1").Find("HPDifference").gameObject,
                    Box.transform.Find("HPBar1").GetComponent<RectTransform>().sizeDelta.x);

                SetTargetBox(units[1], damages[1],
                    Box.transform.Find("Profile2").GetComponent<Image>(),
                    Box.transform.Find("HPBar2").Find("HPFront").gameObject,
                    Box.transform.Find("HPBar2").Find("HPDifference").gameObject,
                    Box.transform.Find("HPBar2").GetComponent<RectTransform>().sizeDelta.x);

                break;
            case 3:

                SetTargetBox(units[0], damages[0],
                    Box.transform.Find("Profile1").GetComponent<Image>(),
                    Box.transform.Find("HPBar1").Find("HPFront").gameObject,
                    Box.transform.Find("HPBar1").Find("HPDifference").gameObject,
                    Box.transform.Find("HPBar1").GetComponent<RectTransform>().sizeDelta.x);

                SetTargetBox(units[1], damages[1],
                    Box.transform.Find("Profile2").GetComponent<Image>(),
                    Box.transform.Find("HPBar2").Find("HPFront").gameObject,
                    Box.transform.Find("HPBar2").Find("HPDifference").gameObject,
                    Box.transform.Find("HPBar2").GetComponent<RectTransform>().sizeDelta.x);

                SetTargetBox(units[2], damages[2],
                    Box.transform.Find("Profile3").GetComponent<Image>(),
                    Box.transform.Find("HPBar3").Find("HPFront").gameObject,
                    Box.transform.Find("HPBar3").Find("HPDifference").gameObject,
                    Box.transform.Find("HPBar3").GetComponent<RectTransform>().sizeDelta.x);

                break;
            default:
                break;
        }
    }

    void SetTargetBox(UnitMovement unit, int dmg, Image PImage, GameObject HPF, GameObject HPD, float BarSize)
    {
        float PercentLeft = (Convert.ToSingle(unit.CurrentHealth) - dmg) / Convert.ToSingle(unit.MaxHealth);
        float PercentBeforeDamage = Convert.ToSingle(unit.CurrentHealth) / Convert.ToSingle(unit.MaxHealth);
        float Diff = dmg / Convert.ToSingle(unit.MaxHealth);
        if (Diff > PercentBeforeDamage) { Diff = PercentBeforeDamage; PercentLeft = 0f; }

        HPF.GetComponent<RectTransform>().sizeDelta = new Vector2(PercentLeft * BarSize, 1);
        HPD.GetComponent<RectTransform>().sizeDelta = new Vector2(Diff * BarSize, 1);

        if (unit.MonsterId != 2) PImage.sprite = MonsterList[unit.MonsterId].MonsterSpriteLeft; //MonsterSpritesLeft[unit.MonsterId];
        else PImage.sprite = FindObjectOfType<PersistantStats>().HeroImage;

        float XPos = HPF.GetComponent<RectTransform>().localPosition.x - HPF.GetComponent<RectTransform>().sizeDelta.x;
        HPD.GetComponent<RectTransform>().localPosition = new Vector3(XPos, 0, 0);
    }
}
