using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GiveUnits : MonoBehaviour
{
    PersistantStats PS;
    public int EncounterNumber = 0;
    public GameObject[] ObjectsToRemove;

    private void Start()
    {
        PS = FindObjectOfType<PersistantStats>();
        if (PS.EncounterList[EncounterNumber] == true)
        {
            foreach (GameObject item in ObjectsToRemove)
            {
                Destroy(item);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (FindObjectOfType<PersistantStats>().EncounterList[EncounterNumber] == false)
        {
            PS.EncounterIdMessage = EncounterNumber;
            PS.PreviousSceneName = SceneManager.GetActiveScene().name;
            PS.Position = other.gameObject.transform.position;
            PS.Rotation = other.gameObject.transform.rotation;
            FindObjectOfType<PersistantStats>().SpawnId = 100;
            StartCoroutine(FindObjectOfType<LevelLoader>().LoadLevel("NewUnitScreen"));

            FindObjectOfType<PersistantStats>().EncounterList[EncounterNumber] = true;
        }
    }
}
