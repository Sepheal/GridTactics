using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueOverworld : MonoBehaviour
{
    bool Active = false;
    float LetterStartTime;
    public float LetterTimeAmount = 0.03f;
    int CurrentDialNum = 0, CurrentLetterNum = 0;
    List<string> DialogueList;

    public GameObject UIPrefab;
    public GameObject DialogueContainer;
    public Text DialogueTextBox;
    public Animator PlayerAnimator;

    public AudioClip ConfirmAudio, ConfirmAudio2, ChangeAudio;

    // Start is called before the first frame update
    void Start()
    {
        DialogueContainer = Instantiate(UIPrefab).transform.Find("Dialogue").gameObject;
        DialogueTextBox = DialogueContainer.transform.Find("Text").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Active)
        {
            //Scrolling through letters
            if (CurrentDialNum < DialogueList.Count)
            {
                if (Time.time - LetterStartTime >= LetterTimeAmount)
                {
                    if (CurrentLetterNum < DialogueList[CurrentDialNum].Length)
                    {
                        DialogueTextBox.text += DialogueList[CurrentDialNum].Substring(CurrentLetterNum, 1);
                        CurrentLetterNum++;
                        LetterStartTime = Time.time;
                    }
                    
                }
            }
            //dialogue over code
            else
            {
                Active = false;
                GetComponent<InputController>().IsActive = true;
                PlayerAnimator.enabled = true;
                DialogueContainer.SetActive(false);
            }

            //Input to progress dialogue
            if (Input.GetButtonDown("Select") || Input.GetMouseButtonDown(0))
            {
                GetComponent<AudioSource>().PlayOneShot(ChangeAudio);
                ResetText();
                CurrentDialNum++;
            }
        }
    }

    public void InitiateDialogue(List<string> Dialogue)
    {
        if (!Active)
        {
            Active = true;
            GetComponent<InputController>().IsActive = false;
            PlayerAnimator.enabled = false;
            DialogueContainer.SetActive(true);
            DialogueList = Dialogue;
            CurrentDialNum = 0;
            ResetText();
        }
        
    }

    void ResetText()
    {
        DialogueTextBox.text = "";
        CurrentLetterNum = 0;
        LetterStartTime = Time.time;
    }
}
