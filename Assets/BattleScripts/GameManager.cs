using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum Element { Neutral, Rock, Paper, Scissors, Pencil};
public enum Owner { Player, Enemy };
public enum UnitState { Ready, Moving, Attacking, Done, DecidingAttack, DecidingMove, DecidingAction, Passive };

public class GameManager : MonoBehaviour
{
    GameObject TitlePanel, TurnPanel;
    public int GameTurn = 0;
    public bool EnemyIsActive = false;
    public bool ReturnToMap = true;
    bool GameFinished = false; //true while exp is showing
    public string GoToSceneName = "";
    public int EncounterNumber = 0;
    List<EnemyMovement> EnemyActionQueue = new List<EnemyMovement>();

    public GameObject BattleUI;
    GameObject CanvasUI;
    public GameObject MonsterDictionaryObject;
    MonsterDictionary MonDictionary;
    public GameObject HealthUI, AlertUI, ArrowUI;
    public GameObject UIHost;
    public EXPDisplay EX;

    public AudioSource MusicManager;
    public AudioClip BattleMusic, VictoryMusic;

    public UnitListing[] EnemyUnits;
    public UnitListing[] PlayerUnits;

    public List<string> StartDialogue;
    private float PauseTimeStart;
    private readonly float PauseLength = 2.0f;

    private void Awake()
    {
        //Setup dictionaries
        CanvasUI = Instantiate(BattleUI);
        TitlePanel = CanvasUI.transform.Find("TitlePanel").gameObject;
        TurnPanel = CanvasUI.transform.Find("TurnPanel").gameObject;
        GetComponent<DialogueInBattle>().DialogueContainer = CanvasUI.transform.Find("Dialogue").gameObject;
        GetComponent<DialogueInBattle>().DialogueTextBox = CanvasUI.transform.Find("Dialogue").Find("Text").GetComponent<Text>();
        EX = CanvasUI.transform.Find("EXPScreen").GetComponent<EXPDisplay>();
        MonDictionary = (Instantiate(MonsterDictionaryObject)).GetComponent<MonsterDictionary>();
        FindObjectOfType<PlayerStatUIControl>().MonsterList = MonDictionary.Monsters;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Setup Units
        foreach (UnitListing unit in FindObjectsOfType<UnitListing>())
        {
            if (unit.Controller == Owner.Enemy) SpawnUnit(unit);
            else unit.Placed = false;
        }
        //Introduction
        FindObjectOfType<CinematicControl>().Activate();
    }

    public void DecideStart()
    {
        bool HasUnits = false;
        foreach (UnitListing unit in FindObjectsOfType<UnitListing>())
        {
            if (unit.Controller == Owner.Player)
            {
                HasUnits = true;
                break;
            }
        }
        if (HasUnits) StartCoroutine(PlaySetup());
        else
        {
            FindObjectOfType<SelectControl>().Activate();
            FindObjectOfType<CameraControl>().IsPlayerTurn = true;
            SetupOver();
        }
    }

    public IEnumerator PlaySetup()
    {
        TitlePanel.GetComponent<Animator>().SetTrigger("PlayTitle");

        yield return new WaitForSeconds(2);

        FindObjectOfType<MapControl>().SetupMode();
        FindObjectOfType<SelectControl>().SetupMode = true;
        FindObjectOfType<SelectControl>().Activate();
        FindObjectOfType<CameraControl>().IsPlayerTurn = true;
        
    }

    public void SetupOver()
    {
        if (StartDialogue.Count > 0)
        {
            GetComponent<DialogueInBattle>().InitiateDialogue(StartDialogue);
        }
        else
        {
            
            StartGame();
        }
    }

    public void StartGame()
    {
        //music
        MusicManager.clip = BattleMusic;
        MusicManager.Play();
        //
        List<UnitListing> FriendlyUnits = new List<UnitListing>();
        foreach (UnitListing unit in FindObjectsOfType<UnitListing>())
        {
            if (unit.Controller == Owner.Player && unit.Placed) FriendlyUnits.Add(unit);
        }
        EX.Setup(FriendlyUnits, MonDictionary, FindObjectOfType<PersistantStats>());
        ChangeTurn();
    }

