using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CinematicStart : MonoBehaviour
{
    PersistantStats PS;
    public List<string> Dialogue;
    public int EncounterNumber = 0, RequiredEncounterNum = 0;
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
                if (PS.EncounterList[EncounterNumber] == false && (PS.EncounterList[RequiredEncounterNum] == true || RequiredEncounterNum == 0))
                {
                    StartCoroutine(DoCinematic());
                    BeenUsed = true;
                    PS.EncounterList[EncounterNumber] = true;
                }
            }
        }
    }

    IEnumerator DoCinematic()
    {
        FindObjectOfType<InputController>().IsActive = false;
        GetComponent<PlayableDirector>().Play();
        float time = (float)GetComponent<PlayableDirector>().playableAsset.duration;
        yield return new WaitForSeconds(time);
        FindObjectOfType<InputController>().IsActive = true;
        FindObjectOfType<DialogueOverworld>().InitiateDialogue(Dialogue);
    }
}
