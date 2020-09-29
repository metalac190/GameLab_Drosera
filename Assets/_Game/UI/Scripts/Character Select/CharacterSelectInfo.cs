using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectInfo : MonoBehaviour
{
    /*
    [SerializeField] Sprite characterSprite;
    public Sprite CharacterSprite
    {
        get => characterSprite;
        set => characterSprite = value;
    }
    */

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

    [SerializeField] string[] weaponHeader;
    public string[] WeaponHeader
    {
        get => weaponHeader;
        set => weaponHeader = value;
    }

    [TextArea(1, 100)]
    [SerializeField] string[] weaponDescription;
    public string[] WeaponDescription
    {
        get => weaponDescription;
        set => weaponDescription = value;
    }

    [SerializeField] string statHeader;
    public string StatHeader
    {
        get => statHeader;
        set => statHeader = value;
    }

    [TextArea(1, 100)]
    [SerializeField] string statDescription;
    public string StatDescription
    {
        get => statDescription;
        set => statDescription = value;
    }
}
