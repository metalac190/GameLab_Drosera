using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogDisplay : MonoBehaviour
{
    [SerializeField] public Conversation conversation;

    [SerializeField] public GameObject speakerLeft;
    [SerializeField] public GameObject speakerRight;

    [SerializeField] public GameObject continueButton;

    [SerializeField] public GameObject deactivateButton;

    [SerializeField] public Canvas canvas;

    private SpeakerUI speakerUILeft;
    private SpeakerUI speakerUIRight;

    private int activateLineIndex = 0;

    public UnityEvent DialogScroll;

    void Start()
    {
        speakerUILeft = speakerLeft.GetComponent<SpeakerUI>();
        speakerUIRight = speakerRight.GetComponent<SpeakerUI>();
        
        speakerUILeft.Speaker = conversation.speakerLeft;
        speakerUIRight.Speaker = conversation.speakerRight;

        AdvanceConversation();

        continueButton.SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AdvanceConversation();
        }
    }

    public void NextSentences()
    {
        AdvanceConversation();
    }

    public void AdvanceConversation()
    {
        if (activateLineIndex < conversation.lines.Length)
        {
            DisplayLine();
            activateLineIndex += 1;
        }
        else
        {
            speakerUILeft.Hide();
            speakerUIRight.Hide();

            activateLineIndex = 0;
            // deactivate dialogue
            deactivateButton.SetActive(true);

            continueButton.SetActive(false);
        }
    }

    public void DisplayLine()
    {
        Line line = conversation.lines[activateLineIndex];

        Character character = line.character;

        if (speakerUILeft.SpeakerIs(character))
        {
            SetDialog(speakerUILeft, speakerUIRight, line.text);
        }
        else
        {
            SetDialog(speakerUIRight, speakerUILeft, line.text);
        }

        DialogScroll.Invoke();
    }

    public void SetDialog(SpeakerUI activeSpeakerUI, SpeakerUI inactiveSpeakerUI, string text)
    {
        activeSpeakerUI.Show();
        inactiveSpeakerUI.Hide();

        StopAllCoroutines();
        StartCoroutine(EffectTypewriter(text, activeSpeakerUI));
    }

    IEnumerator EffectTypewriter(string text, SpeakerUI activeSpeakerUI)
    {
        activeSpeakerUI.Dialog = "";
        foreach (char character in text.ToCharArray())
        {
            activeSpeakerUI.Dialog += character;
            yield return null;
        }
    }

    public void DeactivateCutscene()
    {
        canvas.GetComponent<Canvas>().enabled = false;
        GameManager.Instance.CutSceneComplete();
    }
}
