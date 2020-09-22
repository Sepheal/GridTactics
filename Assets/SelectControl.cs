using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class SelectControl : MonoBehaviour
{
    public Tile PositionTile;
    public PlayerMovement ActivePlayer;
    float CooldownStart; readonly float FrameCooldown = 0.08f;
    float FrameDelayStart; readonly float FrameDelayCooldown = 0.01f;
    public bool Active = true, Refresh = true;

    public GameObject SelectorD;
    float SelectAccel = 0f;

    // Start is called before the first frame update
    void Start()
    {
        CooldownStart = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Active)
        {
            bool Moved = false;
            bool ActionButton = false;
            bool CancelButton = false;
            if (Refresh) Moved = true;
            ///////////////////////////////////////////////////////////////////////////////////
            //Moving Selector
            Vector3 TranslateValue = new Vector3();
            float MultiplyValue;
            if (Input.GetAxisRaw("Mouse Y") != 0 || Input.GetAxisRaw("Mouse X") != 0)
            {
                SelectAccel += 0.05f;
                if (SelectAccel > 1.0f) SelectAccel = 1.0f;
                if (SelectAccel < 0.8f) MultiplyValue = 0.6f;
                else MultiplyValue = 1.2f;
                TranslateValue = new Vector3(Input.GetAxisRaw("Mouse Y") * MultiplyValue, 0, -Input.GetAxisRaw("Mouse X") * MultiplyValue);
                SelectorD.transform.Translate(TranslateValue, Space.Self);
            }
            else if (Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0)
            {
                SelectAccel += 0.05f;
                if (SelectAccel > 1.0f) SelectAccel = 1.0f;
                if (SelectAccel < 0.8f) MultiplyValue = 0.5f;
                else MultiplyValue = 0.8f;
                TranslateValue = new Vector3(Input.GetAxisRaw("Vertical") * MultiplyValue, 0, -Input.GetAxisRaw("Horizontal") * MultiplyValue);
                SelectorD.transform.Translate(TranslateValue, Space.Self);
            }
            else
            {
                SelectAccel -= 0.5f;
                if (SelectAccel < 0) SelectAccel = 0;

                //Magnetic
                Vector3 Dir = new Vector3(PositionTile.transform.position.x, 8, PositionTile.transform.position.z) - new Vector3(SelectorD.transform.position.x, 8, SelectorD.transform.position.z);
                if (Dir.magnitude > 1f) TranslateValue = new Vector3(Dir.x / 100, Dir.y / 100, Dir.z / 100);
                else if (Dir.magnitude > 0.15f) TranslateValue = new Vector3(Dir.x / 10, Dir.y / 10, Dir.z / 10);
                SelectorD.transform.Translate(TranslateValue, Space.World);
            }
            
            if (SearchForUnit())
            {
                SelectorD.transform.position = new Vector3(SelectorD.transform.position.x, PositionTile.transform.position.y + 13, SelectorD.transform.position.z);
                SelectorD.transform.Find("SelectDiamond").GetComponent<MeshRenderer>().material.color = new Color32(255, 0, 0, 255);
            }
            else
            {
                SelectorD.transform.position = new Vector3(SelectorD.transform.position.x, PositionTile.transform.position.y + 8, SelectorD.transform.position.z);
                SelectorD.transform.Find("SelectDiamond").GetComponent<MeshRenderer>().material.color = new Color32(255, 0, 0, 100);
            }

            //FindObjectOfType<CameraControl>().SelectorMoved(SelectorD.transform.position);
            ///////////////////////////////////////////////////////////////////////////////////
            //Check Moved and Action for mouse
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            int layer = 1 << 8;
            //Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layer);
            if (Physics.Raycast(SelectorD.transform.position, new Vector3(0, -1, 0), out RaycastHit RayCastInfo , Mathf.Infinity, layer)) {
                if (RayCastInfo.collider && RayCastInfo.collider.gameObject.CompareTag("Tile") && PositionTile != RayCastInfo.collider.gameObject.GetComponent<Tile>())
                {
                    Moved = true;
                    PositionTile = RayCastInfo.collider.gameObject.GetComponent<Tile>();
                    
                }
                if (Input.GetMouseButtonDown(0) && RayCastInfo.collider && RayCastInfo.collider.gameObject.CompareTag("Tile"))
                {
                    ActionButton = true;
                }
                
                layer = 1 << 9;
                Physics.Raycast(SelectorD.transform.position, new Vector3(0, -1, 0), out RayCastInfo, Mathf.Infinity, layer);
                if (Input.GetMouseButtonDown(0) && RayCastInfo.collider && RayCastInfo.collider.gameObject.CompareTag("Player"))
                {
                    ActionButton = true;
                }
            }
            ///////////////////////////////////////////////////////////////////////////////////
            //Select button controller
            if (Input.GetButtonDown("Select") && Time.time - FrameDelayStart >= FrameDelayCooldown)
            {
                ActionButton = true;
            }

            //Cancel button controller
            if (Input.GetButtonDown("Cancel") && Time.time - FrameDelayStart >= FrameDelayCooldown)
            {
                CancelButton = true;
            }
            ///////////////////////////////////////////////////////////////////////////////////
            //Selector Moved
            if (Moved)
            {
                ///////////////////////////////////////////////////////////////////////////////////
                //Move Selector and Camera
                gameObject.transform.position = PositionTile.transform.position + new Vector3(0, 3, 0);
                //FindObjectOfType<CameraControl>().SelectorMoved(gameObject.transform.position);

                ///////////////////////////////////////////////////////////////////////////////////
                //PathUpdate
                if (ActivePlayer && PositionTile.Selectable && ActivePlayer.MyState == UnitState.DecidingMove)
                {
                    FindObjectOfType<MapControl>().HidePaths();
                    if (PositionTile != ActivePlayer.PositionTile)
                    {
                        FindObjectOfType<MapControl>().GetPath(ActivePlayer.PositionTile, PositionTile, ActivePlayer.MovementPoints, true);
                    }
                }

                ///////////////////////////////////////////////////////////////////////////////////
                //ShowPreview and unit stats
                if (!ActivePlayer)
                {
                    FindObjectOfType<MapControl>().RefreshHighlight(1);
                    FindObjectOfType<PlayerStatUIControl>().HideStats(1);
                    FindObjectOfType<PlayerStatUIControl>().HideStats(2);
                    //search for unit at place
                    UnitMovement U = SearchForUnit();
                    if (U != null)
                    {
                        if (U.MyState != UnitState.Done) SetHighlight(U);
                        FindObjectOfType<PlayerStatUIControl>().SetValues(U, 0);
                    }
                }

                ///////////////////////////////////////////////////////////////////////////////////
                //Have activeplayer and I want to check enemy targets stats
                else
                {
                    if (ActivePlayer.MyState == UnitState.DecidingAttack)
                    {
                        if (PositionTile.Selectable) FindObjectOfType<MapControl>().ShowDamagePreview(ActivePlayer.GetComponent<UnitMovement>(), PositionTile);
                        else FindObjectOfType<PlayerStatUIControl>().HideStats(3);
                    }
                    else if (ActivePlayer.MyState == UnitState.DecidingMove)
                    {
                        //search for unit
                        UnitMovement U = SearchForUnit();
                        if (U != null && U != ActivePlayer) FindObjectOfType<PlayerStatUIControl>().SetValues(U, 1);
                        else FindObjectOfType<PlayerStatUIControl>().HideStats(2);
                    }
                }

                ///////////////////////////////////////////////////////////////////////////////////
                //Attack Pattern
                if (ActivePlayer && ActivePlayer.MyState == UnitState.DecidingAttack && PositionTile != ActivePlayer.PositionTile)
                {
                    FindObjectOfType<MapControl>().RefreshHighlight(2);
                    if (PositionTile.Selectable)
                    {
                        List<Tile> MyList = FindObjectOfType<MapControl>().GetAttackPatternTiles(ActivePlayer.ActiveAttackId, PositionTile, ActivePlayer.PositionTile);
                        FindObjectOfType<MapControl>().SetTiles(MyList, 3);
                    }
                }

                ///////////////////////////////////////////////////////////////////////////////////
                //Occlude background objects
                foreach (GameObject item in GameObject.FindGameObjectsWithTag("BackgroundOcclude"))
                {
                    Color32 c = item.GetComponent<MeshRenderer>().material.color;
                    Vector3 BasePosOfObject = new Vector3(item.transform.position.x, gameObject.transform.position.y, item.transform.position.z);
                    if ((gameObject.transform.position - BasePosOfObject).magnitude <= 8)
                    {
                        c.a = 50;
                        item.GetComponent<MeshRenderer>().material.color = c;
                    }
                    else
                    {
                        c.a = 255;
                        item.GetComponent<MeshRenderer>().material.color = c;
                    }
                }
            }
            ///////////////////////////////////////////////////////////////////////////////////
            ///ACTION BUTTON CLICKED
            if (ActionButton)
            {
                if (ActivePlayer)
                {
                    if (PositionTile.Selectable)
                    {
                        if (ActivePlayer.MyState == UnitState.DecidingMove)
                        {
                            ActivePlayer.StartMoving(PositionTile, true);
                            Active = false;
                        }
                        else if (ActivePlayer.MyState == UnitState.DecidingAttack)
                        {
                            ActivePlayer.Attack(PositionTile);
                            Active = false;
                        }
                    }
                }
                else
                {
                    //search for player then Show Move
                    PlayerMovement PlayerAtPosition = SearchForPlayer();
                    if (PlayerAtPosition && PlayerAtPosition.MyState == UnitState.Ready && !FindObjectOfType<GameManager>().CheckForDead()) ActivePlayer = PlayerAtPosition;
                    if (ActivePlayer)
                    {
                        FindObjectOfType<MapControl>().RefreshHighlight(1);
                        ActivePlayer.ShowMove();
                    }
                }
            }
            ///////////////////////////////////////////////////////////////////////////////////
            ///CANCEL BUTTON CLICKED
            if (CancelButton)
            {
                if (ActivePlayer)
                {
                    if (ActivePlayer.MyState == UnitState.DecidingMove)
                    {
                        ActivePlayer.Reset();
                    }
                    else if (ActivePlayer.MyState == UnitState.DecidingAttack)
                    {
                        PositionTile = ActivePlayer.PositionTile;
                        FindObjectOfType<PlayerStatUIControl>().HideStats(3);
                        ActivePlayer.ShowActions();
                        Active = false;
                    }
                }
            }

            Refresh = false;
        }
    }

    public void ResetDelay()
    {
        FrameDelayStart = Time.time;
    }

    public void PlayerFinished()
    {
        ActivePlayer = null;
        Active = true;
        Refresh = true;
        ResetDelay();
    }

    public void Activate()
    {
        Active = true;
        ResetDelay();
        Refresh = true;
    }

    void SetHighlight(UnitMovement U)
    {
        //Generate Ranges and show them
        List<Tile> MyMList;
        if (U is PlayerMovement) MyMList = FindObjectOfType<MapControl>().GetMoveTiles(U.MovementPoints, U.PositionTile, true);
        else MyMList = FindObjectOfType<MapControl>().GetMoveTiles(U.MovementPoints, U.PositionTile, false);
        List<Tile> NewAList = new List<Tile>();
        List<Tile> NewAPList = new List<Tile>();
        foreach (Tile MTile in MyMList)
        {
            List<Tile> MyAList = FindObjectOfType<MapControl>().GetAttackTiles(U.AttackRange, MTile);
            foreach (Tile ATile in MyAList)
            {
                if (!MyMList.Exists(x => x.Position == ATile.Position))
                {
                    NewAList.Add(ATile);
                    
                }
                foreach (int AttackID in U.AttackIDs)
                {
                    List<Tile> MyAPList = FindObjectOfType<MapControl>().GetAttackPatternTiles(AttackID, ATile, MTile);
                    foreach (Tile APTile in MyAPList)
                    {
                        if (!MyMList.Exists(x => x.Position == APTile.Position) && !NewAList.Exists(x => x.Position == APTile.Position))
                        {
                            NewAPList.Add(APTile);
                        }
                    }
                }
            }
        }
        /*
        foreach (Tile ATile in NewAList)
        {
            foreach (int AttackID in U.AttackIDs)
            {
                List<Tile> MyAPList = FindObjectOfType<MapControl>().GetAttackPatternTiles(AttackID, ATile, U.PositionTile);
                foreach (Tile APTile in MyAPList)
                {
                    if (!MyMList.Exists(x => x.Position == APTile.Position) && !NewAList.Exists(x => x.Position == APTile.Position))
                    {
                        NewAPList.Add(APTile);
                    }
                }
            }
        }
        */
        FindObjectOfType<MapControl>().SetTiles(MyMList, 4);
        FindObjectOfType<MapControl>().SetTiles(NewAList, 6);
        FindObjectOfType<MapControl>().SetTiles(NewAPList, 6);
    }

    PlayerMovement SearchForPlayer()
    {
        //search for player
        PlayerMovement P = null;
        foreach (PlayerMovement p in FindObjectsOfType<PlayerMovement>())
        {
            if (p.PositionTile == PositionTile)
            {
                P = p;
                break;
            }
        }
        return P;
    }

    EnemyMovement SearchForEnemy()
    {
        //search for enemy
        EnemyMovement E = null;
        foreach (EnemyMovement e in FindObjectsOfType<EnemyMovement>())
        {
            if (e.PositionTile == PositionTile)
            {
                E = e;
                break;
            }
        }
        return E;
    }

    UnitMovement SearchForUnit()
    {
        UnitMovement U = null;
        foreach (UnitMovement unit in FindObjectsOfType<UnitMovement>())
        {
            if (unit.PositionTile == PositionTile)
            {
                U = unit;
                break;
            }
        }
        return U;
    }
}
