using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Base unitmovement script for all battle units, gets branched into PlayerMovement and EnemyMovements based on the owner, handles all unit actions and states

public class UnitMovement : MonoBehaviour
{
    public Tile PositionTile, DestinationTile, StartTurnTile, TargetTile;
    public List<Tile> Waypoints;
    int WaypointId;
    public Vector2 LookDirection = new Vector2(1, 0), StartTurnLookDirection = new Vector2(1, 0);
    float WaypointTime; readonly float WaypointCooldown = 0.5f;
    float AttackStartTime; readonly float AttackLength = 1.0f;
    float DyingStartTime; readonly float DyingAnimLength = 2.0f; public bool MarkedForDeath = false;
    
    public int[] AttackIDs;
    public int ActiveAttackId;

    //Script presets
    Vector3 PosOffset = new Vector3(0, 4f, 0);

    //STATS
    public int MonsterId;
    public string MyName = "Bob";
    public int MovementPoints, AttackRange, MaxHealth = 100, CurrentHealth = 100, AttackStat = 2, DefenceStat = 1, Level = 1, Exp = 0;
    public int PassiveLevel = 1;
    public UnitState MyState = UnitState.Done;
    public Owner MyOwner;
    public Element MyElement = Element.Neutral;
    //UI STUFFS
    public GameObject AlertBox, AlertText;
    //public GameObject MyBody;
    public HealthAnimScript HealthScript;

    //Reverse REF
    public UnitListing MyListing;

    //Audio holders
    public AudioClip TakeDamageSound, DeathSound, WalkSound;

    //Find Object References
    protected MapControl mapControl;

    // Start is called before the first frame update
    public virtual void Start()
    {
        //if (MonsterId == 0 || MonsterId == 1 || MonsterId == 2 || MonsterId == 3)
        PosOffset = new Vector3(0, 2.5f, 0);
        gameObject.transform.position = PositionTile.transform.position + PosOffset;
        //SetTileOccupy(true, false);
        StartTurnTile = PositionTile;

        mapControl = FindObjectOfType<MapControl>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (MyState == UnitState.Moving)
        {
            //MOVING CODE
            if (PositionTile != DestinationTile)
            {
                if (Time.time - WaypointTime >= WaypointCooldown)
                {
                    PositionTile = Waypoints[WaypointId + 1];
                    gameObject.transform.position = PositionTile.transform.position + PosOffset;
                    WaypointId++;
                    WaypointTime = Time.time;

                }
                else
                {
                    Vector3 Direction = (Waypoints[WaypointId + 1].transform.position - Waypoints[WaypointId].transform.position);
                    gameObject.transform.position = Waypoints[WaypointId].transform.position + (Direction * ((Time.time - WaypointTime) / WaypointCooldown)) + PosOffset;
                    gameObject.transform.LookAt(Waypoints[WaypointId + 1].transform.position + PosOffset);
                    //LookDirection = new Vector2(Direction.x, Direction.z);
                }
                SetCamera();
            }
            //ARRIVED AT DESTINATION CODE
            else
            {
                PositionTile = DestinationTile;
                EndMovement();
            }
        }
        else if (MyState == UnitState.Attacking)
        {
            //ATTACK CODE
            float factor = (Time.time - AttackStartTime) / AttackLength;
            if (factor <= 1.0f)
            {
                //gameObject.transform.Rotate(new Vector3(0, 5.0f, 0));
            }
            //FINISHED ATTACK CODE
            else
            {
                //gameObject.transform.rotation = new Quaternion();
                List<UnitMovement> Except = new List<UnitMovement> { this };
                FindObjectOfType<MapControl>().DoDamage(ActiveAttackId, TargetTile, Except, AttackStat, MyElement);
                FindObjectOfType<MapControl>().StopAttackParticles(ActiveAttackId, TargetTile, PositionTile);
                EndPawnTurn();
            }
        }
        if (MarkedForDeath)
        {
            //Dying animation
            float factor = (Time.time - DyingStartTime) / DyingAnimLength;
            if (factor <= 1.0f)
            {
                byte alpha = (byte)(255 * (1 - factor));
                byte color = (byte)(50 * (1 - factor));
                //print(alpha);
                //gameObject.GetComponent<MeshRenderer>().material.color = new Color32(color, color, color, alpha);
                //if (MyBody.GetComponent<MeshRenderer>()) MyBody.GetComponent<MeshRenderer>().material.color = new Color32(color, color, color, alpha);
                //else
                //{
                    
                //}
                //else if (MyBody.GetComponent<SkinnedMeshRenderer>()) MyBody.GetComponent<SkinnedMeshRenderer>().material.color = new Color32(color, color, color, alpha);
            }
            else
            {
                ///Add to dex
                if (MyOwner == Owner.Enemy)
                {
                    if (FindObjectOfType<PersistantStats>().MonsterDex[MonsterId] == 0) FindObjectOfType<PersistantStats>().MonsterDex[MonsterId] = 1;
                }

                Destroy(this.gameObject);
            }
        }
    }

