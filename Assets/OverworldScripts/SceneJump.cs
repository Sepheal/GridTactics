using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneJump : MonoBehaviour
{
    public string SceneName;
    public int SpawnId;

    private void OnTriggerEnter(Collider other)
    {
        FindObjectOfType<PersistantStats>().SpawnId = SpawnId;
        StartCoroutine(FindObjectOfType<LevelLoader>().LoadLevel(SceneName));
    }
}
