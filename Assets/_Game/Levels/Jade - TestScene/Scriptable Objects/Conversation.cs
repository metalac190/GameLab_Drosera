using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct Line
{
    [SerializeField] public Character character;

    [TextArea(5, 10)]
    [SerializeField] public string text;
}

[CreateAssetMenu(fileName = "New Conversation", menuName = "Conversation")]
public class Conversation : ScriptableObject
{
    [SerializeField] public Character speakerLeft;
    [SerializeField] public Character speakerRight;
    [SerializeField] public Line[] lines;
}
