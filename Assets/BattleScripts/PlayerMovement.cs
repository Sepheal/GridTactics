using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class PlayerMovement : UnitMovement
{
    UnitMenuControl MyMenu;
    public GameObject ArrowUI;

    override public void Start()
    {
        base.Start();
        //MyMenu = transform.Find("MyMenu").GetComponent<UnitMenuControl>();
        //MyMenu.Deactivate();
        MyMenu = FindObjectOfType<UnitMenuControl>();
        MyOwner = Owner.Player;
        HealthScript.SetMainColour(new Color32(0, 0, 255, 255));
    }

    public override void Reset()
    {
        base.Reset();
        MyMenu.Deactivate();
        FindObjectOfType<SelectControl>().PositionTile = PositionTile;
        FindObjectOfType<SelectControl>().PlayerFinished();
    }

    public override void EndPawnTurn()
    {
        FindObjectOfType<SelectControl>().PlayerFinished();
        base.EndPawnTurn();
        print("Running");
    }

    public override void SetTileOccupy(bool b, bool Dark)
    {
        base.SetTileOccupy(b, Dark);
        Color32 NewColor = new Color32(0, 255, 0, 100);
        if (Dark) NewColor = new Color32(0, 50, 0, 100);
        PositionTile.transform.Find("TopUi").Find("PlayerPos").GetComponent<Image>().color = NewColor;
    }

    public override void SetCamera()
    {
        //Camera.current.transform.position = gameObject.transform.position + new Vector3(0, 45, -30);
        FindObjectOfType<CameraControl>().SetTarget(gameObject);
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

    public void ShowMove()
    {
        List<Tile> MyList = FindObjectOfType<MapControl>().GetMoveTiles(MovementPoints, PositionTile, true);
        FindObjectOfType<MapControl>().SetTiles(MyList, 1);
        MyState = UnitState.DecidingMove;
    }

    public override void EndMovement()
    {
        ShowActions();
        base.EndMovement();
    }

    public void ShowActions()
    {
        MyMenu = FindObjectOfType<UnitMenuControl>();
        MyMenu.Activate(this);
        FindObjectOfType<MapControl>().DeselectAll();
        MyState = UnitState.DecidingAction;

        ArrowUI.SetActive(false);
    }

    public void Wait()
    {
        if (MyState == UnitState.DecidingAction)
        {
            MyMenu.Deactivate();
            EndPawnTurn();
        }
    }

    public void ShowAttack(int id)
    {
        if (MyState == UnitState.DecidingAction)
        {
            List<Tile> MyList = FindObjectOfType<MapControl>().GetAttackTiles(AttackRange, PositionTile);
            FindObjectOfType<MapControl>().SetTiles(MyList, 2);
            ActiveAttackId = id;
            MyMenu.Deactivate();
            FindObjectOfType<SelectControl>().Activate();
            MyState = UnitState.DecidingAttack;


            //ARROW CODE
            //get rid of directions I can't attack
            foreach (Transform item in ArrowUI.transform)
            {
                item.GetComponent<Image>().enabled = false;
            }
            foreach (Tile t in PositionTile.Connections)
            {
                Vector2 Dir = t.Position - PositionTile.Position;
                switch (Dir.x)
                {
                    case 1: ArrowUI.transform.Find("Up").GetComponent<Image>().enabled = true; break;
                    case -1: ArrowUI.transform.Find("Down").GetComponent<Image>().enabled = true; break;
                    default:
                        switch (Dir.y)
                        {
                            case 1: ArrowUI.transform.Find("Left").GetComponent<Image>().enabled = true; break;
                            case -1: ArrowUI.transform.Find("Right").GetComponent<Image>().enabled = true; break;
                            default: break;
                        }
                        break;
                }
            }
            ArrowUI.transform.position = gameObject.transform.position + new Vector3(0, 4.0f, 0);
            ArrowUI.SetActive(true);
        }
    }

    public override void Attack(Tile t)
    {
        base.Attack(t);

        ArrowUI.SetActive(false);
    }

    public void ActivateArrowAnim(Vector2 Dir)
    {
        switch (Dir.x)
        {
            case 1:
                ArrowUI.GetComponent<Animator>().SetTrigger("UpTrigger");
                break;
            case -1:
                ArrowUI.GetComponent<Animator>().SetTrigger("DownTrigger");
                break;
            default:
                switch (Dir.y)
                {
                    case 1:
                        ArrowUI.GetComponent<Animator>().SetTrigger("LeftTrigger");
                        break;
                    case -1:
                        ArrowUI.GetComponent<Animator>().SetTrigger("RightTrigger");
                        break;
                    default:
                        ArrowUI.GetComponent<Animator>().SetTrigger("NoneTrigger");
                        break;
                }
                break;
        }
    }
}
