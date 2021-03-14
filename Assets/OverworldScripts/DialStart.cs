using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialStart : MonoBehaviour
{
    PersistantStats PS;
    public List<string> Dialogue;
    public int EncounterNumber = 0;
    public bool Repeatable = false;
    bool BeenUsed = false;
    public bool OnCollide = true;

    // Start is called before the first frame update
    void Start()
    {
        PS = FindObjectOfType<PersistantStats>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (OnCollide)
        {
            if (!BeenUsed || (BeenUsed && Repeatable))
            {
                if (PS.EncounterList[EncounterNumber] == false)
                {
                    FindObjectOfType<DialogueOverworld>().InitiateDialogue(Dialogue);
                    BeenUsed = true;
                }
            }
        }
    }
}
