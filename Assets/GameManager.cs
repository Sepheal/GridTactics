using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Element { Neutral, Rock, Paper, Scissors, Pencil};
public enum Owner { Player, Enemy };
public enum UnitState { Ready, Moving, Attacking, Done, DecidingAttack, DecidingMove, DecidingAction, Passive };

public class Listing
{
    public Owner Controller;
    public int MonsterId;
    public Tile StartingTile;
    public Stats UnitStats;
}

public class Stats
{
    public string MyName;
    public int MovementPoints, AttackRange, MaxHealth, AttackStat, DefenceStat;
    public Element MyElement;
    public int[] AttackList;
}

public class GameManager : MonoBehaviour
{
    public GameObject TitlePanel, TurnPanel;
    public int GameTurn = 0;
    public bool EnemyIsActive = false;
    List<EnemyMovement> EnemyActionQueue = new List<EnemyMovement>();

    public GameObject[] MonsterPrefabs;
    public Material[] SlimeMaterials, SpikyMaterials;
    public Texture[] MonsterTextures;
    public GameObject HealthUI, AlertUI;
    public GameObject UIHost;

    public Listing[] PlayerUnits;

    float PauseTimeStart, PauseLength = 3.0f;
    float StartGameTime, StartGameDelayLength = 3.0f;
    bool StartGameDelayOver = false;

    // Start is called before the first frame update
    void Start()
    {
        TitlePanel.GetComponent<Animator>().SetTrigger("PlayTitle");
        Listing NewListing;
        Stats ListingStats;
        ///////////////////////////////////////////////////////////////////////////////PLAYERS
        {
            NewListing = new Listing
            {
                Controller = Owner.Player,
                MonsterId = 0
            };
            ListingStats = new Stats
            {
                MyName = "Slime",
                MovementPoints = 2,
                AttackRange = 1,
                MaxHealth = 100,
                AttackStat = 2,
                DefenceStat = 1,
                MyElement = Element.Paper,
                AttackList = new int[] { 1, 3 }
            };
            NewListing.UnitStats = ListingStats;
            NewListing.StartingTile = FindObjectOfType<MapControl>().FindTile(new Vector2(-3, 0));
            SpawnUnit(NewListing);
            ///////////////////////////////////////////////////////////////////////////////
            NewListing = new Listing
            {
                Controller = Owner.Player,
                MonsterId = 0
            };
            ListingStats = new Stats
            {
                MyName = "Slime",
                MovementPoints = 2,
                AttackRange = 1,
                MaxHealth = 100,
                AttackStat = 2,
                DefenceStat = 1,
                MyElement = Element.Scissors,
                AttackList = new int[] { 1, 2 }
            };
            NewListing.UnitStats = ListingStats;
            NewListing.StartingTile = FindObjectOfType<MapControl>().FindTile(new Vector2(-3, 5));
            SpawnUnit(NewListing);
            ///////////////////////////////////////////////////////////////////////////////
            NewListing = new Listing
            {
                Controller = Owner.Player,
                MonsterId = 1
            };
            ListingStats = new Stats
            {
                MyName = "Shell",
                MovementPoints = 2,
                AttackRange = 1,
                MaxHealth = 100,
                AttackStat = 2,
                DefenceStat = 1,
                MyElement = Element.Rock,
                AttackList = new int[] { 1, 4 }
            };
            NewListing.UnitStats = ListingStats;
            NewListing.StartingTile = FindObjectOfType<MapControl>().FindTile(new Vector2(-3, 2));
            SpawnUnit(NewListing);
        }
        ///////////////////////////////////////////////////////////////////////////////ENEMIES
        {
            NewListing = new Listing
            {
                Controller = Owner.Enemy,
                MonsterId = 0
            };
            ListingStats = new Stats
            {
                MyName = "TestSlime",
                MovementPoints = 2,
                AttackRange = 1,
                MaxHealth = 100,
                AttackStat = 2,
                DefenceStat = 1,
                MyElement = Element.Rock,
                AttackList = new int[] { 1, 3 }
            };
            NewListing.UnitStats = ListingStats;
            NewListing.StartingTile = FindObjectOfType<MapControl>().FindTile(new Vector2(6, 0));
            SpawnUnit(NewListing);
            ///////////////////////////////////////////////////////////////////////////////
            NewListing = new Listing
            {
                Controller = Owner.Enemy,
                MonsterId = 1
            };
            ListingStats = new Stats
            {
                MyName = "TestSlime",
                MovementPoints = 2,
                AttackRange = 1,
                MaxHealth = 100,
                AttackStat = 2,
                DefenceStat = 1,
                MyElement = Element.Scissors,
                AttackList = new int[] { 1, 2 }
            };
            NewListing.UnitStats = ListingStats;
            NewListing.StartingTile = FindObjectOfType<MapControl>().FindTile(new Vector2(2, 1));
            SpawnUnit(NewListing);
            ///////////////////////////////////////////////////////////////////////////////

            NewListing = new Listing
            {
                Controller = Owner.Enemy,
                MonsterId = 1
            };
            ListingStats = new Stats
            {
                MyName = "TestSlime",
                MovementPoints = 2,
                AttackRange = 1,
                MaxHealth = 100,
                AttackStat = 2,
                DefenceStat = 1,
                MyElement = Element.Paper,
                AttackList = new int[] { 1, 3 }
            };
            NewListing.UnitStats = ListingStats;
            NewListing.StartingTile = FindObjectOfType<MapControl>().FindTile(new Vector2(6, 4));
            SpawnUnit(NewListing);
        }
        ///////////////////////////////////////////////////////////////////////////////
        //Player needs to set up their team position first BUT for now just start game immediately
        //ChangeTurn();
        StartGameTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (!StartGameDelayOver && Time.time - StartGameTime >= StartGameDelayLength)
        {
            ChangeTurn();
            StartGameDelayOver = true;
        }

        if (Time.time - PauseTimeStart >= PauseLength)
        {
            if (EnemyActionQueue.Count > 0)
            {
                if (EnemyActionQueue[0].MyState == UnitState.Done)
                {
                    EnemyActionQueue.RemoveAt(0);
                }
                else if (EnemyActionQueue[0].MyState == UnitState.Ready && !CheckForDead())
                {
                    EnemyActionQueue[0].DecideMovement();
                }
            }
        }
    }

