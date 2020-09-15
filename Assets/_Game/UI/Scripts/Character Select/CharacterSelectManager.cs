using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

// 9/8 - worked on by Vinson Kok
// 9/13 - worked on by Vinson Kok
public class CharacterSelectManager : MonoBehaviour
{
    int currentlySelectedCharacter = 0;
    int previouslySelectedCharacter = 0;

    [SerializeField] CharacterSelectInfo[] characterList;

    [Header("Text Stuff")]
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI weaponHeaderText;
    [SerializeField] TextMeshProUGUI weaponDescriptionText;
    [SerializeField] TextMeshProUGUI statHeaderText;
    [SerializeField] TextMeshProUGUI statDescriptionText;

    [Header("Visual Stuff")]
    [SerializeField] Color[] characterColors;

    [SerializeField] CharacterButtonAnim[] characterButtons;    // for animating character buttons on hover
    [SerializeField] Image characterImage;
    [SerializeField] Image characterBackgroundImage;
    [SerializeField] Image[] weaponImages;

    [SerializeField] Image[] scrollBarElements;

    private void Awake()
    {
        SetCharacterBorderColors();
        DisplaySelectedCharacter();
    }

    void SetCharacterBorderColors()
    {
        for (int i = 0; i < characterColors.Length; i++)
        {
            characterButtons[i].GetComponent<Image>().color = characterColors[i];
        }
    }

    public void OnHoverCharacterButtonAnimation(int index)
    {
        if (index != currentlySelectedCharacter)
        {
            currentlySelectedCharacter = index;

            characterButtons[previouslySelectedCharacter].AnimateCharacterBorderIn();
            characterButtons[currentlySelectedCharacter].AnimateCharacterBorderOut();

            previouslySelectedCharacter = currentlySelectedCharacter;
        }
    }

    public void DisplaySelectedCharacter()
    {
        // text stuff
        nameText.text = characterList[currentlySelectedCharacter].Name;
        nameText.color = characterColors[currentlySelectedCharacter];

        titleText.text = characterList[currentlySelectedCharacter].CharacterTitle;
        titleText.color = characterColors[currentlySelectedCharacter];

        DisplaySelectedWeaponInfo(0);

        statHeaderText.text = characterList[currentlySelectedCharacter].StatHeader;
        statHeaderText.color = characterColors[currentlySelectedCharacter];
        statDescriptionText.text = characterList[currentlySelectedCharacter].StatDescription;
        statDescriptionText.color = characterColors[currentlySelectedCharacter];

        // visual stuff
        characterImage.sprite = characterList[currentlySelectedCharacter].CharacterSprite;

        characterBackgroundImage.color = characterColors[currentlySelectedCharacter];

        for (int i = 0; i < weaponImages.Length; i++)
        {
            weaponImages[i].sprite = characterList[currentlySelectedCharacter].WeaponSprites[i];
        }

        // 0- scroll bar, 1- scroll bar handle
        scrollBarElements[0].color = characterColors[currentlySelectedCharacter];
        scrollBarElements[1].color = characterColors[currentlySelectedCharacter];
    }

    public void DisplaySelectedWeaponInfo(int weaponIndex)
    {
        weaponHeaderText.text = characterList[currentlySelectedCharacter].WeaponHeader[weaponIndex];
        weaponHeaderText.color = characterColors[currentlySelectedCharacter];

        weaponDescriptionText.text = characterList[currentlySelectedCharacter].WeaponDescription[weaponIndex];
        weaponDescriptionText.color = characterColors[currentlySelectedCharacter];
    }

    public void ConfirmCharacter()
    {

    }
}
