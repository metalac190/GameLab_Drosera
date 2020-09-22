using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Character")]
public class Character : ScriptableObject
{
    [SerializeField] public string fullName;
    //[SerializeField] public Sprite portrait;
}
