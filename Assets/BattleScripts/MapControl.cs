using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;

class Checkpoint
{
    public float TotalCost = 0, DistanceLeft = 10000000;
    public Tile tile;
    public Checkpoint OriginCheckpoint;
}

public enum Directions {Straight, Back, Left, Right};

public class MapControl : MonoBehaviour
{
    public List<Tile> Map;
    public GameObject BaseTilePrefab;
    public Sprite SelectIconPrefab;
    AttackDictionary AD;

    static UnityEngine.Vector2 Up = new UnityEngine.Vector2(0, 1), Down = new UnityEngine.Vector2(0, -1), Left = new UnityEngine.Vector2(-1, 0), Right = new UnityEngine.Vector2(1, 0);
    static UnityEngine.Vector3 Null = new UnityEngine.Vector3();
    // Start is called before the first frame update
    void Start()
    {
        AD = FindObjectOfType<AttackDictionary>();
        Map = new List<Tile>();

        int count = 1;
        foreach (Transform tilepiece in transform)
        {
            Tile newtile = tilepiece.GetComponent<Tile>();
            newtile.SelfSetup();
            newtile.ID = count;
            Map.Add(newtile);
            count++;
        }

        foreach (Tile t in Map)
        {
            UnityEngine.Vector2 pos = t.Position;
            if (CheckTile(pos + Up))
            {
                t.Connections.Add(FindTile(pos + Up));
            }
            if (CheckTile(pos + Down))
            {
                t.Connections.Add(FindTile(pos + Down));
            }
            if (CheckTile(pos + Left))
            {
                t.Connections.Add(FindTile(pos + Left));
            }
            if (CheckTile(pos + Right))
            {
                t.Connections.Add(FindTile(pos + Right));
            }
        }
    }

    public void SetupMode()
    {
        foreach (Tile tile in Map)
        {
            if (tile.StartingTileId == 1)
            {
                tile.SetHighlightColor(new Color32(255, 255, 255, 170));
            }
        }
    }

    public bool CheckTile(UnityEngine.Vector2 CheckPos)
    {
        return Map.Exists(x => x.Position == CheckPos);
    }

    public Tile FindTile(UnityEngine.Vector2 FindPos)
    {
        return Map.Find(x => x.Position == FindPos);
    }

    public void DeselectAll()
    {
        foreach (Tile t in Map)
        {
            t.Deselect();
        }
    }

    void ShowPath(List<Tile> Tiles)
    {
        for (int i = 0; i < Tiles.Count; i++)
        {
            if (i == 0)
            {
                Tiles[0].ShowPath(1, Tiles[1]);
            }
            else if (i == Tiles.Count - 1)
            {
                Tiles[i].ShowPath(2, Tiles[i - 1]);
            }
            else
            {
                Tiles[i].ShowPath(Tiles[i - 1], Tiles[i + 1]);
            }
        }
    }

    public void HidePaths()
    {
        foreach (Tile t in Map)
        {
            t.HidePath();
        }
    }

