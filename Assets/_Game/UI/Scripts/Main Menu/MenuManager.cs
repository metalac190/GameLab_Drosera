using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class MenuManager : MonoBehaviour
{
    [Header("Animations")]
    [SerializeField] float initialFadeTime;
    [SerializeField] float fadeInTime;
    public bool gameStart = false;

    [Header("Panels")]
    [SerializeField] GameObject mainMenuPanel;
    [SerializeField] GameObject instructionsPanel;
    [SerializeField] GameObject extrasPanel;
    [SerializeField] GameObject settingsPanel;

    [Header("Buttons")]
    [SerializeField] Image[] hoverlineImages;
    [SerializeField] Image[] buttonImages;
    [SerializeField] Sprite[] whiteText;
    [SerializeField] Sprite[] coloredText;
    int currentlySelectedButton = -1;
    int previouslySelectedButton = -1;

    // controller support
    bool axisInUse = false;
    bool cycleDown;
    bool cycleUp;

    private void Awake()
    {
        foreach (Image h in hoverlineImages)
        {
            h.enabled = false;
        }
    }

    private void Start()
    {
        FadeInMainMenuPanel();
        DisplayMainMenuPanel();
    }

    void FadeInMainMenuPanel()
    {
        Image[] imagesInMainMenu = mainMenuPanel.GetComponentsInChildren<Image>();

        // TODO- move this to Game Manager (need DontDestroyOnLoad)
        if (gameStart)
        {
            foreach (Image i in imagesInMainMenu)
            {
                i.DOFade(1, fadeInTime);
            }
        }
        else
        {
            foreach (Image i in imagesInMainMenu)
            {
                i.DOFade(1, initialFadeTime);
            }

            gameStart = true;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DisplayMainMenuPanel();
        }

        ControllerSupport();
    }

    void ControllerSupport()
    {
        if (Input.GetJoystickNames().Length != 0)
        {
            cycleUp = Input.GetAxisRaw("Vertical") == 1;
            cycleDown = Input.GetAxisRaw("Vertical") == -1;

            if (cycleDown && currentlySelectedButton == -1)
            {
                currentlySelectedButton = 0;
                OnHoverMenuButton(currentlySelectedButton);

                return;
            }

            if (cycleDown && !axisInUse)
            {
                if (currentlySelectedButton + 1 <= 4)
                    OnHoverMenuButton(currentlySelectedButton + 1);

                axisInUse = true;
                StartCoroutine(ControllerAxisCooldown());
            }
            if (cycleUp && !axisInUse)
            {
                if (currentlySelectedButton - 1 >= 0)
                    OnHoverMenuButton(currentlySelectedButton - 1);

                axisInUse = true;
                StartCoroutine(ControllerAxisCooldown());
            }

            // confirm
            if (Input.GetKeyDown(KeyCode.Joystick1Button2))
            {
                ControllerConfirm();
            }
            // go back
            if (Input.GetKeyDown(KeyCode.Joystick1Button1))
            {
                DisplayMainMenuPanel();
            }
        }
    }

    IEnumerator ControllerAxisCooldown()
    {
        yield return new WaitForSeconds(0.1f);

        axisInUse = false;
    }

    void ControllerConfirm()
    {
        switch (currentlySelectedButton)
        {
            case 0:
                StartGame("Main");
                break;
            case 1:
                DisplayInstructionsPanel();
                break;
            case 2:
                DisplayExtrasPanel();
                break;
            case 3:
                DisplaySettingsPanel();
                break;
            case 4:
                QuitGame();
                break;
        }
    }

    void ControllerActionsForExtras()
    {

    }

    void ControllerActionsForSettings()
    {

    }

    void PlaySound(int clipIndex)
    {
        AudioScript audioScript = buttonImages[currentlySelectedButton].GetComponent<AudioScript>();
        audioScript.PlayOneSound(clipIndex);
    }

    // highlight menu button on hover
    public void OnHoverMenuButton(int index)
    {
        if (index != currentlySelectedButton)
        {
            previouslySelectedButton = currentlySelectedButton;
            currentlySelectedButton = index;

            buttonImages[currentlySelectedButton].sprite = coloredText[currentlySelectedButton];
            buttonImages[currentlySelectedButton].SetNativeSize();

            hoverlineImages[currentlySelectedButton].enabled = true;

            if (previouslySelectedButton >= 0)
            {
                buttonImages[previouslySelectedButton].sprite = whiteText[previouslySelectedButton];
                buttonImages[previouslySelectedButton].SetNativeSize();

                hoverlineImages[previouslySelectedButton].enabled = false;
            }
        }

        PlaySound(1);
    }

    void ClearButtons()
    {
        for (int i = 0; i < buttonImages.Length; i++)
        {
            buttonImages[i].sprite = whiteText[i];
            hoverlineImages[i].enabled = false;
        }
    }

    public void StartGame(string name)
    {
        PlaySound(0);

        SceneManager.LoadScene(name);
    }

    public void DisplayMainMenuPanel()
    {
        CloseAllPanels();
        ClearButtons();

        mainMenuPanel.SetActive(true);
    }

    public void DisplayInstructionsPanel()
    {
        PlaySound(0);

        CloseAllPanels();
        instructionsPanel.SetActive(true);
    }

    public void DisplayExtrasPanel()
    {
        PlaySound(0);

        CloseAllPanels();
        extrasPanel.SetActive(true);
    }

    public void DisplaySettingsPanel()
    {
        PlaySound(0);

        CloseAllPanels();
        settingsPanel.SetActive(true);
    }

    public void CloseAllPanels()
    {
        mainMenuPanel.SetActive(false);
        instructionsPanel.SetActive(false);
        extrasPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    public void QuitGame()
    {
        PlaySound(0);

        Application.Quit();
    }
}
