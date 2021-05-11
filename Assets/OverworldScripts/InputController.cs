using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public bool IsActive = false, IsInMenu = false;
    public GameObject MyCharacter, RotHolder, WalkCheck, SpawnHolder, StartingPosition;
    Tile LastKnownTile;
    float Accel = 0f;

    public OverworldMenu overworldMenu;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    public void SetupControls()
    {
        PersistantStats PS = FindObjectOfType<PersistantStats>();
        if (PS.SpawnId != 0)
        {
            if (PS.SpawnId == 100)
            {
                MyCharacter.transform.SetPositionAndRotation(PS.Position, PS.Rotation);
            }
            else
            {
                GameObject Spawn = SpawnHolder.transform.Find("Spawn" + FindObjectOfType<PersistantStats>().SpawnId).gameObject;
                MyCharacter.transform.SetPositionAndRotation(Spawn.transform.position, Spawn.transform.rotation);
            }
            PS.SpawnId = 0;
        }
        else
        {
            MyCharacter.transform.position = StartingPosition.transform.position;
        }

        if (GetTileAt(MyCharacter.transform))
        {
            LastKnownTile = GetTileAt(MyCharacter.transform);
        }
        IsActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsActive)
        {
            if (!IsInMenu)
            {
                ////////////////////////////////////////////////movement
                Vector3 TranslateValue;
                float MultiplyValue;
                if (Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0)
                {
                    Accel += 0.5f * Time.deltaTime;
                    if (Accel > 1.0f) Accel = 1.0f;

                    if (Accel < 0.7f) MultiplyValue = 20f;
                    else MultiplyValue = 30f;
                    //TranslateValue = new Vector3(Input.GetAxisRaw("Vertical") * MultiplyValue, 0, -Input.GetAxisRaw("Horizontal") * MultiplyValue);
                    TranslateValue = new Vector3(Input.GetAxisRaw("Vertical"), 0, -Input.GetAxisRaw("Horizontal"));
                    TranslateValue *= Time.deltaTime * MultiplyValue;

                    RotHolder.transform.position = MyCharacter.transform.position;
                    RotHolder.transform.Translate(TranslateValue, Space.Self);

                    WalkCheck.transform.position = RotHolder.transform.position;
                    WalkCheck.transform.rotation = RotHolder.transform.rotation;
                    WalkCheck.transform.Translate(TranslateValue * 5, Space.Self);

                    MyCharacter.transform.LookAt(RotHolder.transform, new Vector3(0, 1, 0));

                    if (GetTileAt(RotHolder.transform)) //GetTileAt(WalkCheck.transform) && 
                    {
                        float heightdiff = GetTileTop(GetTileAt(RotHolder.transform)) - GetTileTop(LastKnownTile);
                        if (heightdiff <= 2.2f && heightdiff >= -5.2f)
                        {
                            MyCharacter.transform.position = RotHolder.transform.position;
                            MyCharacter.GetComponent<Animator>().SetBool("Walking", true);
                        }

                    }
                    if (GetTileAt(MyCharacter.transform))
                    {
                        LastKnownTile = GetTileAt(MyCharacter.transform);
                        MyCharacter.transform.position = new Vector3(MyCharacter.transform.position.x, GetTileTop(LastKnownTile), MyCharacter.transform.position.z);
                    }
                }
                else
                {
                    Accel -= 5f * Time.deltaTime;
                    if (Accel <= 0)
                    {
                        Accel = 0;

                        MyCharacter.GetComponent<Animator>().SetBool("Walking", false);
                    }
                }
            }
            /////////////////
            ///Menu stuff
            if (Input.GetButtonDown("StartButton"))
            {
                switch (IsInMenu)
                {
                    case true:
                        overworldMenu.gameObject.SetActive(false);
                        IsInMenu = false;
                        break;
                    case false:
                        overworldMenu.gameObject.SetActive(true);
                        IsInMenu = true;
                        break;
                }
            }
        }
    }

    float GetTileTop(Tile tile)
    {
        if (tile.gameObject.GetComponent<BoxCollider>())
        {
            return tile.gameObject.transform.position.y + (tile.gameObject.GetComponent<BoxCollider>().bounds.size.y / 2);
        }
        else
        {
            return tile.gameObject.transform.position.y + (tile.gameObject.GetComponent<MeshCollider>().bounds.size.y / 2);
        }
    }

    Tile GetTileAt(Transform T)
    {
        LayerMask TileMask = LayerMask.GetMask("Tiles");
        if (Physics.Raycast(T.position + new Vector3(0, 3, 0), new Vector3(0, -1, 0), out RaycastHit RayCastInfo, Mathf.Infinity, TileMask))
        {
            if (RayCastInfo.collider && RayCastInfo.collider.gameObject.CompareTag("Tile"))
            {
                return RayCastInfo.collider.gameObject.GetComponent<Tile>();
            }
        }
        return null;
    }
}
