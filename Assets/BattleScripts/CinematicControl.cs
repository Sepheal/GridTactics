using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

//cinematic controller in battle

public class CinematicControl : MonoBehaviour
{

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

        yield return new WaitForSeconds(5);

        FindObjectOfType<GameManager>().DecideStart();
    }
}
