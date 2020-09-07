using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 9/6 - worked on by Vinson Kok
public class CharacterSelectInfo : MonoBehaviour
{
    [SerializeField] Sprite characterSprite;
    public Sprite CharacterSprite
    {
        get => characterSprite;
        set => characterSprite = value;
    }

    [SerializeField] string characterName;
    public string Name
    {
        get => characterName;
        set => characterName = value;
    }

    [SerializeField] string characterTitle;
    public string CharacterTitle
    {
        get => characterTitle;
        set => characterTitle = value;
    }

    [SerializeField] Sprite[] weaponSprites;
    public Sprite[] WeaponSprites
    {
        get => weaponSprites;
        set => weaponSprites = value;
    }

    [SerializeField] string characterSubtitle;
    public string CharacterSubtitle
    {
        get => characterSubtitle;
        set => characterSubtitle = value;
    }

    [TextArea(1, 100)]
    [SerializeField] string characterDescription;
    public string CharacterDescription
    {
        get => characterDescription;
        set => characterDescription = value;
    }
}
