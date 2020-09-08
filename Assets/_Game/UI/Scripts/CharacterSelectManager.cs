using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

// 9/8 - worked on by Vinson Kok
public class CharacterSelectManager : MonoBehaviour
{
    [SerializeField] CharacterSelectInfo[] characterList;

    [Header("Text Stuff")]
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI subtitleText;
    [SerializeField] TextMeshProUGUI descriptionText;

    [Header("Visual Stuff")]
    [SerializeField] Color[] characterColors;

    int previouslySelectedCharacter = 1;
    [SerializeField] Image[] characterBorderImages;
    [SerializeField] Image characterImage;
    [SerializeField] Image characterBackgroundImage;
    [SerializeField] Image[] weaponImages;

    private void Awake()
    {
        SetCharacterBorderColors();
        DisplaySelectedCharacter(0);
    }

    void SetCharacterBorderColors()
    {
        for (int i = 0; i < characterColors.Length; i++)
        {
            characterBorderImages[i].color = characterColors[i];
        }
    }

    void AnimateCharacterBorder(int animateIn, int animateOut)
    {
        if (animateIn != animateOut)
        {
            characterBorderImages[animateOut].gameObject.transform.DOMoveX(150, 1);
            characterBorderImages[animateIn].gameObject.transform.DOMoveX(0, 1);
        }
    }

    // setup selected character info visually when player presses that character's button
    public void DisplaySelectedCharacter(int index)
    {
        AnimateCharacterBorder(previouslySelectedCharacter, index);
        previouslySelectedCharacter = index;

        // text stuff
        nameText.text = characterList[index].Name;
        nameText.color = characterColors[index];

        titleText.text = characterList[index].CharacterTitle;
        titleText.color = characterColors[index];

        subtitleText.text = characterList[index].CharacterSubtitle;
        subtitleText.color = characterColors[index]; 

        descriptionText.text = characterList[index].CharacterDescription;
        descriptionText.color = characterColors[index];

        // visual stuff
        characterImage.sprite = characterList[index].CharacterSprite;

        characterBackgroundImage.color = characterColors[index];

        for (int i = 0; i < weaponImages.Length; i++)
        {
            weaponImages[i].sprite = characterList[index].WeaponSprites[i];
        }
    }

    // TODO- display weapon info when player clicks on weapon button
    public void DisplaySelectedWeaponInfo(int index)
    {

    }
}
