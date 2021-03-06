using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Inherited version of the unitmovement for enemies, also contains the AI logic for in battle

class AttackInstruction
{
    public Tile AttackOriginTile, MovedTooTile;
    public List<Tile> AffectedTiles;
    public int AttackIdUsed = -1;
    public int NumPlayersAffected, NumEnemiesAffected;
    public int DamageDeltPlayer, DamageDeltEnemy;
    public float Score;
}

public class EnemyMovement : UnitMovement
{
    AttackInstruction ActiveInstruction;

    override public void Start()
    {
        base.Start();
        MyOwner = Owner.Enemy;
        HealthScript.SetMainColour(new Color32(255, 0, 0, 255));
    }

    public override void SetCamera()
    {
        FindObjectOfType<CameraControl>().SetTarget(gameObject);
    }

    public override void SetTileOccupy(bool b, bool Dark)
    {
        base.SetTileOccupy(b, Dark);
        Color32 NewColor = new Color32(255, 0, 0, 100);
        if (Dark) NewColor = new Color32(50, 0, 0, 100);
        PositionTile.transform.Find("TopUi").Find("PlayerPos").GetComponent<Image>().color = NewColor;
    }

    public override void TakeDamage(int Amount, float ElementApplify)
    {
        //super effective or not
        string AlertString = "-" + Amount;
        if (ElementApplify > 1.1f)
        {
            AlertText.GetComponent<Text>().fontSize = 300;
            AlertString += "!";
        }
        else if (ElementApplify < 0.9f)
        {
            AlertText.GetComponent<Text>().fontSize = 150;
        }
        else AlertText.GetComponent<Text>().fontSize = 200;
        AlertText.GetComponent<Text>().text = AlertString;

        base.TakeDamage(Amount, ElementApplify);
    }

    public void DecideMovement()
    {
        List<Tile> MyMList = mapControl.GetMoveTiles(MovementPoints, PositionTile, false);
        List<AttackInstruction> ListOfAttackInstructions = new List<AttackInstruction>();
        
        int count = 0;
        float DistanceToClosestPlayer = 10000.0f;
        Tile ClosestTile = PositionTile;
        foreach (Tile MTile in MyMList)
        {
            List<Tile> MyAList = mapControl.GetAttackTiles(AttackRange, MTile);
            foreach (Tile ATile in MyAList)
            {
                foreach (int AttackID in AttackIDs)
                {
                    int playersaffected = 0, enemiesaffected = 0, damagedeltplayer = 0, damagedeltenemy = 0;
                    List<Tile> MyAPList = mapControl.GetAttackPatternTiles(AttackID, ATile, MTile);
                    foreach (Tile APTile in MyAPList)
                    {
                        foreach (PlayerMovement player in FindObjectsOfType<PlayerMovement>())
                        {
                            if (player.PositionTile == APTile && !player.MarkedForDeath)
                            {
                                playersaffected++;
                                damagedeltplayer += mapControl.CalculateDamage(AttackID, AttackStat, player.DefenceStat, MyElement, player.MyElement);
                                break;
                            }
                            count++;
                        }
                        foreach (EnemyMovement enemy in FindObjectsOfType<EnemyMovement>())
                        {
                            if (enemy.PositionTile == APTile && enemy != this)
                            {
                                enemiesaffected++;
                                damagedeltenemy += mapControl.CalculateDamage(AttackID, AttackStat, enemy.DefenceStat, MyElement, enemy.MyElement);
                                break;
                            }
                            count++;
                        }
                    }
                    if (playersaffected > 0)
                    {
                        AttackInstruction AI = new AttackInstruction
                        {
                            AttackOriginTile = ATile,
                            MovedTooTile = MTile,
                            AffectedTiles = MyAPList,
                            NumPlayersAffected = playersaffected,
                            NumEnemiesAffected = enemiesaffected,
                            DamageDeltPlayer = damagedeltplayer,
                            DamageDeltEnemy = damagedeltenemy,
                            AttackIdUsed = AttackID
                        };
                        ListOfAttackInstructions.Add(AI);
                    }
                }
            }

            
            ///check if closest spot to players
            foreach (PlayerMovement player in FindObjectsOfType<PlayerMovement>())
            {
                if (Vector3.Distance(player.PositionTile.Position, MTile.Position) < DistanceToClosestPlayer)
                {
                    DistanceToClosestPlayer = Vector3.Distance(player.PositionTile.Position, MTile.Position);
                    ClosestTile = MTile;
                }
            }
        }
        print(count + " number of loops....");

        //Check if I died
        if (MarkedForDeath)
        {
            MyState = UnitState.Done;
            return;
        }

        //Passive
        if (ListOfAttackInstructions.Count > 0)
        {
            //Choose instruction
            float Score = 0;
            AttackInstruction ChosenAI = ListOfAttackInstructions[0];
            foreach (AttackInstruction AI in ListOfAttackInstructions)
            {
                //AI.Score = (float)AI.NumPlayersAffected - ((float)AI.NumEnemiesAffected / 2);
                AI.Score = AI.DamageDeltPlayer - AI.DamageDeltEnemy;
                if (AI.Score >= Score)
                {
                    Score = AI.Score;
                    ChosenAI = AI;
                }
            }
            //Start Instruction
            StartMoving(ChosenAI.MovedTooTile, false);
            FindObjectOfType<PlayerStatUIControl>().SetValues(this, 2);
            ActiveInstruction = ChosenAI;
        }
        else if (PassiveLevel > 1)
        {
            AttackInstruction AI = new AttackInstruction
            {
                MovedTooTile = ClosestTile
            };
            StartMoving(AI.MovedTooTile, false);
            FindObjectOfType<PlayerStatUIControl>().SetValues(this, 2);
            ActiveInstruction = AI;

        }
        else
        {
            EndPawnTurn();
        }
    }

    public override void EndMovement()
    {
        base.EndMovement();
        if (ActiveInstruction.AttackIdUsed > -1)
        {
            ActiveAttackId = ActiveInstruction.AttackIdUsed;
            Attack(ActiveInstruction.AttackOriginTile);
        }
        else
        {
            EndPawnTurn();
        }
        
    }
}
