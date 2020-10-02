using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class CharacterSelectManager : MonoBehaviour
{
    int currentlySelectedCharacter = 0;
    int previouslySelectedCharacter = 0;

    [SerializeField] CharacterSelectInfo[] characterList;

    [Header("Left-Hand Stuff")]
    [SerializeField] CharacterButtonAnim[] characterButtons;    // for animating character buttons on hover

    [Header("Center Stuff")]
    [SerializeField] Image characterImage;

    [Header("Right-Hand Stuff")]
    // character info
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] TextMeshProUGUI damageText;

    // weapon info
    [SerializeField] GameObject[] weaponSelectedBorder;
    [SerializeField] TextMeshProUGUI weaponNameText;
    [SerializeField] TextMeshProUGUI speedText;
    [SerializeField] TextMeshProUGUI ammoText;
    int currentlySelectedWeapon = 0;
    int previouslySelectedWeapon = 0;

    private void Awake()
    {
        // animate first character border out
        characterButtons[currentlySelectedCharacter].AnimateCharacterBorderOut();

        UpdateCharacterInfo();
    }

    // happens when player hovers over character border on the left of the screen
    // animate character border in/out
    public void OnHoverCharacterButtonAnimation(int index)
    {
        if (index != currentlySelectedCharacter)
        {
            previouslySelectedCharacter = currentlySelectedCharacter;
            currentlySelectedCharacter = index;

            if (!characterList[currentlySelectedCharacter].IsLocked)
            {
                characterButtons[previouslySelectedCharacter].AnimateCharacterBorderIn();
                characterButtons[currentlySelectedCharacter].AnimateCharacterBorderOut();
            }
            // TODO- play locked animation instead 
            else
            {

            }
        }
    }

    // update character image in the center of the screen
    void UpdateCharacterImage()
    {
        characterImage.sprite = characterList[currentlySelectedCharacter].CharacterImage;
        characterImage.SetNativeSize();
    }

    // update character stats on the right of the screen
    public void UpdateCharacterInfo()
    {
        if (!characterList[currentlySelectedCharacter].IsLocked)
        {
            UpdateCharacterImage();

            // character info
            healthText.text = characterList[currentlySelectedCharacter].Health.ToString() + " HP";
            damageText.text = characterList[currentlySelectedCharacter].Damage.ToString();

            // weapon info
            DisplaySelectedWeaponInfo(0);
        }
    }

    // happens when player clicks on a weapon image
    // highlights button and displays currently selected weapon info
    public void DisplaySelectedWeaponInfo(int weaponIndex)
    {
        previouslySelectedWeapon = currentlySelectedWeapon;
        currentlySelectedWeapon = weaponIndex;

        weaponSelectedBorder[previouslySelectedWeapon].SetActive(false);
        weaponSelectedBorder[currentlySelectedWeapon].SetActive(true);

        weaponNameText.text = characterList[currentlySelectedCharacter].WeaponName[weaponIndex];

        // show speed and ammo stats for primary(0) and secondary(1) weap
        if (weaponIndex == 0 || weaponIndex == 1)
        {
            speedText.text = characterList[currentlySelectedCharacter].Speed[weaponIndex];
            ammoText.text = characterList[currentlySelectedCharacter].Ammo[weaponIndex] + " / " + characterList[currentlySelectedCharacter].Ammo[weaponIndex].ToString();
        }
        // TODO- show passive description
        else
        {

        }
    }

    public void ConfirmCharacter()
    {

    }
}
