using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistantStats : MonoBehaviour
{
    public static PersistantStats instance;

    //SCENE COMMUNICATION
    public int SpawnId = 0;
    public Vector3 Position;
    public Quaternion Rotation;
    public string PreviousSceneName = "";
    public int EncounterIdMessage = 0;

    //Global encounter list
    public bool[] EncounterList;

    //PLAYER STATS
    public string PName = "";
    //public int MP = 2, AR = 1, MaxHp = 100, Attack = 5, Defence = 3;
    public int PlayerLevel = 1;
    public int PlayerExp = 0;
    public Element PElement = Element.Neutral;
    public int[] PAttacks = { 1 };

    //PLAYER APPEARANCE
    public int HeroId = 0;
    public int[] HeroCloths;
    public Sprite HeroImage;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    

    
}
