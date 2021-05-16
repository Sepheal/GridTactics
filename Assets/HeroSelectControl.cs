using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HeroSelectControl : MonoBehaviour
{
    public int CustomStage = 0;
    public int HairChoice = 0, FaceChoice = 0, HeadChoice = 0, TorsoChoice = 0, ShoeChoice = 0, GloveChoice = 0, ShoulderChoice = 0, BeltChoice = 0;
    int NumOfStages = 9;
    public GameObject Cam, DisplayHero, HeroCam;
    public RenderTexture HeroTexture;
    public Text DisplayCatagoryText, NameOutputText;

    public Material[] ColourMats;
    public GameObject[] StageUIs;

    public GameObject[] Hairs;
    public GameObject[] Faces;
    public GameObject[] Headgears;
    public GameObject[] Torsos;
    public GameObject[] Shoes;
    public GameObject[] Gloves;
    public GameObject[] Shoulders;
    public GameObject[] Belts;
    public int[] HatToHairIds;

    PersistantStats PS;

    public AudioClip ConfirmAudio, ConfirmAudio2, ChangeAudio;
    AudioSource Audio;

    public GameObject[] LimeBackdrops;

    // Start is called before the first frame update
    void Start()
    {
        UpdateChoice(0);
        PS = FindObjectOfType<PersistantStats>();
        Audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        /*
        //Character rotation
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            DisplayHero.transform.Rotate(new Vector3(0, -1, 0), Space.World);
        }
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            DisplayHero.transform.Rotate(new Vector3(0, 1, 0), Space.World);
        }*/
    }

    void UpdateChoice(int NewStage)
    {
        if (StageUIs[CustomStage] != null) StageUIs[CustomStage].SetActive(false);

        CustomStage = NewStage;
        switch (CustomStage)
        {
            case 0: DisplayCatagoryText.text = "Hair"; break;
            case 1: DisplayCatagoryText.text = "Skin"; break;
            case 2: DisplayCatagoryText.text = "Headgear"; break;
            case 3: DisplayCatagoryText.text = "Torso"; break;
            case 4: DisplayCatagoryText.text = "Shoes"; break;
            case 5: DisplayCatagoryText.text = "Gloves"; break;
            case 6: DisplayCatagoryText.text = "Shoulders"; break;
            case 7: DisplayCatagoryText.text = "Belt"; break;
            default: DisplayCatagoryText.text = "Confirmation"; break;
        }
        if (StageUIs[CustomStage] != null) StageUIs[CustomStage].SetActive(true);
    }

    public void RightClicked()
    {
        Audio.PlayOneShot(ChangeAudio);

        if (CustomStage < (NumOfStages - 2)) UpdateChoice(CustomStage + 1);
        else UpdateChoice(NumOfStages - 1);
    }

    public void LeftClicked()
    {
        Audio.PlayOneShot(ChangeAudio);

        if (CustomStage > 1) UpdateChoice(CustomStage-1);
        else UpdateChoice(0);
    }

    public void ToggleTorso(int Id)
    {
        PlaySound();

        Torsos[TorsoChoice].SetActive(false);
        TorsoChoice = Id;
        Torsos[TorsoChoice].SetActive(true);
    }

    public void ToggleShoe(int Id)
    {
        PlaySound();

        Shoes[ShoeChoice].SetActive(false);
        ShoeChoice = Id;
        Shoes[ShoeChoice].SetActive(true);
    }

    public void ToggleHair(int Id)
    {
        PlaySound();

        HideHairs();
        HairChoice = Id;
        SetHair(HairChoice);
    }

    void HideHairs()
    {
        foreach (GameObject hair in Hairs)
        {
            hair.SetActive(false);
        }
    }

    void SetHair(int Id)
    {
        switch (HatToHairIds[HeadChoice])
        {
            case 0:
                //Show full hair
                Hairs[HairChoice].SetActive(true);
                break;
            case 1:
                //Show Half hair
                Hairs[HairChoice + 5].SetActive(true);
                break;
            case 2:
                //show no hair
                break;
            default:
                break;
        }
    }

    public void ToggleFace(int Id)
    {
        PlaySound();

        Faces[FaceChoice].SetActive(false);
        FaceChoice = Id;
        Faces[FaceChoice].SetActive(true);
    }

    public void ToggleGloves(int Id)
    {
        PlaySound();

        Gloves[GloveChoice].SetActive(false);
        GloveChoice = Id;
        Gloves[GloveChoice].SetActive(true);
    }

    public void ToggleShoulders(int Id)
    {
        PlaySound();

        if (Shoulders[ShoulderChoice]) Shoulders[ShoulderChoice].SetActive(false);
        ShoulderChoice = Id;
        if (Shoulders[ShoulderChoice]) Shoulders[ShoulderChoice].SetActive(true);
    }

    public void ToggleBelt(int Id)
    {
        PlaySound();

        if (Belts[BeltChoice]) Belts[BeltChoice].SetActive(false);
        BeltChoice = Id;
        if (Belts[BeltChoice]) Belts[BeltChoice].SetActive(true);
    }

    public void ToggleHeadgear(int Id)
    {
        PlaySound();

        if (Headgears[HeadChoice]) Headgears[HeadChoice].SetActive(false);
        HeadChoice = Id;
        if (Headgears[HeadChoice]) Headgears[HeadChoice].SetActive(true);

        //ToggleHair(HairChoice);
        HideHairs();
        SetHair(HairChoice);
    }

    public void randomisechoices()
    {
        PlaySound();
        //Audio.enabled = false;

        ToggleHair(Random.Range(0, 5));
        ToggleFace(Random.Range(0, Faces.Length));
        ToggleHeadgear(Random.Range(0, Headgears.Length));
        ToggleTorso(Random.Range(0, Torsos.Length));
        ToggleShoe(Random.Range(0, Shoes.Length));
        ToggleGloves(Random.Range(0, Gloves.Length));
        ToggleShoulders(Random.Range(0, Shoulders.Length));
        ToggleBelt(Random.Range(0, Belts.Length));

        //Audio.enabled = true;
    }

    public void ConfirmHero()
    {
        Audio.PlayOneShot(ConfirmAudio2);

        StartCoroutine(MoveOn());

        StartCoroutine(FindObjectOfType<LevelLoader>().LoadLevel("DreamWorld"));
    }

    void PlaySound()
    {
        if (!Audio.isPlaying) Audio.PlayOneShot(ConfirmAudio);
    }

    public IEnumerator MoveOn()
    {
        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
        yield return waitForEndOfFrame;

        int X = HeroTexture.width, Y = HeroTexture.height;
        Texture2D Screenshot = new Texture2D(X, Y, TextureFormat.RGBA32, false);
        RenderTexture.active = HeroTexture;
        HeroCam.GetComponent<Camera>().Render();
        Screenshot.ReadPixels(new Rect(0, 0, X, Y), 0, 0);

        for (int i = 0; i < Screenshot.width; i++)
        {
            for (int j = 0; j < Screenshot.height; j++)
            {
                if (Screenshot.GetPixel(i, j).g >= 0.6f && Screenshot.GetPixel(i, j).r >= 0.2f)
                {
                    Screenshot.SetPixel(i, j, Color.clear);
                }
                else
                {
                    Screenshot.SetPixel(i, j, Screenshot.GetPixel(i, j));
                }
            }
        }
        Screenshot.Apply();

        Sprite sprite = Sprite.Create(Screenshot, new Rect(0, 0, X, Y), new Vector2(0.5f, 0.5f), 4.0f);
        PS.HeroImage = sprite;

        PS.HeroCloths = new int[] { HairChoice, FaceChoice, HeadChoice, TorsoChoice, ShoeChoice, GloveChoice, ShoulderChoice, BeltChoice };
        if (NameOutputText.text != "") PS.PName = NameOutputText.text;
        else PS.PName = "Hero";
    }
}
