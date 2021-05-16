using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Tile script, used enmass, all controlled by the Map control

public class Tile : MonoBehaviour
{
    public Vector2 Position;
    public int Cost, ID;
    public GameObject TilePiece;
    public List<Tile> Connections;
    public bool Selectable = false;
    public GameObject PathImage, HighlightImage;
    public Sprite[] PathPrefabs;
    public int StartingTileId = 0;

    public Tile(Vector2 pos, int cost, GameObject t)
    {
        Position = pos;
        Cost = cost;
        TilePiece = t;
        TilePiece.transform.position = new Vector3(pos.x * 5, 0, pos.y * 5);
    }

    public void SelfSetup()
    {
        Position = new Vector2(gameObject.transform.localPosition.x / 5, gameObject.transform.localPosition.z / 5);
        TilePiece = gameObject;
    }

    public void Deselect()
    {
        TurnOffHighlight();
        Selectable = false;
    }

    public void ShowPath(int pathid, Tile OtherSpot) //For Start and FInal
    {
        PathImage.transform.rotation = new Quaternion();
        //PathImage.GetComponent<Image>().enabled = true;
        PathImage.SetActive(true);
        Vector2 VectorD;
        VectorD = OtherSpot.Position - Position;
        if (VectorD.x == 1) PathImage.transform.Rotate(new Vector3(0, 0, -90));
        else if (VectorD.x == -1) PathImage.transform.Rotate(new Vector3(0, 0, 90));
        else if (VectorD.y == -1) PathImage.transform.Rotate(new Vector3(0, 0, 180));
        if (pathid == 2) PathImage.transform.Rotate(new Vector3(0, 0, 180));

        switch (pathid)
        {
            case 1: PathImage.GetComponent<Image>().sprite = PathPrefabs[0]; break;
            case 2: PathImage.GetComponent<Image>().sprite = PathPrefabs[1]; break;
            default: break;
        }
    }
    public void ShowPath(Tile PrevSpot, Tile NextSpot)
    {
        PathImage.transform.rotation = new Quaternion();
        //PathImage.GetComponent<Image>().enabled = true;
        PathImage.SetActive(true);
        Vector2 Vector1, Vector2;
        Vector1 = Position - PrevSpot.Position; Vector2 = NextSpot.Position - Position;
        if (Vector1 == Vector2)
        {
            if (Vector1.x != 0) PathImage.transform.Rotate(new Vector3(0, 0, 90));
            PathImage.GetComponent<Image>().sprite = PathPrefabs[2];
        }
        else
        {
            if (Vector1.y == 1 && Vector2.x == -1)
            {
                PathImage.GetComponent<Image>().sprite = PathPrefabs[3]; // up+left
            }
            else if (Vector1.y == 1 && Vector2.x == 1)
            {
                PathImage.GetComponent<Image>().sprite = PathPrefabs[4]; // up+right
            }
            else if (Vector1.x == 1 && Vector2.y == 1)
            {
                PathImage.GetComponent<Image>().sprite = PathPrefabs[3]; //Right+left
                PathImage.transform.Rotate(new Vector3(0, 0, -90));
            }
            else if (Vector1.x == 1 && Vector2.y == -1)
            {
                PathImage.GetComponent<Image>().sprite = PathPrefabs[4]; //Right+right
                PathImage.transform.Rotate(new Vector3(0, 0, -90));
            }
            else if (Vector1.y == -1 && Vector2.x == 1)
            {
                PathImage.GetComponent<Image>().sprite = PathPrefabs[3]; // Down+left
                PathImage.transform.Rotate(new Vector3(0, 0, -180));
            }
            else if (Vector1.y == -1 && Vector2.x == -1)
            {
                PathImage.GetComponent<Image>().sprite = PathPrefabs[4]; // Down+right
                PathImage.transform.Rotate(new Vector3(0, 0, -180));
            }
            else if (Vector1.x == -1 && Vector2.y == -1)
            {
                PathImage.GetComponent<Image>().sprite = PathPrefabs[3]; //Left+left
                PathImage.transform.Rotate(new Vector3(0, 0, 90));
            }
            else if (Vector1.x == -1 && Vector2.y == 1)
            {
                PathImage.GetComponent<Image>().sprite = PathPrefabs[4]; //Left+right
                PathImage.transform.Rotate(new Vector3(0, 0, 90));
            }
        }
    }

    public void HidePath()
    {
        PathImage.SetActive(false);
    }

    public void SetHighlightColor(Color32 NewColor)
    {
        HighlightImage.GetComponent<Image>().color = NewColor;
        HighlightImage.SetActive(true);
    }

    public void TurnOffHighlight()
    {
        HighlightImage.SetActive(false);
    }
}
