using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
    public DialogueBase dialogue;
    public GameObject dialogueBox;
    public Text dialogueName;
    public Text dialogueText;
    public Image dialoguePortrait;
    public UnityEvent DialogScroll;
    private bool isCurrentlyTyping;
    private string completeText;
    public GameObject continueButton;
    public GameObject deactivateButton;

    //FIFO Collection
    private Queue<DialogueBase.Info> dialogueInfo;

    void Start()
    {
        dialogueInfo = new Queue<DialogueBase.Info>();
        EnqueueDialogue(dialogue);
        continueButton.SetActive(true);
    }

    public void EnqueueDialogue(DialogueBase db)
    {
        dialogueBox.SetActive(true);
        dialogueInfo.Clear();

        foreach (DialogueBase.Info info in db.dialogueInfo)
        {
            dialogueInfo.Enqueue(info);
        }
        DequeueDialogue();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            DequeueDialogue();
        }
    }

    public void NextSentence()
    {
        DequeueDialogue();
    }

    public void DequeueDialogue()
    {
        if(isCurrentlyTyping == true)
        {
            CompleteText();
            StopAllCoroutines();
            isCurrentlyTyping = false;
            return;
        }

        if(dialogueInfo.Count == 0)
        {
            DeactiveCutscene();
            return;
        }

        //add in code that detects if dialogue is no more 
        DialogueBase.Info info = dialogueInfo.Dequeue();
        completeText = info.myText;
        dialogueName.text = info.character.myname;
        dialogueText.text = info.myText;
        dialoguePortrait.sprite = info.character.myPortrait;

        dialogueText.text = "";
        StartCoroutine(TypeText(info));
        DialogScroll.Invoke();
    }

    IEnumerator TypeText(DialogueBase.Info info)
    {
        isCurrentlyTyping = true;
        foreach (char letter in info.myText.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }
    }

    public void CompleteText()
    {
        dialogueText.text = completeText;
    }

    public void DeactiveCutscene()
    {
        dialogueBox.SetActive(false);
        GameManager.Instance.CutSceneComplete();
    }
}
