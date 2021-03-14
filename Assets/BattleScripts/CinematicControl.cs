using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CinematicControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activate()
    {
        StartCoroutine(DoAnim());
    }

    IEnumerator DoAnim()
    {
        foreach (EnemyMovement enemy in FindObjectsOfType<EnemyMovement>())
        {
            enemy.gameObject.GetComponent<Animator>().SetTrigger("Taunt");
        }
        GetComponent<PlayableDirector>().Play();

        yield return new WaitForSeconds(2);

        /*

        bool HasUnits = false;
        foreach (UnitListing unit in FindObjectsOfType<UnitListing>())
        {
            if (unit.Controller == Owner.Player)
            {
                HasUnits = true;
                break;
            }
        }
        if (HasUnits) StartCoroutine(FindObjectOfType<GameManager>().PlayTitle());
        else
        {
            FindObjectOfType<SelectControl>().Activate();
            FindObjectOfType<CameraControl>().IsPlayerTurn = true;
            FindObjectOfType<GameManager>().ChangeTurn();
        }*/

        FindObjectOfType<GameManager>().DecideStart();
    }
}