    // Update is called once per frame
    void Update()
    {
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

    public void SpawnUnit(UnitListing UnitListing)
    {
        Monster MyMonster = MonDictionary.Monsters[UnitListing.MonsterId];
        GameObject Monster = Instantiate(MyMonster.MonPrefab);
        UnitMovement MM;
        if (UnitListing.Controller == Owner.Player)
        {
            MM = Monster.AddComponent<PlayerMovement>();
            Monster.GetComponent<PlayerMovement>().ArrowUI = Instantiate(ArrowUI, UIHost.transform);
        }
        else
        {
            MM = Monster.AddComponent<EnemyMovement>();
            Monster.transform.Rotate(0, 180, 0);
        }
        //identification stats
        MM.MonsterId = UnitListing.MonsterId;
        MM.MyOwner = UnitListing.Controller;
        MM.PositionTile = UnitListing.StartingTile;
        MM.MyName = UnitListing.MyName;

        //RPG stats
        int level = UnitListing.MyLevel;
        MM.MovementPoints = MyMonster.SpeedBaseStat; //MonDictionary.SpeedBaseStat[UnitListing.MonsterId];
        MM.AttackRange = 1;
        MM.MaxHealth = MyMonster.HPBaseStat + (MyMonster.HPBaseStat / 10 * level);  //MonDictionary.HPBaseStat[UnitListing.MonsterId] + (MonDictionary.HPBaseStat[UnitListing.MonsterId]/10 * level);
        MM.CurrentHealth = MM.MaxHealth;
        MM.AttackStat = MyMonster.AttackBaseStat * level; //MonDictionary.AttackBaseStat[UnitListing.MonsterId] * level;
        MM.DefenceStat = MyMonster.DefenceBaseStat * level; //MonDictionary.DefenceBaseStat[UnitListing.MonsterId] * level;
        MM.Level = level;
        MM.Exp = UnitListing.Exp;

        //Element and Attacks and AI
        MM.MyElement = MyMonster.MyElement; //UnitListing.MyElement;
        MM.AttackIDs = MonDictionary.GetAttacks(UnitListing.MonsterId, level); //AD.GetAttacks(UnitListing.MonsterId, level);
        MM.PassiveLevel = UnitListing.PassiveLevel;

        //Audio files
        MM.TakeDamageSound = MyMonster.TakeDamageSound; //MonDictionary.TakeDamageSound[UnitListing.MonsterId];
        MM.DeathSound = MyMonster.DeathSound; //MonDictionary.DeathSound[UnitListing.MonsterId];
        MM.WalkSound = MyMonster.WalkSound; //MonDictionary.WalkSound[UnitListing.MonsterId];

        //UI settings
        MM.HealthScript = (Instantiate(HealthUI, UIHost.transform)).GetComponent<HealthAnimScript>();
        MM.AlertBox = Instantiate(AlertUI, UIHost.transform);
        MM.AlertText = MM.AlertBox.transform.Find("Text").gameObject;
        //MM.MyBody = MM.transform.Find("Monster").gameObject;

        //Setup for use
        MM.MyState = UnitState.Passive;
        if (MM.MyOwner == Owner.Player)
        {
            //MM.MyBody.GetComponent<SkinnedMeshRenderer>().material.SetColor("_OutlineColor", new Color32(0, 255, 0, 255));
        }
        else
        {
            //MM.MyBody.GetComponent<SkinnedMeshRenderer>().material.SetColor("_OutlineColor", new Color32(255, 0, 0, 255));
            MM.LookDirection = new Vector2(-1, 0);
        }

        UnitListing.MyBody = MM;
        UnitListing.Placed = true;
        MM.MyListing = UnitListing;
    }

    public void ChangeTurn()
    {
        GameTurn++;
        FindObjectOfType<PlayerStatUIControl>().HideStats(4);
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
            //Game Win
            if (EnemyActionQueue.Count == 0)
            {
                CheckBattleEnd();
            }
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
            //Game Lost
            if (PlayerQueue.Count == 0)
            {
                CheckBattleEnd();
            }
        }
    }

    void CheckBattleEnd()
    {
        bool EnemyExist = false, PlayerExist = false, GameEnd = false;
        foreach (EnemyMovement enemy in FindObjectsOfType<EnemyMovement>())
        {
            if (!enemy.MarkedForDeath) EnemyExist = true;
        }
        foreach (PlayerMovement player in FindObjectsOfType<PlayerMovement>())
        {
            if (!player.MarkedForDeath) PlayerExist = true;
        }
        if (!PlayerExist || !EnemyExist) GameEnd = true;

        if (GameEnd)
        {
            GameFinished = true;
            if (!EnemyExist)
            {
                MusicManager.loop = false;
                MusicManager.clip = VictoryMusic;
                MusicManager.Play();

                StartCoroutine(EX.DoExp()); //leads to EndBattleInstance(!EnemyExist);
            }
            else EndBattleInstance(false);
        }
        
    }

    public void EndBattleInstance(bool victory)
    {
        if (victory)
        {
            //PLAYER WON
            FindObjectOfType<PersistantStats>().EncounterList[EncounterNumber] = true;
            if (ReturnToMap)
            {
                StartCoroutine(FindObjectOfType<LevelLoader>().LoadLevel(FindObjectOfType<PersistantStats>().PreviousSceneName));
            }
            else
            {
                FindObjectOfType<PersistantStats>().SpawnId = 0;
                StartCoroutine(FindObjectOfType<LevelLoader>().LoadLevel(GoToSceneName));
            }
        }
        else
        {
            //PLAYER LOST
            Destroy(FindObjectOfType<PersistantStats>().gameObject);
            SceneManager.LoadScene("GameOverScreen");
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
        CheckBattleEnd();

        if (!GameFinished)
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
