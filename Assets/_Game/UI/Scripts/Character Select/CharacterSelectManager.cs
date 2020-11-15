using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;

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
    [SerializeField] GameObject[] rightHandPanel;
    [SerializeField] Image backgroundImage;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] TextMeshProUGUI damageText;

    // weapon info
    [SerializeField] GameObject[] weaponSelectedBorder;
    [SerializeField] TextMeshProUGUI weaponNameText;
    [SerializeField] TextMeshProUGUI speedText;
    [SerializeField] TextMeshProUGUI ammoText;
    int currentlySelectedWeapon = 0;
    int previouslySelectedWeapon = 0;

    [SerializeField] Image confirmGlow;

    AudioScript audioScript;

    private void Awake()
    {
        // animate first character border out
        characterButtons[currentlySelectedCharacter].AnimateCharacterBorderOut();

        UpdateCharacterInfo();
    }

    private void Start()
    {
        audioScript = GetComponent<AudioScript>();

        ConfirmGlow();  
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }

        ControllerSupport();
    }

    void ControllerSupport()
    {
        if (Input.GetJoystickNames().Length != 0)
        {
            if (Input.GetKeyDown(KeyCode.Joystick1Button2))
            {
                ConfirmCharacter("Main");
            }
        }
    }

    // happens when player hovers over character border on the left of the screen
    // animate character border in/out
    public void OnHoverCharacterButtonAnimation(int index)
    {
        if (index != currentlySelectedCharacter)
        {
            previouslySelectedCharacter = currentlySelectedCharacter;
            currentlySelectedCharacter = index;

            characterButtons[previouslySelectedCharacter].AnimateCharacterBorderIn();
            characterButtons[currentlySelectedCharacter].AnimateCharacterBorderOut();
        }
    }

    // update character image in the center of the screen
    void UpdateCharacterImage()
    {
        if (!characterList[currentlySelectedCharacter].IsLocked)
        {
            characterImage.sprite = characterList[currentlySelectedCharacter].CharacterImage;
            characterImage.transform.localScale = new Vector3(1f, 1f, 1f);
            characterImage.SetNativeSize();
        }
        else
        {
            characterImage.sprite = characterList[currentlySelectedCharacter].CharacterImage;
            characterImage.SetNativeSize();
            characterImage.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            characterImage.transform.localPosition = new Vector3(-690, characterImage.transform.localPosition.y, characterImage.transform.localPosition.z);
        }
    }

    // update character stats on the right of the screen
    public void UpdateCharacterInfo()
    {
        UpdateCharacterImage();

        if (!characterList[currentlySelectedCharacter].IsLocked)
        {
            foreach(GameObject o in rightHandPanel)
            {
                o.SetActive(true);
            }
            backgroundImage.sprite = characterList[currentlySelectedCharacter].BackgroundImage;
            backgroundImage.SetNativeSize();
            backgroundImage.transform.localPosition = new Vector3(60, backgroundImage.transform.localPosition.y, backgroundImage.transform.localPosition.z);

            confirmGlow.gameObject.SetActive(true);

            // character info
            healthText.text = characterList[currentlySelectedCharacter].Health.ToString() + " HP";
            damageText.text = characterList[currentlySelectedCharacter].Damage.ToString();

            // weapon info
            DisplaySelectedWeaponInfo(0);
        }
        else
        {
            foreach (GameObject o in rightHandPanel)
            {
                o.SetActive(false);
            }

            confirmGlow.gameObject.SetActive(false);

            backgroundImage.sprite = characterList[currentlySelectedCharacter].LockedImage;
            backgroundImage.SetNativeSize();
            backgroundImage.transform.localPosition = new Vector3(82, backgroundImage.transform.localPosition.y, backgroundImage.transform.localPosition.z);
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

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void ConfirmCharacter(string sceneName)
    {
        audioScript.PlaySound(0);

        //For now just start the game. Later will actually change character.
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        GameManager.Instance.GameState = DroseraGlobalEnums.GameState.CutScene; 
    }

    void ConfirmGlow()
    {
        StartCoroutine(ConfirmGlowCoroutine());
    }

    IEnumerator ConfirmGlowCoroutine()
    {
        while(true)
        {
            confirmGlow.DOFade(0.25f, 0.5f);

            yield return new WaitForSeconds(0.5f);

            confirmGlow.DOFade(1, 0.5f);

            yield return new WaitForSeconds(0.5f);
        }
    }
}
