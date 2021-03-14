using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleEngage : MonoBehaviour
{
    public string BattleName = "TestBattle";
    PersistantStats PS;
    public int MyEncounterNum = 0;
    public GameObject[] ObjectsToRemove;
    bool Active = false;

    private void Start()
    {
        PS = FindObjectOfType<PersistantStats>();
        if (PS.EncounterList[MyEncounterNum] == true)
        {
            foreach (GameObject item in ObjectsToRemove)
            {
                Destroy(item);
            }
        }
        else
        {
            Active = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Active)
        {
            PS.PreviousSceneName = SceneManager.GetActiveScene().name;
            PS.Position = other.gameObject.transform.position;
            PS.Rotation = other.gameObject.transform.rotation;
            FindObjectOfType<PersistantStats>().SpawnId = 100;
            StartCoroutine(FindObjectOfType<LevelLoader>().LoadBattle(BattleName));
        }
    }
}