    public List<Tile> GetPath(Tile Start, Tile End, int points, bool IsPlayer)
    {
        List<Tile> Waypoints = new List<Tile>();

        List<Checkpoint> Checkpoints = new List<Checkpoint>(), FinishedCheckpoints = new List<Checkpoint>();
        Checkpoint s = new Checkpoint { tile = Start };
        Checkpoints.Add(s);

        bool GoalReached = false;
        //int checkpointnum = 0;
        do
        {
            foreach (Tile t in Checkpoints[0].tile.Connections)
            {
                bool HasOpponent = false;
                if (IsPlayer) foreach (EnemyMovement p in FindObjectsOfType<EnemyMovement>()) { if (p.PositionTile == t) { HasOpponent = true; break; } }
                else foreach (PlayerMovement p in FindObjectsOfType<PlayerMovement>()) { if (p.PositionTile == t) { HasOpponent = true; break; } }

                if (!FinishedCheckpoints.Exists(x => x.tile.ID == t.ID) && (Checkpoints[0].TotalCost + t.Cost) <= points && !HasOpponent)
                {
                    if (Checkpoints.Exists(x => x.tile.ID == t.ID))
                    {
                        Checkpoint c = Checkpoints.Find(x => x.tile.ID == t.ID);
                        float NewCost = Checkpoints[0].TotalCost + t.Cost;
                        if (c.TotalCost > NewCost)
                        {
                            c.OriginCheckpoint = Checkpoints[0];
                            c.TotalCost = c.OriginCheckpoint.TotalCost + t.Cost;
                            c.DistanceLeft = (End.gameObject.transform.localPosition - t.gameObject.transform.localPosition).magnitude;
                        }
                    }
                    else
                    {
                        Checkpoint c = new Checkpoint { 
                            tile = t,
                            OriginCheckpoint = Checkpoints[0]
                        };
                        c.TotalCost = c.OriginCheckpoint.TotalCost + t.Cost;
                        c.DistanceLeft = (End.gameObject.transform.localPosition - t.gameObject.transform.localPosition).magnitude;
                        Checkpoints.Add(c);
                    }
                }
            }
            //sort
            FinishedCheckpoints.Add(Checkpoints[0]);
            Checkpoints.RemoveAt(0);
            Checkpoints.Sort(delegate (Checkpoint c1, Checkpoint c2) { return c1.DistanceLeft.CompareTo(c2.DistanceLeft); });
            //print("///////////////////");
            foreach (Checkpoint c in Checkpoints)
            {
                //print("ID: " + c.tile.ID + "Cost: " + c.TotalCost);
            }

            if (Checkpoints.Count <= 0 || Checkpoints[0].tile == End) GoalReached = true;
        } while (!GoalReached);

        bool BeginningReached = false;
        Checkpoint Check = Checkpoints.Find(x => x.tile == End);
        do
        {
            Waypoints.Add(Check.tile);
            if (Check.OriginCheckpoint != null)
            {
                Check = Check.OriginCheckpoint;
            }
            else
            {
                BeginningReached = true;
            }
        } while (!BeginningReached);

        Waypoints.Reverse();
        ShowPath(Waypoints);
        return Waypoints;
    }

    public void RefreshHighlight(int id)
    {
        foreach (Tile t in Map)
        {
            if (t.Selectable)
            {
                switch (id)
                {
                    case 1: t.SetHighlightColor(new Color32(0, 255, 0, 170)); break;
                    case 2: t.SetHighlightColor(new Color32(255, 0, 0, 170)); break;
                    default: break;
                }
            }
            else t.TurnOffHighlight();
        }
    }

    public void DoDamage(int AttackId, Tile Target ,List<UnitMovement> Exceptions, float AttackStat, Element AttackElement)
    {
        List<Tile> HitTiles = GetAttackPatternTiles(AttackId, Target, Exceptions[0].PositionTile);
        foreach (UnitMovement unit in FindObjectsOfType<UnitMovement>())
        {
            if (HitTiles.Exists(x => x == unit.PositionTile) && !Exceptions.Exists(x => x == unit))
            {
                unit.TakeDamage(CalculateDamage(AttackId, AttackStat, unit.DefenceStat, AttackElement, unit.MyElement), GetElementInteraction(AttackElement, unit.MyElement));
            }
        }
    }

    public void ShowAttackParticles(int AttackId, Tile Target, Tile Source)
    {
        List<Tile> HitTiles = GetAttackPatternTiles(AttackId, Target, Source);
        foreach (Tile tile in HitTiles)
        {
            tile.transform.Find("Particle System").GetComponent<ParticleSystem>().Play();
        }
    }

    public void StopAttackParticles(int AttackId, Tile Target, Tile Source)
    {
        List<Tile> HitTiles = GetAttackPatternTiles(AttackId, Target, Source);
        foreach (Tile tile in HitTiles)
        {
            tile.transform.Find("Particle System").GetComponent<ParticleSystem>().Stop();
        }
    }