    void SpawnUnit(Listing UnitListing)
    {
        GameObject Monster = Instantiate(MonsterPrefabs[UnitListing.MonsterId]);
        UnitMovement MM;
        if (UnitListing.Controller == Owner.Player)
        {
            MM = Monster.AddComponent<PlayerMovement>();
        }
        else
        {
            MM = Monster.AddComponent<EnemyMovement>();
            Monster.transform.Rotate(0, 180, 0);
        }
        MM.MonsterId = UnitListing.MonsterId;
        MM.MyOwner = UnitListing.Controller;
        MM.PositionTile = UnitListing.StartingTile;
        MM.MyName = UnitListing.UnitStats.MyName;
        MM.MovementPoints = UnitListing.UnitStats.MovementPoints;
        MM.AttackRange = UnitListing.UnitStats.AttackRange;
        MM.MaxHealth = UnitListing.UnitStats.MaxHealth;
        MM.CurrentHealth = UnitListing.UnitStats.MaxHealth;
        MM.AttackStat = UnitListing.UnitStats.AttackStat;
        MM.DefenceStat = UnitListing.UnitStats.DefenceStat;
        MM.MyElement = UnitListing.UnitStats.MyElement;
        MM.AttackIDs = UnitListing.UnitStats.AttackList;
        MM.HealthScript = (Instantiate(HealthUI, UIHost.transform)).GetComponent<HealthAnimScript>();
        MM.AlertBox = Instantiate(AlertUI, UIHost.transform);
        MM.AlertText = MM.AlertBox.transform.Find("Text").gameObject;
        MM.MyBody = MM.transform.Find("Monster").gameObject;
        MM.MyState = UnitState.Passive;
        if (MM.MyOwner == Owner.Player)
        {
            //MM.MyBody.GetComponent<SkinnedMeshRenderer>().material = PlayerMaterials[MM.MonsterId];
            MM.MyBody.GetComponent<SkinnedMeshRenderer>().material.SetColor("_OutlineColor", new Color32(0, 255, 0, 255));
        }
        else
        {
            MM.MyBody.GetComponent<SkinnedMeshRenderer>().material.SetColor("_OutlineColor", new Color32(255, 0, 0, 255));
            MM.LookDirection = new Vector2(-1, 0);
            //MM.MyBody.GetComponent<SkinnedMeshRenderer>().material = EnemyMaterials[MM.MonsterId];
        }
        /*
        switch (MM.MyElement)
        {
            case Element.Neutral:
                break;
            case Element.Rock:
                switch (MM.MonsterId)
                {
                    case 0: MM.MyBody.GetComponent<SkinnedMeshRenderer>().material.SetTexture("_MainTex", MonsterTextures[1]); break; //Slime Blue
                    case 1: MM.MyBody.GetComponent<SkinnedMeshRenderer>().material.SetTexture("_MainTex", MonsterTextures[0]); break; //Shell Blue
                    default: break;
                }
                break;
            case Element.Paper:
                switch (MM.MonsterId)
                {
                    case 0: MM.MyBody.GetComponent<SkinnedMeshRenderer>().material.SetTexture("_MainTex", MonsterTextures[2]); break; //Slime Green
                    case 1: MM.MyBody.GetComponent<SkinnedMeshRenderer>().material.SetTexture("_MainTex", MonsterTextures[1]); break; //Shell Green
                    default: break;
                }
                break;
            case Element.Scissors:
                switch (MM.MonsterId)
                {
                    case 0: MM.MyBody.GetComponent<SkinnedMeshRenderer>().material.SetTexture("_MainTex", MonsterTextures[0]); break; //Slime Green
                    case 1: MM.MyBody.GetComponent<SkinnedMeshRenderer>().material.SetTexture("_MainTex", MonsterTextures[2]); break; //Shell Green
                    default: break;
                }
                break;
            case Element.Pencil:
                break;
            default:
                break;
        }
        */
    }

