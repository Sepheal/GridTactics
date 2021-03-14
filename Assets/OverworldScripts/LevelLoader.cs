using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{

    public IEnumerator LoadLevel(string SceneName)
    {
        StopActions();
        GetComponent<Animator>().SetTrigger("Start");

        yield return new WaitForSeconds(1);

        SceneManager.LoadScene(SceneName);
    }

    public IEnumerator LoadBattle(string SceneName)
    {
        StopActions();
        GetComponent<Animator>().SetTrigger("Start");
        FindObjectOfType<BattleEffect>().StartAnim();

        yield return new WaitForSeconds(1);

        SceneManager.LoadScene(SceneName);
    }

    void StopActions()
    {
        if (FindObjectOfType<InputController>())
        {
            FindObjectOfType<InputController>().IsActive = false;
        }
        foreach (Animator A in FindObjectsOfType<Animator>())
        {
            if (A.gameObject.name != "SceneTransition") A.enabled = false;
        }
        foreach (OverWorldAI OA in FindObjectsOfType<OverWorldAI>())
        {
            OA.Active = false;
        }
    }
}