    public virtual void EndPawnTurn()
    {
        MyState = UnitState.Done;
        StartTurnTile = PositionTile;
        FindObjectOfType<MapControl>().HidePaths();
        FindObjectOfType<MapControl>().DeselectAll();
        FindObjectOfType<CameraControl>().ResetTarget();
        SetMyColor();
        
        Vector3 Direction3D = new Vector3(LookDirection.x, 0, LookDirection.y);
        gameObject.transform.LookAt(gameObject.transform.position + Direction3D);
        StartTurnLookDirection = LookDirection;
        FindObjectOfType<GameManager>().CheckTurnEnd();
    }

    public virtual void SetMyColor() {
        switch (MyState)
        {
            case UnitState.Ready:
                //gameObject.GetComponent<MeshRenderer>().material.color = new Color32(0, 255, 0, 255);
                //if (MonsterId == 0 || MonsterId == 1 || MonsterId == 2) MyBody.GetComponent<SkinnedMeshRenderer>().material.color = new Color32(255, 255, 255, 255);
                SetTileOccupy(true, false);
                break;
            case UnitState.Moving:
                break;
            case UnitState.Attacking:
                break;
            case UnitState.Done:
                //gameObject.GetComponent<MeshRenderer>().material.color = new Color32(0, 150, 0, 255);
                //if (MonsterId == 0 || MonsterId == 1 || MonsterId == 2) MyBody.GetComponent<SkinnedMeshRenderer>().material.color = new Color32(120, 120, 120, 255);
                SetTileOccupy(true, true);
                break;
            case UnitState.DecidingAttack:
                break;
            case UnitState.DecidingMove:
                break;
            case UnitState.DecidingAction:
                break;
            case UnitState.Passive:
                //gameObject.GetComponent<MeshRenderer>().material.color = new Color32(0, 255, 0, 255);
                //if (MonsterId == 0 || MonsterId == 1 || MonsterId == 2) MyBody.GetComponent<SkinnedMeshRenderer>().material.color = new Color32(255, 255, 255, 255);
                break;
            default:
                break;
        }
    } //made to be rewritten

    public virtual void SetCamera() { } //rewrite me

    public virtual void SetTileOccupy(bool b, bool Dark)
    {
        PositionTile.transform.Find("TopUi").Find("PlayerPos").GetComponent<Image>().enabled = b;
    }
    
    public object[] GetStats()
    {
        object[] MyStats = new object[8];
        MyStats[0] = MyName;
        MyStats[1] = (float)MaxHealth;
        MyStats[2] = (float)CurrentHealth;
        MyStats[3] = AttackStat;
        MyStats[4] = DefenceStat;
        MyStats[5] = MovementPoints;
        MyStats[6] = AttackRange;
        MyStats[7] = Level;
        return MyStats;
    }

    public virtual void TakeDamage(int Amount, float ElementApplify)
    {
        if (!MarkedForDeath)
        {
            CurrentHealth -= Amount;
            if (CurrentHealth <= 0)
            {
                DyingStartTime = Time.time;
                MarkedForDeath = true;
                SetMyColor();
                SetTileOccupy(false, false);
                if (gameObject.GetComponent<Animator>()) { gameObject.GetComponent<Animator>().SetTrigger("Die"); }
                if (GetComponent<AudioSource>()) GetComponent<AudioSource>().PlayOneShot(DeathSound);
            }
            else
            {
                if (gameObject.GetComponent<Animator>()) { gameObject.GetComponent<Animator>().SetTrigger("Hit"); }
                if (GetComponent<AudioSource>()) GetComponent<AudioSource>().PlayOneShot(TakeDamageSound);
            }
            float Percentage = ((float)CurrentHealth / (float)MaxHealth);
            HealthScript.SetPos(PositionTile.transform.position);
            HealthScript.SetNewHealthPercent(Percentage);
            AlertBox.transform.position = gameObject.transform.position;
            AlertBox.transform.rotation = Quaternion.Euler(45, FindObjectOfType<CameraControl>().TargetRotateValue, 0);
            AlertBox.GetComponent<Animator>().SetTrigger("Rise");
            print(Amount + " damage taken");
        }
    }

