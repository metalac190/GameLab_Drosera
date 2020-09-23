using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogDisplay : MonoBehaviour
{
    [SerializeField] public Conversation conversation;
    [SerializeField] public GameObject speakerLeft;
    [SerializeField] public GameObject speakerRight;

    private SpeakerUI speakerUILeft;
    private SpeakerUI speakerUIRight;

    private int activateLineIndex = 0;

    private void Start()
    {
        speakerUILeft = speakerLeft.GetComponent<SpeakerUI>();
        speakerUIRight = speakerRight.GetComponent<SpeakerUI>();

        speakerUILeft.Speaker = conversation.speakerLeft;
        speakerUIRight.Speaker = conversation.speakerRight;
    }
    
    void Update()
    {
        if(Input.GetKeyDown("space"))
        {
            AdvanceConversation();
        } 
        else if(Input.GetKeyDown("x"))
        {
            EndConversation();
        }
            
    }

    void AdvanceConversation()
    {
        if(activateLineIndex < conversation.lines.Length)
        {
            DisplayLine();
            activateLineIndex += 1;
        }
        else
        {
            speakerUILeft.Hide();
            speakerUIRight.Hide();
            activateLineIndex = 0;
        }
    }

    void EndConversation()
    {
        speakerUILeft.Hide();
        speakerUIRight.Hide();
    }

    void DisplayLine()
    {
        Line line = conversation.lines[activateLineIndex];
        Character character = line.character;

        if(speakerUILeft.SpeakerIs(character))
        {
            SetDialog(speakerUILeft, speakerUIRight, line.text);
        }
        else
        {
            SetDialog(speakerUIRight, speakerUILeft, line.text);
        }
    }

    void SetDialog(SpeakerUI activeSpeakerUI, SpeakerUI inactiveSpeakerUI, string text)
    {
        //activeSpeakerUI.Dialog = text;
        activeSpeakerUI.Show();
        inactiveSpeakerUI.Hide();

        activeSpeakerUI.Dialog = "";
        StartCoroutine(EffectTypewriter(text, activeSpeakerUI));
    }

    private IEnumerator EffectTypewriter(string text, SpeakerUI uI)
    {
        foreach(char character in text.ToCharArray())
        {
            uI.Dialog += character;
            yield return new WaitForSeconds(0.02f);
            // yield reutrn null;
        }
    }
}
