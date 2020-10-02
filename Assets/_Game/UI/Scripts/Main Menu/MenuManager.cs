using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
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

    private void Awake()
    {
        foreach (Image h in hoverlineImages)
        {
            h.enabled = false;
        }
    }

    private void Start()
    {
        DisplayMainMenuPanel();
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
    }

    public void StartGame(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void DisplayMainMenuPanel()
    {
        CloseAllPanels();
        mainMenuPanel.SetActive(true);
    }

    public void DisplayInstructionsPanel()
    {
        CloseAllPanels();
        instructionsPanel.SetActive(true);
    }

    public void DisplayExtrasPanel()
    {
        CloseAllPanels();
        extrasPanel.SetActive(true);
    }

    public void DisplaySettingsPanel()
    {
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
        Application.Quit();
    }
}
