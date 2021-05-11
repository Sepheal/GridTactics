using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupHero : MonoBehaviour
{
    public GameObject DictionaryPrefab;
    public GameObject HealthUIPrefab, AlertUIPrefab, UIHost;
    HeroDictionary Dictionary;

    PersistantStats PS;

    private void Awake()
    {
        Dictionary = (Instantiate(DictionaryPrefab)).GetComponent<HeroDictionary>();
    }

    // Start is called before the first frame update
    void Start()
    {
        PS = FindObjectOfType<PersistantStats>();
        UpdateHero(PS.HeroId, PS.SpawnId);
    }

    public void UpdateHero(int HeroId, int SpawnId)
    {
        //Spawn plain Hero
        GameObject PlayerAvatar = Instantiate(Dictionary.HeroPrefabs[3]);

        //Update Costume
        CostumeIDs CI = PlayerAvatar.GetComponent<CostumeIDs>();
        CI.Faces[PS.HeroCloths[1]].SetActive(true);
        if (CI.Headgears[PS.HeroCloths[2]]) CI.Headgears[PS.HeroCloths[2]].SetActive(true);
        CI.Torsos[PS.HeroCloths[3]].SetActive(true);
        CI.Shoes[PS.HeroCloths[4]].SetActive(true);
        CI.Gloves[PS.HeroCloths[5]].SetActive(true);
        if (CI.Shoulders[PS.HeroCloths[6]]) CI.Shoulders[PS.HeroCloths[6]].SetActive(true);
        if (CI.Belts[PS.HeroCloths[7]]) CI.Belts[PS.HeroCloths[7]].SetActive(true);
        switch (PlayerAvatar.GetComponent<CostumeIDs>().HatToHairIds[PS.HeroCloths[2]])
        {
            case 0:
                //Show full hair
                CI.Hairs[PS.HeroCloths[0]].SetActive(true);
                break;
            case 1:
                //Show Half hair
                CI.Hairs[PS.HeroCloths[0] + 5].SetActive(true);
                break;
            case 2:
                //show no hair
                break;
            default:
                break;
        }

        //Setup controllers
        if (FindObjectOfType<InputController>()) //overworld
        {
            FindObjectOfType<InputController>().MyCharacter = PlayerAvatar;
            FindObjectOfType<InputController>().SetupControls();
            FindObjectOfType<CameraControlOverworld>().Target = PlayerAvatar;
            FindObjectOfType<DialogueOverworld>().PlayerAnimator = PlayerAvatar.GetComponent<Animator>();
        }
        else if (FindObjectOfType<GameManager>()) //ingame
        {
            foreach (Transform item in PlayerAvatar.transform.Find("root"))
            {
                item.gameObject.SetActive(true);
            }
            PlayerMovement PM = PlayerAvatar.AddComponent<PlayerMovement>();
            PM.MonsterId = 2;
            PM.MyName = PS.PName;
            //reworked
            /*
            PM.MovementPoints = PS.MP;
            PM.AttackRange = PS.AR;
            PM.MaxHealth = PS.MaxHp;
            PM.CurrentHealth = PS.MaxHp;
            PM.AttackStat = PS.Attack;
            PM.DefenceStat = PS.Defence;
            */
            PM.MovementPoints = 2;
            PM.AttackRange = 1;
            PM.MaxHealth = 100 + (10* PS.PlayerLevel);
            PM.CurrentHealth = PM.MaxHealth;
            PM.AttackStat = 5 * PS.PlayerLevel;
            PM.DefenceStat = 3 * PS.PlayerLevel;
            PM.Level = PS.PlayerLevel;
            PM.Exp = PS.PlayerExp;

            PM.MyElement = PS.PElement;
            PM.AttackIDs = FindObjectOfType<MonsterDictionary>().GetAttacks(2, PM.Level);

            //Audio files
            PM.TakeDamageSound = Dictionary.TakeDamageSound;
            PM.DeathSound = Dictionary.DeathSound;
            PM.WalkSound = Dictionary.WalkSound;

            PM.LookDirection = new Vector2(1, 0);
            PM.StartTurnLookDirection = new Vector2(1, 0);
            foreach (Tile item in FindObjectsOfType<Tile>())
            {
                if (item.StartingTileId == 2)
                {
                    PM.PositionTile = item;
                    break;
                }
            }
            PM.HealthScript = (Instantiate(HealthUIPrefab, UIHost.transform)).GetComponent<HealthAnimScript>();
            PM.AlertBox = Instantiate(AlertUIPrefab, UIHost.transform);
            PM.AlertText = PM.AlertBox.transform.Find("Text").gameObject;
            PM.ArrowUI = UIHost.transform.Find("ArrowUI").gameObject;
            PM.MyState = UnitState.Passive;
        }
    }
}