    public void ShowDamagePreview(UnitMovement AttackingUnit, Tile origin)
    {
        List<Tile> AffectedTiles = GetAttackPatternTiles(AttackingUnit.ActiveAttackId, origin, AttackingUnit.PositionTile);
        List<UnitMovement> Units = new List<UnitMovement>();
        List<int> Damage = new List<int>();
        foreach (Tile t in AffectedTiles)
        {
            foreach (UnitMovement unit in FindObjectsOfType<UnitMovement>())
            {
                if (unit.PositionTile == t && AttackingUnit != unit)
                {
                    Units.Add(unit);
                    Damage.Add(CalculateDamage(AttackingUnit.ActiveAttackId, AttackingUnit.AttackStat, unit.DefenceStat, AttackingUnit.MyElement, unit.MyElement));
                    break;
                }
            }
        }
        if (Units.Count > 0) FindObjectOfType<PlayerStatUIControl>().ShowDamagePreview(Units, Damage);
        else FindObjectOfType<PlayerStatUIControl>().HideStats(3);
    }

    int CalculateDamage(int Id ,float Attack, float Defence, Element AttackElement, Element TargetElement)
    {
        float damage = (Attack - Defence)/10;
        if (damage <= 0f) damage = 0f;
        damage++;
        //Minimun damage is 1 * damage below
        float ElementApplify = GetElementInteraction(AttackElement, TargetElement);
        //switch (Id)
        //{
        //    case 1: damage *= AD.AttackList[0].BaseDamage; break;
        //    case 2: damage *= AD.AttackList[1].BaseDamage; break;
        //    case 3: damage *= AD.AttackList[2].BaseDamage; break;
        //    case 4: damage *= AD.AttackList[3].BaseDamage; break;
        //    default:
        //        break;
        //}
        damage *= AD.AttackList[Id - 1].BaseDamage;
        damage *= ElementApplify;
        return (int)damage;
    }

    float GetElementInteraction(Element Attack, Element Defence)
    {
        float ElementApplify = 1.0f;
        switch (Attack)
        {
            case Element.Neutral:
                if (Defence == Element.Rock) ElementApplify = 0.5f; //Score = 1
                break;
            case Element.Rock:
                if (Defence == Element.Scissors || Defence == Element.Pencil) ElementApplify = 1.5f; //Score = 4.5
                else if (Defence == Element.Paper) ElementApplify = 0.5f;
                break;
            case Element.Paper:
                if (Defence == Element.Rock) ElementApplify = 1.5f; //Score = 1.5
                else if (Defence == Element.Scissors) ElementApplify = 0.5f;
                break;
            case Element.Scissors:
                if (Defence == Element.Paper || Defence == Element.Pencil) ElementApplify = 1.5f; //Score = 2.5
                else if (Defence == Element.Rock) ElementApplify = 0.5f;
                break;
            case Element.Pencil:
                if (Defence == Element.Neutral || Defence == Element.Paper) ElementApplify = 1.5f; //Score = 1.5
                else if (Defence == Element.Rock) ElementApplify = 0.5f;
                break;
        }
        return ElementApplify;
    }

    public void SetTiles(List<Tile> List, int id)
    {
        switch (id)
        {
            case 1:
                foreach (Tile item in List)
                {
                    item.SetHighlightColor(new Color32(0, 255, 0, 170));
                    item.Selectable = true;
                }
                break;
            case 2:
                foreach (Tile item in List)
                {
                    item.SetHighlightColor(new Color32(255, 0, 0, 170));
                    item.Selectable = true;
                }
                break;
            case 3:
                foreach (Tile item in List)
                { item.SetHighlightColor(new Color32(255, 165, 0, 170)); }
                break;
            case 4:
                foreach (Tile item in List)
                { item.SetHighlightColor(new Color32(0, 153, 76, 170)); }
                break;
            case 5:
                foreach (Tile item in List)
                { item.SetHighlightColor(new Color32(255, 178, 102, 170)); }
                break;
            case 6:
                foreach (Tile item in List)
                { item.SetHighlightColor(new Color32(255, 90, 90, 170)); }
                break;
            default:
                break;
        }
    }

