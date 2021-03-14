using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueInBattle : MonoBehaviour
{
    bool Active = false;
    float LetterStartTime;
    public float LetterTimeAmount = 0.03f;
    int CurrentDialNum = 0, CurrentLetterNum = 0;
    List<string> DialogueList;

    public GameObject DialogueContainer;
    public Text DialogueTextBox;
    public Animator PlayerAnimator;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Active)
        {
            FindObjectOfType<SelectControl>().Active = false;

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
                DialogueContainer.SetActive(false);
                FindObjectOfType<GameManager>().StartGame();
            }

            //Input to progress dialogue
            if (Input.GetButtonDown("Select") || Input.GetMouseButtonDown(0))
            {
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