    public virtual void Reset()
    {
        FindObjectOfType<MapControl>().DeselectAll();
        SetTileOccupy(false, false);
        PositionTile = StartTurnTile;
        gameObject.transform.position = PositionTile.transform.position + PosOffset;
        SetTileOccupy(true, false);
        //HealthBarCanvas.transform.position = PositionTile.transform.position + new Vector3(-2f, 5f, -2f);
        //HealthScript.SetPos(PositionTile.transform.position + new Vector3(-2f, 5f, -2f));
        FindObjectOfType<MapControl>().HidePaths();
        FindObjectOfType<MapControl>().DeselectAll();
        //FindObjectOfType<SelectControl>().PositionTile = PositionTile;
        //FindObjectOfType<SelectControl>().PlayerFinished();
        FindObjectOfType<CameraControl>().ResetTarget();
        Vector3 Direction3D = new Vector3(LookDirection.x, 0, LookDirection.y);
        gameObject.transform.LookAt(gameObject.transform.position + Direction3D);
        MyState = UnitState.Ready;
    }

    public void StartMoving(Tile t, bool IsPlayer)
    {
        DestinationTile = t;
        if (DestinationTile != PositionTile) Waypoints = FindObjectOfType<MapControl>().GetPath(PositionTile, DestinationTile, MovementPoints, IsPlayer);
        WaypointId = 0;
        WaypointTime = Time.time;
        FindObjectOfType<MapControl>().DeselectAll();
        SetTileOccupy(false, false);
        if (gameObject.GetComponent<Animator>()) { gameObject.GetComponent<Animator>().SetBool("Walking", true); }
        if (GetComponent<AudioSource>())
        {
            GetComponent<AudioSource>().clip = WalkSound;
            GetComponent<AudioSource>().loop = true;
            GetComponent<AudioSource>().Play();
        }
        //HealthBarCanvas.SetActive(false);
        //HealthScript.TurnOff();


        MyState = UnitState.Moving;
    }

    public virtual void EndMovement()
    {
        FindObjectOfType<MapControl>().DeselectAll();
        if (gameObject.GetComponent<Animator>()) { gameObject.GetComponent<Animator>().SetBool("Walking", false); }
        if (GetComponent<AudioSource>())
        {
            GetComponent<AudioSource>().Stop();
        }
        SetTileOccupy(true, false);
        //HealthBarCanvas.SetActive(true);
        //HealthBarCanvas.transform.position = PositionTile.transform.position + new Vector3(-2f, 5f, -2f);
        //HealthScript.SetPos(PositionTile.transform.position + new Vector3(-2f, 5f, -2f));
        MyState = UnitState.DecidingAction;
    }

    public virtual void Attack(Tile t)
    {
        TargetTile = t;
        FindObjectOfType<MapControl>().HidePaths();
        FindObjectOfType<MapControl>().DeselectAll();
        FindObjectOfType<MapControl>().ShowAttackParticles(ActiveAttackId, TargetTile, PositionTile);
        FindObjectOfType<PlayerStatUIControl>().HideStats(3);
        gameObject.transform.LookAt(t.transform.position + PosOffset);
        LookDirection = (t.Position - PositionTile.Position);
        if (gameObject.GetComponent<Animator>()) { gameObject.GetComponent<Animator>().SetTrigger("Attack"); }
        if (gameObject.GetComponent<AudioSource>())
        {
            AudioClip attsound = FindObjectOfType<AttackDictionary>().AttackList[ActiveAttackId - 1].AttSound;
            gameObject.GetComponent<AudioSource>().PlayOneShot(attsound);
        }
        AttackStartTime = Time.time;
        MyState = UnitState.Attacking;
    }
}