    public void ChangeTurn()
    {
        GameTurn++;
        if (GameTurn % 2 == 0)
        {
            print("Enemy turn");
            FindObjectOfType<SelectControl>().Active = false;
            SetTurnPanel(false);
            FindObjectOfType<CameraControl>().IsPlayerTurn = false;
            foreach (PlayerMovement player in FindObjectsOfType<PlayerMovement>())
            {
                if (player.MyState == UnitState.Done && !player.MarkedForDeath)
                {
                    player.MyState = UnitState.Passive;
                    player.SetMyColor();
                }
            }
            foreach (EnemyMovement enemy in FindObjectsOfType<EnemyMovement>())
            {
                if (enemy.MyState == UnitState.Passive && !enemy.MarkedForDeath)
                {
                    enemy.MyState = UnitState.Ready;
                    enemy.SetMyColor();
                }
            }

            //TODO: start enemy AI
            EnemyActionQueue = new List<EnemyMovement>();
            foreach (EnemyMovement enemy in FindObjectsOfType<EnemyMovement>())
            {
                if (enemy.MyState == UnitState.Ready) EnemyActionQueue.Add(enemy);
            }
            if (EnemyActionQueue.Count == 0) CheckTurnEnd(); //actually game over and the player won
        }
        else
        {
            print("Player turn");
            FindObjectOfType<SelectControl>().Activate();
            SetTurnPanel(true);
            FindObjectOfType<CameraControl>().IsPlayerTurn = true;
            foreach (PlayerMovement player in FindObjectsOfType<PlayerMovement>())
            {
                if (player.MyState == UnitState.Passive && !player.MarkedForDeath)
                {
                    player.MyState = UnitState.Ready;
                    player.SetMyColor();
                }
            }
            foreach (EnemyMovement enemy in FindObjectsOfType<EnemyMovement>())
            {
                if (enemy.MyState == UnitState.Done && !enemy.MarkedForDeath)
                {
                    enemy.MyState = UnitState.Passive;
                    enemy.SetMyColor();
                }
            }
            List<PlayerMovement> PlayerQueue = new List<PlayerMovement>();
            foreach (PlayerMovement player in FindObjectsOfType<PlayerMovement>())
            {
                if (!player.MarkedForDeath) PlayerQueue.Add(player);
            }
            if (PlayerQueue.Count == 0) CheckTurnEnd(); //actually game over and the player lost
        }
    }

    void SetTurnPanel(bool IsPlayer)
    {
        if (IsPlayer)
        {
            TurnPanel.transform.Find("Image").GetComponent<Image>().color = new Color32(0, 255, 0, 255);
            TurnPanel.transform.Find("Text").GetComponent<Text>().text = "Player's turn";
        }
        else
        {
            TurnPanel.transform.Find("Image").GetComponent<Image>().color = new Color32(255, 0, 0, 255);
            TurnPanel.transform.Find("Text").GetComponent<Text>().text = "Enemy's turn";
        }
        PauseTimeStart = Time.time;
        TurnPanel.GetComponent<Animator>().SetTrigger("PlayTitle");
    }

    public void CheckTurnEnd()
    {
        bool CanEnd = true;
        foreach (UnitMovement unit in FindObjectsOfType<UnitMovement>())
        {
            if (!(unit.MyState == UnitState.Done || unit.MyState == UnitState.Passive))
            {
                CanEnd = false;
            }
        }
        if (CanEnd) ChangeTurn();
    }

    public bool CheckForDead()
    {
        bool IsDead = false;
        foreach (UnitMovement unit in FindObjectsOfType<UnitMovement>())
        {
            if (unit.MarkedForDeath){ IsDead = true; break;}
        }
        return IsDead;
    }
}