    public List<Tile> GetMoveTiles(int points, Tile StartTile, bool IsPlayer)
    {
        List<Tile> MovementTiles = new List<Tile> 
        {
            StartTile
        };
        if (IsPlayer) MovementTiles = CheckTileConnections(StartTile, points - 1, StartTile, null, MovementTiles, 1, Null);
        else MovementTiles = CheckTileConnections(StartTile, points - 1, StartTile, null, MovementTiles, 4, Null);
        //print("Checks done: " + ChecksDone);
        return MovementTiles;
    }

    public List<Tile> GetAttackTiles(int points, Tile StartTile)
    {
        List<Tile> AttackTiles = new List<Tile>();
        AttackTiles = CheckTileConnections(StartTile, points - 1, StartTile, null, AttackTiles, 2, Null);
        //print("Checks done: " + ChecksDone);
        return AttackTiles;
    }

    public List<Tile> GetAttackPatternTiles(int Id, Tile StartTile, Tile Origin)
    {
        UnityEngine.Vector2 InitialDirection = (StartTile.Position - Origin.Position);
        List<Tile> PatternTiles = new List<Tile>();
        List<List<Directions>> ListOfPathDirections = new List<List<Directions>>();
        List<Directions> TempList = new List<Directions>();
        if (Id == 1 || Id == 5) //single target in front
        {
            PatternTiles.Add(StartTile);
        }
        else if (Id == 2) // Wide attack
        {
            PatternTiles.Add(StartTile);
            TempList = new List<Directions> { Directions.Left };
            ListOfPathDirections.Add(TempList);
            TempList = new List<Directions> { Directions.Right };
            ListOfPathDirections.Add(TempList);
        }
        else if (Id == 3) // diagonal attack
        {
            PatternTiles.Add(StartTile);
            TempList = new List<Directions> { Directions.Left, Directions.Right };
            ListOfPathDirections.Add(TempList);
            TempList = new List<Directions> { Directions.Straight, Directions.Left };
            ListOfPathDirections.Add(TempList);
            TempList = new List<Directions> { Directions.Straight, Directions.Right };
            ListOfPathDirections.Add(TempList);
            TempList = new List<Directions> { Directions.Right, Directions.Left };
            ListOfPathDirections.Add(TempList);
        }
        else if (Id == 4) // forward attack 2
        {
            PatternTiles.Add(StartTile);
            TempList = new List<Directions> { Directions.Straight };
            ListOfPathDirections.Add(TempList);
        }
        foreach (List<Directions> Path in ListOfPathDirections)
        {
            Tile CurrentTile = StartTile;
            UnityEngine.Vector2 LastVector = InitialDirection;
            int DirCount = 0;
            foreach (Directions Dir in Path)
            {
                DirCount++;
                LastVector = ConvertVector(LastVector, Dir);
                Tile NextTile = SearchForConnectionInDirection(CurrentTile, LastVector);
                if (NextTile)
                {
                    bool CanAdd = true;
                    if (Id == 3 && DirCount % 2 == 1) CanAdd = false; //If Odd, don't add
                    if (PatternTiles.Exists(x => x == NextTile)) CanAdd = false; // no duplicates
                    if (CanAdd) PatternTiles.Add(NextTile);
                    CurrentTile = NextTile;
                }
                else break;
            }
        }
        //print("Checks done: " + ChecksDone);
        return PatternTiles;
    }

