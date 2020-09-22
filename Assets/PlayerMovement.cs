using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class PlayerMovement : UnitMovement
{
    UnitMenuControl MyMenu;

    override public void Start()
    {
        base.Start();
        //MyMenu = transform.Find("MyMenu").GetComponent<UnitMenuControl>();
        //MyMenu.Deactivate();
        MyMenu = FindObjectOfType<UnitMenuControl>();
        MyOwner = Owner.Player;
        //HealthBarCanvas.transform.Find("Front").GetComponent<Image>().color = new Color32(0, 0, 255, 255);
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
        MyMenu.Activate(this);
        FindObjectOfType<MapControl>().DeselectAll();
        MyState = UnitState.DecidingAction;
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
            //ActiveAttackId = AttackIDs[id - 1];
            ActiveAttackId = id;
            MyMenu.Deactivate();
            FindObjectOfType<SelectControl>().Activate();
            MyState = UnitState.DecidingAttack;
        }
    }
}
