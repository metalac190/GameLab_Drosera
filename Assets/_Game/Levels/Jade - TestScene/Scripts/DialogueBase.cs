using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue")]
public class DialogueBase : ScriptableObject
{
    [System.Serializable]
    public class Info
    {
        public CharacterProfile character;
        [TextArea(5, 10)]
        public string myText;
    }

    [Header("Insert Dialogue Information Below")]
    public Info[] dialogueInfo;
}