    UnityEngine.Vector2 ConvertVector(UnityEngine.Vector2 LastVect, Directions NewDir)
    {
        switch (NewDir)
        {
            case Directions.Straight:
                if (LastVect == Right) return Right;
                else if (LastVect == Left) return Left;
                else if (LastVect == Up) return Up;
                else if (LastVect == Down) return Down;
                break;
            case Directions.Back:
                if (LastVect == Right) return Left;
                else if (LastVect == Left) return Right;
                else if (LastVect == Up) return Down;
                else if (LastVect == Down) return Up;
                break;
            case Directions.Left:
                if (LastVect == Right) return Up;
                else if (LastVect == Left) return Down;
                else if (LastVect == Up) return Left;
                else if (LastVect == Down) return Right;
                break;
            case Directions.Right:
                if (LastVect == Right) return Down;
                else if (LastVect == Left) return Up;
                else if (LastVect == Up) return Right;
                else if (LastVect == Down) return Left;
                break;
            default:
                break;
        }
        return LastVect;
    }

    Tile SearchForConnectionInDirection(Tile OriginTile, UnityEngine.Vector2 Dir)
    {
        foreach (Tile t in OriginTile.Connections)
        {
            if (t.Position - OriginTile.Position == Dir) return t;
        }
        return null;
    }

    bool CheckTileConnection(Tile NextTile, Tile PrevTile, int Id)
    {
        bool IsUsedByOpponent = false;
        if (Id == 1)
        {
            foreach (EnemyMovement p in FindObjectsOfType<EnemyMovement>())
            {
                if (p.PositionTile == NextTile)
                {
                    IsUsedByOpponent = true; break;
                }
            }
        }
        else if (Id == 4)
        {
            foreach (PlayerMovement p in FindObjectsOfType<PlayerMovement>())
            {
                if (p.PositionTile == NextTile)
                {
                    IsUsedByOpponent = true; break;
                }
            }
        }


        if (PrevTile == NextTile || (IsUsedByOpponent && (Id == 1 || Id == 4))) return false;
        else return true;
    }

    List<Tile> CheckTileConnections(Tile CurrentTile, int points, Tile StartTile, Tile PrevTile, List<Tile> MyList, int Id, UnityEngine.Vector2 Dir)
    {
        foreach (Tile NextTile in CurrentTile.Connections)
        {
            //CHECK MOVING
            if (Id == 1)
            {
                if (CheckTileConnection(NextTile, PrevTile, Id))
                {
                    bool HasPlayer = false;
                    foreach (PlayerMovement p in FindObjectsOfType<PlayerMovement>())
                    {
                        if (p.PositionTile == NextTile && StartTile != NextTile)
                        {
                            HasPlayer = true; break;
                        }
                    }
                    if (!MyList.Exists(x => NextTile.Position == x.Position) && !HasPlayer) MyList.Add(NextTile);
                    if (points > 0) CheckTileConnections(NextTile, points - 1, StartTile, CurrentTile, MyList, Id, Null);
                }
            }
            //enemy moving
            else if (Id == 4)
            {
                if (CheckTileConnection(NextTile, PrevTile, Id))
                {
                    bool HasEnemy = false;
                    foreach (EnemyMovement p in FindObjectsOfType<EnemyMovement>())
                    {
                        if (p.PositionTile == NextTile && StartTile != NextTile)
                        {
                            HasEnemy = true; break;
                        }
                    }
                    if (!MyList.Exists(x => NextTile.Position == x.Position) && !HasEnemy) MyList.Add(NextTile);
                    if (points > 0) CheckTileConnections(NextTile, points - 1, StartTile, CurrentTile, MyList, Id, Null);
                }
            }
            //CHECK ATTACKING
            else if (Id == 2)
            {
                if (CheckTileConnection(NextTile, PrevTile, Id))
                {
                    if (!MyList.Exists(x => NextTile.Position == x.Position)) MyList.Add(NextTile);
                    if (points > 0) CheckTileConnections(NextTile, points - 1, StartTile, CurrentTile, MyList, Id, Null);
                }
            }
        }
        return MyList;
    }
}
